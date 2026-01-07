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
    public TMP_Dropdown whiteDropDown;
    public TMP_Dropdown blackDropDown;
    public TMP_Dropdown yourDropDown;
    public AudioClip[] titleScreenMusic;
    public AudioSource musicSource;
    public string[] tempTeam = new string[16];
    public List<MapData> mapList = new List<MapData>();
    public List<Puzzles> puzzleList = new List<Puzzles>();
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
    void Start()
    {
        Instance = this;
        gameData = GameObject.Find("GameData").GetComponent<GameData>();
        gameData.map = mapList[0];
        LoadTeams();
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
        yourDropDown.ClearOptions();
        blackDropDown.ClearOptions();
        whiteDropDown.ClearOptions();

        yourDropDown.AddOptions(gameData.teams.Keys.ToList());
        blackDropDown.AddOptions(gameData.teams.Keys.ToList());
        whiteDropDown.AddOptions(gameData.teams.Keys.ToList());
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
        SetWhiteTeam(0);
        SetBlackTeam(0);
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

    public void SetWhiteTeam(int team)
    {
        Debug.Log(whiteDropDown.options[whiteDropDown.value].text);
        gameData.whiteTeamName = whiteDropDown.options[whiteDropDown.value].text;

        // string[] theTeam = gameData.teams[whiteDropDown.options[whiteDropDown.value].text];

        // for(int i = 0; i < 16; i++)
        // {
        //     Debug.Log(theTeam[i]);
        // }
        // Debug.Log(gameData.teams[whiteDropDown.options[whiteDropDown.value].text]);
    }

    public void SetBlackTeam(int team)
    {
        Debug.Log(blackDropDown.options[blackDropDown.value].text);
        gameData.blackTeamName = blackDropDown.options[blackDropDown.value].text;

        // string[] theTeam = gameData.teams[blackDropDown.options[blackDropDown.value].text];

        // for(int i = 0; i < 16; i++)
        // {
        //     Debug.Log(theTeam[i]);
        // }
        // Debug.Log(gameData.teams[blackDropDown.options[blackDropDown.value].text]);
    }

    public void SetYourTeam(int team)
    {
        Debug.Log(yourDropDown.options[yourDropDown.value].text);
        gameData.yourTeamName = yourDropDown.options[yourDropDown.value].text;
    }

    public void SetMap(int index)
    {
        gameData.map = mapList[index];
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

        whiteDropDown.ClearOptions();
        blackDropDown.ClearOptions();

        teamCreateMenu.SetActive(false);
        teamSelectMenu.SetActive(false);
        collectionsMenu.SetActive(false);
        how2ChessMenu.SetActive(false);
        singlePlayerMenu.SetActive(false);
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
            string[] copy = new string[17];
            copy[0] = name;
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

    public void LoadTeams()
    {
        for(int i = 0; i < getNumber(); i++)
        {
            string[] teamData = PlayerPrefs.GetString(i.ToString()).Split(':');
            string name = teamData[0];
            string[] team = teamData.Skip(1).ToArray();
            gameData.teams.Add(name, team);
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

    private void makeSlot(int slotNum, GameObject addTo)
    {
        GameObject slot = Instantiate(slotPrefab);
        TeamSlotRework teamSlot = slot.GetComponent<TeamSlotRework>();

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

    public void SelectPuzzle(string nums)
    {
        string[] split = nums.Split(" ");

        gameData.puzzle = puzzleList[int.Parse(split[0])];
        gameData.map = mapList[int.Parse(split[1])];
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
                switch (element)
                {
                    case Elements.CanMove:
                        baseColor = Color.black;
                        break;
                    case Elements.CantMove:
                        baseColor = Color.green;
                        break;
                    case (Elements)2:
                        baseColor = Color.yellow;
                        break;
                    default:
                        baseColor = Color.red;
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
}
