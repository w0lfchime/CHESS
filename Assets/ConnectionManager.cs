using Steamworks;             // Steamworks.NET
using PurrNet;
using PurrNet.Steam;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace SteamExample
{
    public sealed class ConnectionManager : NetworkIdentity
    {
        [Header("Core UI")]
        [SerializeField] private Button hostButton;
        [SerializeField] private Button startGameButton;
        [SerializeField] private Button backButton;

        [Header("Scene To Load")]
        [SerializeField] private string gameSceneName = "GameScene";

        [Header("Player Requirements")]
        [Tooltip("Total players required (host + clients) before Start button is usable.")]
        [SerializeField] private int requiredPlayers = 2;

        [Header("Steam")]
        [Tooltip("Your Steam AppID. Use 480 for local testing, replace with your own for production.")]
        [SerializeField] private uint appId = 480;

        [Header("Friends Hosting List UI")]
        [Tooltip("The ScrollRect that will contain your friend buttons.")]
        [SerializeField] private ScrollRect friendsScrollRect;

        [Tooltip("The content Transform inside the ScrollRect (where buttons will be parented).")]
        [SerializeField] private Transform friendsContentRoot;

        [Tooltip("A prefab GameObject with a Button + Text/TMP_Text child that will display a friend's name.")]
        [SerializeField] private GameObject friendButtonPrefab;

        [Tooltip("How often (in seconds) to refresh the list of hosting friends.")]
        [SerializeField] private float friendsRefreshInterval = 5f;

        [Header("Testing")]
        [Tooltip("If true, also show a 'self' entry so another instance (editor/build) can join your own host for testing.")]
        [SerializeField] private bool allowSelfJoinForTesting = true;

        private bool _steamInitialized;
        private bool _isHosting;
        private float _friendsRefreshTimer;

        private void Awake()
        {
            InstanceHandler.RegisterInstance(this);
            DontDestroyOnLoad(gameObject);

            InitializeSteam();

            if (startGameButton != null)
                startGameButton.interactable = false;

            if (backButton != null)
                backButton.interactable = false;

            if (_steamInitialized)
                PopulateFriendsHostingList();
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

            if (_isHosting && _steamInitialized)
            {
                // Clear hosting flag when we go away
                SteamFriends.ClearRichPresence();
            }

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
                SteamAPI.RunCallbacks();

                // Periodically refresh who is hosting among your friends
                _friendsRefreshTimer += Time.deltaTime;
                if (_friendsRefreshTimer >= friendsRefreshInterval)
                {
                    _friendsRefreshTimer = 0f;
                    PopulateFriendsHostingList();
                }
            }

            // Only the host/server cares about enabling Start Game based on player count
            if (_isHosting && NetworkManager.main != null && NetworkManager.main.isServer)
            {
                // If your PurrNet version uses a different property name,
                // change playerCount accordingly (e.g. connectionCount).
                int playerCount = NetworkManager.main.playerCount;
                bool hasEnoughPlayers = playerCount >= requiredPlayers;

                if (startGameButton != null)
                    startGameButton.interactable = hasEnoughPlayers;
            }
        }

        // --------------------------------------------------------------------
        // Steam + PurrNet Core
        // --------------------------------------------------------------------

        private void InitializeSteam()
        {
            if (_steamInitialized)
                return;

            AppId_t appId_t = new AppId_t(appId);

            if (SteamAPI.RestartAppIfNecessary(appId_t))
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

            CSteamID userId = SteamUser.GetSteamID();
            string address = userId.m_SteamID.ToString();

            steamTransport.peerToPeer = true;
            steamTransport.dedicatedServer = false;
            steamTransport.address = address;

            NetworkManager.main.StartHost();
            _isHosting = true;

            // Mark ourselves as hosting in Steam Rich Presence so friends (and our own 2nd instance) can see us
            SteamFriends.SetRichPresence("purr_host", "1");
            SteamFriends.SetRichPresence("steam_display", "#StatusHosting");

            if (backButton != null)
                backButton.interactable = true;

            if (startGameButton != null)
                startGameButton.interactable = false;
        }

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
        // Friends Hosting List (+ self hosting entry)
        // --------------------------------------------------------------------

        private void PopulateFriendsHostingList()
        {
            if (!_steamInitialized)
                return;

            if (friendsContentRoot == null || friendButtonPrefab == null)
            {
                // Not wired in inspector, just skip UI
                return;
            }

            // Clear existing children
            for (int i = friendsContentRoot.childCount - 1; i >= 0; i--)
            {
                Destroy(friendsContentRoot.GetChild(i).gameObject);
            }

            AppId_t thisAppId = new AppId_t(appId);

            // Optional: self entry for testing (join your own host from another instance)
            if (allowSelfJoinForTesting)
            {
                CSteamID selfId = SteamUser.GetSteamID();
                string selfName = SteamFriends.GetPersonaName();

                // We don't strictly need to check app/game here: if you're running this,
                // you're in this game. The host instance will set rich presence when hosting.
                // This lets a second instance (editor or build) click and join.
                CreateHostingEntryButton(
                    label: $"{selfName} (You)",
                    hostSteamId: selfId.m_SteamID.ToString()
                );
            }

            int friendCount = SteamFriends.GetFriendCount(EFriendFlags.k_EFriendFlagImmediate);

            for (int i = 0; i < friendCount; i++)
            {
                CSteamID friendId = SteamFriends.GetFriendByIndex(i, EFriendFlags.k_EFriendFlagImmediate);
                string friendName = SteamFriends.GetFriendPersonaName(friendId);

                // 1) Are they in a game at all, and is it THIS game?
                if (!SteamFriends.GetFriendGamePlayed(friendId, out FriendGameInfo_t gameInfo))
                    continue;

                if (!gameInfo.m_gameID.IsValid())
                    continue;

                // Check AppID matches
                if (!gameInfo.m_gameID.AppID().Equals(thisAppId))
                    continue;

                // 2) Are they marked as hosting via our rich presence key?
                string hostFlag = SteamFriends.GetFriendRichPresence(friendId, "purr_host");
                if (string.IsNullOrEmpty(hostFlag) || hostFlag != "1")
                    continue;

                // Now we consider them "hosting this game", so we add a button
                CreateHostingEntryButton(
                    label: friendName,
                    hostSteamId: friendId.m_SteamID.ToString()
                );
            }
        }

        private void CreateHostingEntryButton(string label, string hostSteamId)
        {
            GameObject go = Instantiate(friendButtonPrefab, friendsContentRoot);
            go.name = $"Host_{label}";

            Button friendButton = go.GetComponent<Button>();
            if (friendButton == null)
                friendButton = go.GetComponentInChildren<Button>();

            if (friendButton == null)
            {
                Debug.LogError("Friend button prefab has no Button component.", go);
                Destroy(go);
                return;
            }

            // Set the label text (TMP or legacy Text)
            TMP_Text tmpText = go.GetComponentInChildren<TMP_Text>();
            if (tmpText != null)
                tmpText.text = label;
            else
            {
                Text uiText = go.GetComponentInChildren<Text>();
                if (uiText != null)
                    uiText.text = label;
            }

            friendButton.onClick.AddListener(() =>
            {
                Debug.Log($"Joining host: {label} ({hostSteamId})");
                StartClient(hostSteamId);
            });
        }

        // --------------------------------------------------------------------
        // UI Handlers
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

                // Also clear rich presence if host failed
                if (_steamInitialized)
                    SteamFriends.ClearRichPresence();

                return;
            }

            hostButton.interactable = false;
        }

        private void HandleStartGameClicked()
        {
            if (!_isHosting || NetworkManager.main == null || !NetworkManager.main.isServer)
                return;

            int playerCount = NetworkManager.main.playerCount;

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

            // Load a networked scene for all connections
            NetworkManager.main.sceneModule.LoadSceneAsync(gameSceneName);
        }

        private void HandleBackClicked()
        {
            if (NetworkManager.main != null)
            {
                NetworkManager.main.StopServer();
            }

            _isHosting = false;

            if (_steamInitialized)
            {
                // We are no longer hosting, clear presence
                SteamFriends.ClearRichPresence();
            }

            if (hostButton != null)
                hostButton.interactable = true;

            if (startGameButton != null)
                startGameButton.interactable = false;

            if (backButton != null)
                backButton.interactable = false;
        }
    }
}
