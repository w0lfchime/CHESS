using Steamworks;             // Steamworks.NET
using PurrNet;
using PurrNet.Steam;
using UnityEngine;
using UnityEngine.UI;

namespace SteamExample
{
    /// <summary>
    /// Simple Steam + PurrNet connection manager using Steamworks.NET
    /// - Only a Host, StartGame, and Back button.
    /// - Host waits for at least one client, then StartGame loads the game scene.
    /// </summary>
    public sealed class ConnectionManager : NetworkIdentity
    {
        [Header("UI")]
        [SerializeField] private Button hostButton;
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button backButton;

        [Header("Scene To Load")]
        [SerializeField] private string gameSceneName = "GameScene";

        [Header("Player Requirements")]
        [Tooltip("Total players required (host + clients) before StartGame is allowed.")]
        [SerializeField] private int requiredPlayers = 2;

        private bool _steamInitialized;
        private bool _isHosting;

        private void Awake()
        {
            InstanceHandler.RegisterInstance(this);
            DontDestroyOnLoad(gameObject);

            InitializeSteam();

            if (startGameButton != null)
                startGameButton.interactable = false;

            if (backButton != null)
                backButton.interactable = false;
        }

        private void OnEnable()
        {
            if (hostButton != null)
                hostButton.onClick.AddListener(HandleHostClicked);

            if (startGameButton != null)
                startGameButton.onClick.AddListener(HandleStartGameClicked);

            if (backButton != null)
                backButton.onClick.AddListener(HandleBackClicked);
        }

        private void OnDisable()
        {
            if (hostButton != null)
                hostButton.onClick.RemoveListener(HandleHostClicked);

            if (startGameButton != null)
                startGameButton.onClick.RemoveListener(HandleStartGameClicked);

            if (backButton != null)
                backButton.onClick.RemoveListener(HandleBackClicked);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            InstanceHandler.UnregisterInstance<ConnectionManager>();

            if (_steamInitialized)
            {
                SteamAPI.Shutdown();
                _steamInitialized = false;
            }
        }

        private void Update()
        {
            if (_steamInitialized)
            {
                // Keep Steam callbacks processing
                SteamAPI.RunCallbacks();
            }

            // Only the host cares about player count for enabling the Start Game button
            if (_isHosting && NetworkManager.main != null && NetworkManager.main.isServer)
            {
                // NOTE: The exact property name may differ depending on your PurrNet version.
                // In the docs they refer to "player count" on the NetworkManager inspector.
                // If this line errors, try renaming playerCount → connections, clientCount, etc.
                int playerCount = NetworkManager.main.playerCount; 

                bool hasEnoughPlayers = playerCount >= requiredPlayers;

                if (startGameButton != null)
                    startGameButton.interactable = hasEnoughPlayers;
            }
        }

        /// <summary>
        /// Initialize Steamworks.NET.
        /// </summary>
        private void InitializeSteam()
        {
            if (_steamInitialized)
                return;

            // Replace 480 with your real AppID when you have it.
            const uint appId = 480;

            // If the user is running the game outside Steam, this will relaunch it via Steam.
            if (SteamAPI.RestartAppIfNecessary(new AppId_t(appId)))
            {
                Application.Quit();
                return;
            }

            if (!SteamAPI.Init())
            {
                Debug.LogError("Failed to initialize SteamAPI.");
                return;
            }

            _steamInitialized = true;
        }

        /// <summary>
        /// Configure SteamTransport and start hosting.
        /// </summary>
        public void StartHost()
        {
            if (!_steamInitialized)
            {
                Debug.LogError("Steam is not initialized. Cannot host.");
                return;
            }

            var steamTransport = NetworkManager.main.transport as SteamTransport;
            if (steamTransport == null)
            {
                Debug.LogError("SteamTransport missing on NetworkManager.", this);
                return;
            }

            // Get this user's SteamID from Steamworks.NET
            CSteamID userId = SteamUser.GetSteamID();
            string address = userId.m_SteamID.ToString();

            steamTransport.peerToPeer = true;
            steamTransport.dedicatedServer = false;
            steamTransport.address = address;

            NetworkManager.main.StartHost();
            _isHosting = true;

            // While hosting, back button becomes usable, start is disabled until client joins.
            if (backButton != null)
                backButton.interactable = true;

            if (startGameButton != null)
                startGameButton.interactable = false;
        }

        /// <summary>
        /// Start a client given a string SteamID of host (Hex or decimal).
        /// (You can call this from some other UI if you want client-side joining.)
        /// </summary>
        public void StartClient(string hostSteamIdString)
        {
            if (!_steamInitialized)
            {
                Debug.LogError("Steam is not initialized. Cannot join.");
                return;
            }

            var steamTransport = NetworkManager.main.transport as SteamTransport;
            if (steamTransport == null)
            {
                Debug.LogError("SteamTransport missing on NetworkManager.", this);
                return;
            }

            if (string.IsNullOrEmpty(hostSteamIdString))
            {
                Debug.LogError("Host Steam ID is empty.");
                return;
            }

            // You can customize how you parse this (decimal vs hex, etc.)
            if (!ulong.TryParse(hostSteamIdString, out ulong hostId))
            {
                Debug.LogError($"{hostSteamIdString} is not a valid SteamID (ulong parse failed).");
                return;
            }

            steamTransport.peerToPeer = true;
            steamTransport.dedicatedServer = false;
            steamTransport.address = hostId.ToString();

            NetworkManager.main.StartClient();
        }

        // --------------------------------------------------------------------
        // UI handlers
        // --------------------------------------------------------------------

        private void HandleHostClicked()
        {
            if (hostButton == null)
            {
                Debug.LogError("Host button is not assigned.", this);
                return;
            }

            StartHost();

            if (NetworkManager.main.isOffline)
            {
                Debug.LogError("Failed to start host – NetworkManager is offline.");
                _isHosting = false;

                if (backButton != null)
                    backButton.interactable = false;
                if (startGameButton != null)
                    startGameButton.interactable = false;

                return;
            }

            // Once we start hosting, we usually don't want to re-press host
            hostButton.interactable = false;
        }

        private void HandleStartGameClicked()
        {
            if (!_isHosting || NetworkManager.main == null || !NetworkManager.main.isServer)
            {
                // Only the server/host should be allowed to start the match
                return;
            }

            // Double-check we have enough players before loading the game scene
            int playerCount = NetworkManager.main.playerCount; // adjust property name if needed

            if (playerCount < requiredPlayers)
            {
                Debug.LogWarning($"Not enough players to start. Have {playerCount}, need {requiredPlayers}.");
                return;
            }

            if (string.IsNullOrEmpty(gameSceneName))
            {
                Debug.LogError("Game scene name is not set in ConnectionManager.");
                return;
            }

            // Use PurrNet's Scene Module to load a networked scene for all connections.
            // This runs on the server and automatically moves the clients.
            NetworkManager.main.sceneModule.LoadSceneAsync(gameSceneName);
        }

        private void HandleBackClicked()
        {
            // Cancel hosting / leave network and go back to "offline" state.
            if (NetworkManager.main != null)
            {
                // PurrNet exposes a Shutdown() API to stop networking completely.
                // If your version uses a different method (e.g. StopHost/StopClient),
                // replace the line below as appropriate.
                NetworkManager.main.StopServer();
            }

            _isHosting = false;

            // Reset UI state
            if (hostButton != null)
                hostButton.interactable = true;

            if (startGameButton != null)
                startGameButton.interactable = false;

            if (backButton != null)
                backButton.interactable = false;
        }
    }
}
