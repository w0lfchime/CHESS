using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;

public class TitleScreenButtons : MonoBehaviour
{
    public static TitleScreenButtons Instance { get; private set;}
    public InsultScript insultScript;
    public TextMeshProUGUI matText;
    public TMP_InputField teamName;
    public TextMeshProUGUI errorTextText;
    public TextMeshProUGUI flavorText;
    public TextMeshProUGUI pieceDescText;
    public Image collectionsImage;
    public GameObject errorText;
    public GameObject mainMenu;
    public GameObject teamCreateMenu;
    public GameObject teamSelectMenu;
    public GameObject collectionsMenu;
    public GameObject how2ChessMenu;
    public GameObject teamContent;
    public GameObject slotPrefab;
    public GameObject creditsMenu;
    public GameObject singlePlayerMenu;
    public GameData gameData;
    public Transform whiteDropDown;
    public Transform blackDropDown;
    public Transform yourDropDown;
    public AudioClip[] titleScreenMusic;
    public AudioSource musicSource;
    public string[] tempTeam = new string[16];
    public List<MapData> mapList = new List<MapData>();
    public string[] puzzleFolders;
    public int maxMaterial = 39;
    public int matValue = 0;
    public int teamOn = 0;
    public DraggableUIPiece selectedPiece;
    public Slider materialSlider;
    public ScrollRect TeamView;
    public GameObject TeamViewPart;
    // mini collections
    public TextMeshProUGUI miniCollectionsName;
    public TextMeshProUGUI miniCollectionsDesc;
    public TextMeshProUGUI miniCollectionsAbilityName;
    public GameObject miniColelctionsGrid;
    public GameObject miniCollectionsContent;
    public GameObject noPieceSelected;
    public int abilityNumber = 0;
    // assigning custom mat stuff
    public int tempMatIndex = 0;
    public GameObject matSelectScreen;
    public GameObject puzzleButton;
    public Transform puzzleContent;
    public GameObject matButton;
    public Transform matContent;
    public GameObject whiteTeamContentDisplay, blackTeamContentDisplay, yourTeamContentDisplay;
    public GameObject teamVisualUI;
    public GameObject mapButton;
    public Transform mapDropdown;
    public Transform mapGrid;

    void Start()
    {
        Instance = this;
        gameData = GameObject.Find("GameData").GetComponent<GameData>();
        gameData.map = mapList[0];
        LoadTeams();
        CreatePuzzleUI();
        SpawnMats();
        CreateMapButtons();
        SetMap(0);

        for (int i = 0; i < 16; i++)
        {
            makeSlot(i, whiteTeamContentDisplay, true);
        }

        for (int i = 0; i < 16; i++)
        {
            makeSlot(i, blackTeamContentDisplay, true);
        }

        for (int i = 0; i < 16; i++)
        {
            makeSlot(i, yourTeamContentDisplay, true);
        }
    }

    void CreateMapButtons()
    {
        for(int i = 0; i < mapList.Count; i++){
            int temp = i;
            GameObject mapButtonIns = Instantiate(mapButton, mapDropdown);
            mapButtonIns.transform.GetChild(0).GetComponent<TMP_Text>().text = mapList[i].name;
            mapButtonIns.GetComponent<Toggle>().onValueChanged.AddListener((value) => { SetMap(temp); });
        }
    }

    public void DeleteAllTeams()
    {
        PlayerPrefs.DeleteAll();
        gameData.teams.Clear();
    }
    public void MoveToTeamCreation()
    {
        // Makes team slots
        for (int i = 0; i < 16; i++)
        {
            makeSlot(i, teamContent);
            tempTeam[i] = null;
        }

        matValue = 0;
        tempMatIndex = 0;
        teamName.text = "";

        mainMenu.SetActive(false);
        teamCreateMenu.SetActive(true);
    }

    public void MoveToTeamSelection()
    {
        PopulateYourDropdown();

        teamSelectMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void PopulateYourDropdown()
    {
        ClearOptions(yourDropDown);
        ClearOptions(blackDropDown);
        ClearOptions(whiteDropDown);

        AddOptions(yourDropDown, gameData.teams.Keys.ToList());
        AddOptions(blackDropDown, gameData.teams.Keys.ToList());
        AddOptions(whiteDropDown, gameData.teams.Keys.ToList());

        if(mapDropdown.childCount > 0) mapDropdown.GetChild(0).GetComponent<Toggle>().isOn = true;
        if(whiteDropDown.childCount > 0) whiteDropDown.GetChild(0).GetComponent<Toggle>().isOn = true;
        if(blackDropDown.childCount > 0) blackDropDown.GetChild(0).GetComponent<Toggle>().isOn = true;
    }

    void AddOptions(Transform content, List<string> names)
    {
        foreach (string name in names){
            GameObject teamVisualUIIns = Instantiate(teamVisualUI, content);
            teamVisualUIIns.transform.GetChild(0).GetComponent<TMP_Text>().text = name;
            teamVisualUIIns.GetComponent<Toggle>().onValueChanged.AddListener((value) => { SetTeam(name, content); });
        }
    }

    void ClearOptions(Transform content)
    {
        foreach(Transform child in content)
        {
            Destroy(child.gameObject);
        }
    }

    public void MoveToSingleplayer()
    {
        mainMenu.SetActive(false);
        singlePlayerMenu.SetActive(true);
    }

    public void MoveToCollections()
    {
        collectionsMenu.SetActive(true);
        mainMenu.SetActive(false);
        musicSource.clip = titleScreenMusic[1];
        musicSource.Play();
    }
    public void MoveTOHow2()
    {
        how2ChessMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void MoveToCredits()
    {
        creditsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void MoveOffCredits()
    {
        creditsMenu.SetActive(false);
        mainMenu.SetActive(true);
    }

    public void StartGame()
    {
        gameData.isDoingPuzzle = false;
        if (gameData.teams.Keys.Count != 0)
        {
            SceneManager.LoadScene(gameData.map.scene);    
        }
    }

    public void SwitchTeams()
    {
        if (teamOn == 0)
        {
            teamOn = 1;
        }
        else
        {
            teamOn = 0;
        }
    }

    public void SetTeam(string name, Transform dropdown)
    {
        if(dropdown == whiteDropDown){
            gameData.whiteTeamName = name;

            populateDisplayTeam(whiteTeamContentDisplay.transform, name);
        }
        if (dropdown == blackDropDown)
        {
            gameData.blackTeamName = name;

            populateDisplayTeam(blackTeamContentDisplay.transform, name);
        }
        if(dropdown == yourDropDown)
        {
            gameData.yourTeamName = name;

            populateDisplayTeam(yourTeamContentDisplay.transform, name);
        }
    }

    void populateDisplayTeam(Transform content, string name)
    {
        if(!gameData.teams.ContainsKey(name)) return;
        foreach(Transform child in content)
        {
            child.GetComponent<TeamSlotRework>().SetPiece("");
        }
        int i = 0;
        foreach(Transform child in content)
        {
            string pieceName = gameData.teams[name][i];
            child.GetComponent<TeamSlotRework>().SetPiece(pieceName);
            i++;
        }

        int tempMaterial = 0;
        for(int index = 0; index < 15; index++){
            if(gameData.teams[name][index]=="" || gameData.teams[name][index]==null) continue;
            tempMaterial+=PieceLibrary.Instance.GetPrefab(gameData.teams[name][index]).materialValue;
        }
        content.parent.parent.GetChild(1).GetComponent<Slider>().value = (float)tempMaterial/maxMaterial;
        content.parent.parent.GetChild(1).GetChild(2).GetComponent<TMP_Text>().text = "Material: "+tempMaterial;
    }

    public void SetMap(int index)
    {
        gameData.map = mapList[index];
        foreach (Transform child in mapGrid) {
            if(child.gameObject.activeSelf) Destroy(child.gameObject);
        }
        int i = 0;
        foreach(int tile in gameData.map.nullTiles)
        {
            GameObject tileIns = Instantiate(mapGrid.GetChild(0).gameObject, mapGrid);
            tileIns.SetActive(true);
            if (tile==2)
            {
                tileIns.GetComponent<Image>().enabled = false;
            }
            else
            {
                Color tempColor = ((i+(i/gameData.map.width))%2 == 0 ? Color.white : Color.gray);
                tempColor.a = .5f;
                tileIns.GetComponent<Image>().color = tempColor;
            }
            i++;
        }
        mapGrid.GetComponent<GridLayoutGroup>().constraintCount = gameData.map.width;
    }

    public void ResetTeams()
    {
        for (int i = 0; i < teamContent.transform.childCount; i++)
        {
            Destroy(teamContent.transform.GetChild(i).gameObject);
        }

        MoveToTeamCreation();
        updateMatText();
    }


    public void BackButton()
    {
        for (int i = 0; i < teamContent.transform.childCount; i++)
        {
            Destroy(teamContent.transform.GetChild(i).gameObject);
        }

        for (int i = 0; i < 16; i++)
        {
            tempTeam[i] = null;
        }

        matText.text = "Material Value: 0";

        PopulateYourDropdown();

        teamCreateMenu.SetActive(false);
        teamSelectMenu.SetActive(false);
        collectionsMenu.SetActive(false);
        how2ChessMenu.SetActive(false);
        singlePlayerMenu.SetActive(false);
        matSelectScreen.SetActive(false);
        mainMenu.SetActive(true);

        miniCollectionsContent.SetActive(false);
        noPieceSelected.SetActive(true);

        insultScript.DisplayRandomInsult();
    }

    public void BackChangeMusic()
    {
        BackButton();
        musicSource.clip = titleScreenMusic[0];
        musicSource.Play();
    }

    public void SinglePlayerBUtton()
    {
        insultScript.insultText.text = "Not in this demo dummy.";
    }

    public void SaveTeam()
    {
        string name = teamName.text;

        if (lifelineCount() == 1 && !(name.Length <= 1))
        {
            string[] copy = new string[18];
            copy[0] = name;
            copy[17] = "" + tempMatIndex;

            for (int i = 0; i < 16; i++)
            {
                copy[i+1] = tempTeam[i];
                Debug.Log(tempTeam[i]);
            }

            if (nameIndex(name) == -1)
            {
                AddTeamView(name);
                gameData.teams.Add(name, copy.Skip(1).ToArray());
                PlayerPrefs.SetString(getNumber().ToString(), string.Join(':', copy));
                print(PlayerPrefs.GetString(getNumber().ToString()));
            }
            else
            {
                string index = nameIndex(name).ToString();
                PlayerPrefs.DeleteKey(index);
                gameData.teams[name] = copy;
                PlayerPrefs.SetString(index, string.Join(':', copy));
            }
        }
        else
        {
            StopAllCoroutines();
            matText.color = Color.white;

            if (name.Length <= 1)
            {
                StartCoroutine(showErrorText("Name too short"));
            }
            else if (lifelineCount() == 0)
            {
                StartCoroutine(showErrorText("Need a lifeline"));
            }
            else
            {
                StartCoroutine(showErrorText("Too many lifelines"));
            }
        }

    }

    public void displayMatSelect()
    {
        matSelectScreen.SetActive(true);
    }

    public void colseMatSelect()
    {
        matSelectScreen.SetActive(false);
    }

    void SpawnMats()
    {
        for(int i = 0; i < GameData.Instance.matList.Count; i++){
            GameObject buttonIns = Instantiate(matButton, matContent);
            if(!GameData.Instance.matList[i].locked || PlayerPrefs.HasKey("unlocked color" + i)) {
                var temp = i;
                buttonIns.GetComponent<Image>().color = Color.white;
                buttonIns.transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { cangeTempMat(temp); });
                print(i);
                buttonIns.transform.GetChild(1).GetChild(0).GetComponent<TMP_Text>().text = GameData.Instance.matList[i].material.name;
            }
            buttonIns.transform.GetChild(0).GetComponent<RawImage>().color = GameData.Instance.matList[i].material.color;
        }
    }

    public void cangeTempMat(int num)
    {
        tempMatIndex = num;
    }

    public void LoadTeams()
    {
        for(int i = 0; i < getNumber(); i++)
        {
            string[] teamData = PlayerPrefs.GetString(i.ToString()).Split(':');
            string name = teamData[0];
            string[] team = teamData.Skip(1).ToArray();
            if(!gameData.teams.ContainsKey(name)) gameData.teams.Add(name, team);
            AddTeamView(name);
        }
    }

    void AddTeamView(string name)
    {
        GameObject teamViewIns = Instantiate(TeamViewPart, TeamView.content.transform);
        teamViewIns.GetComponent<TeamButtonUI>().textAsset.text = name;
        LayoutRebuilder.ForceRebuildLayoutImmediate(TeamView.content.GetComponent<RectTransform>());
        PopulateYourDropdown();
    }

    public void EditTeam(string name)
    {
        int i = 0;
        foreach(Transform child in teamContent.transform)
        {
            child.GetComponent<TeamSlotRework>().SetPiece("");
        }
        foreach(Transform child in teamContent.transform)
        {
            string pieceName = gameData.teams[name][i];
            child.GetComponent<TeamSlotRework>().SetPiece(pieceName);
            i++;
        }
        teamName.text = name;
    }

    public void RemoveTeam(string name)
    {
        int num = nameIndex(name);
        if (num == -1)
        {
            Debug.LogWarning($"RemoveTeam: team '{name}' not found.");
            return;
        }

        int count = getNumber(); // number of existing

        // Shift everything after num down by one
        for (int i = num + 1; i < count; i++)
        {
            string currentString = PlayerPrefs.GetString(i.ToString());
            PlayerPrefs.SetString((i - 1).ToString(), currentString);
        }

        // Delete the last
        PlayerPrefs.DeleteKey((count - 1).ToString());
        gameData.teams.Remove(name);
        PopulateYourDropdown();
    }


    public int nameIndex(string name)
    {
        bool search = true;
        int index = 0;
        bool hasname = false;
        while(search)
        {
            if(!PlayerPrefs.HasKey(index.ToString())) break;
            if(name == PlayerPrefs.GetString(index.ToString()).Split(':')[0])
            {
                search = false;
                hasname = true;
                return index;
            }
            index++;
        }

        return -1;
    }

    public int getNumber()
    {
        int index = 0;
        while(PlayerPrefs.HasKey(index.ToString()))
        {
            index++;
        }

        return index;
    }

    public void updateCollectionMenu(GameObject clicked)
    {
        CollectionIcon ci = clicked.GetComponent<CollectionIcon>();

        collectionsImage.sprite = ci.sprite;
        collectionsImage.color = new Color(collectionsImage.color.r, collectionsImage.color.g, collectionsImage.color.b, 1f);
        flavorText.text = ci.flavorText;
        pieceDescText.text = ci.desc;
    }

    public void editMatTextStuff(string error)
    {
        Debug.Log("Started");
        StopAllCoroutines();
        matText.color = Color.white;
        StartCoroutine(flashMatText(1));
        StartCoroutine(showErrorText(error));
    }

    public void editMatTextNotError(string text)
    {
        StopAllCoroutines();
        matText.color = Color.white;
        StartCoroutine(showErrorText(text));
    }

    public void updateMatText()
    {
        materialSlider.value = (float)matValue/(float)maxMaterial;
        matText.text = "Material: " + matValue;
    }

    private void makeSlot(int slotNum, GameObject addTo, bool visual = false)
    {
        GameObject slot = Instantiate(slotPrefab);
        TeamSlotRework teamSlot = slot.GetComponent<TeamSlotRework>();
        teamSlot.visual = visual;

        teamSlot.slotNum = slotNum;

        slot.transform.SetParent(addTo.gameObject.transform);
    }

    public int lifelineCount()
    {
        int lifeLineCount = 0;

        for (int i = 0; i < 16; i++)
        {
            if (tempTeam[i] != null && PieceLibrary.Instance.GetPrefab(tempTeam[i]).lifeLine)
            {
                lifeLineCount++;
            }
        }

        return lifeLineCount;
    }

    public void CreatePuzzleUI()
    {
        List<Puzzles> puzzles = FindPuzzlesDataInFolders(puzzleFolders);
        print(puzzles.Count);
        foreach(Puzzles puzzle in puzzles)
        {
            GameObject puzzleIns = Instantiate(puzzleButton, puzzleContent);
            puzzleIns.transform.GetChild(0).GetComponent<TMP_Text>().text = puzzle.name;
            puzzleIns.GetComponent<Button>().onClick.AddListener(() => { SelectPuzzle(puzzle); });
        }
    }

    public void SelectPuzzle(Puzzles puzzle)
    {
        gameData.puzzle = puzzle;
        gameData.map = puzzle.mapData;
        gameData.isDoingPuzzle = true;

        SceneManager.LoadScene(gameData.map.scene);
    }

    public void updateMiniCollections()
    {
        miniCollectionsContent.SetActive(true);
        noPieceSelected.SetActive(false);
        miniCollectionsName.text = selectedPiece.pieceId.name;
        miniCollectionsDesc.text = selectedPiece.pieceId.description;
        abilityNumber = 0;
        miniCollectionsAbilityName.text = selectedPiece.pieceId.abilities[abilityNumber].name;
        updateAbilityGrid();
    }

    public void changedViewedAbility(int endAbility)
    {
        abilityNumber += endAbility;

        if(abilityNumber < 0)
        {
            abilityNumber = selectedPiece.pieceId.abilities.Count - 1;
        } else if (abilityNumber >= selectedPiece.pieceId.abilities.Count)
        {
            abilityNumber = 0;
        }

        updateAbilityGrid();
    }

    public void updateAbilityGrid()
    {
        miniCollectionsAbilityName.text = selectedPiece.pieceId.abilities[abilityNumber].name;

        int imgCount = 0;
        for (int i = 0; i < 11; i++)
        {
            for (int j = 0; j < 11; j++)
            {
                Elements element = selectedPiece.pieceId.abilities[abilityNumber].actions[0].visualGrid[i].values[j];
                Color baseColor;
                if ((i + j) % 2 == 0)
                {
                    baseColor = new Color(0.255f, 0.255f, 0.255f, 1.000f);
                }
                else
                {
                    baseColor = new Color(0.294f, 0.294f, 0.294f, 1.000f);
                }

                switch (element)
                {
                    case Elements.CanMove:
                        baseColor = baseColor;
                        break;
                    case Elements.CantMove:
                        baseColor = Color.green*3 * baseColor;
                        break;
                    case (Elements)2:
                        baseColor = Color.yellow*3 * baseColor;
                        break;
                    default:
                        baseColor = Color.red*3 * baseColor;
                        break;
                }

                bool darkTile = (i + j) % 2 == 0;
                float colorMult = darkTile ? 0.85f : 0.95f;
                miniColelctionsGrid.transform.GetComponentsInChildren<Image>()[imgCount].color = baseColor * colorMult;

                imgCount++;
            }
        }
    }

    public IEnumerator showErrorText(string error)
    {

        errorTextText.text = error;
        errorText.SetActive(true);
        yield return new WaitForSeconds(2f);
        errorText.SetActive(false);
    }

    IEnumerator flashMatText(int on)
    {
        if(on != 6){
            matText.color = Color.red;
            yield return new WaitForSeconds(0.2f);
            matText.color = Color.white;
            yield return new WaitForSeconds(0.2f);
            StartCoroutine(flashMatText(on + 1));
        }
    }

    public static List<Puzzles> FindPuzzlesDataInFolders(string[] folders)
    {
        string filter = "t:Puzzles";
        string[] guids = AssetDatabase.FindAssets(filter, folders);

        var assets = new List<Puzzles>(guids.Length);

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            Puzzles asset = AssetDatabase.LoadAssetAtPath<Puzzles>(path);
            if (asset != null)
                assets.Add(asset);
        }

        return assets;
    }
}
