using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenButtons : MonoBehaviour
{
    public InsultScript insultScript;
    public TextMeshProUGUI matText;
    public TextMeshProUGUI teamName;
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
    public GameData gameData;
    public TMP_Dropdown whiteDropDown;
    public TMP_Dropdown blackDropDown;
    public string[] tempTeam = new string[16];
    public int maxMaterial = 39;
    public int matValue = 0;
    public int teamOn = 0;
    void Start()
    {
        gameData = GameObject.Find("GameData").GetComponent<GameData>();    
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
        blackDropDown.AddOptions(gameData.teamNames);
        whiteDropDown.AddOptions(gameData.teamNames);

        teamSelectMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void MoveToCollections()
    {
        collectionsMenu.SetActive(true);
        mainMenu.SetActive(false);
    }
    public void MoveTOHow2()
    {
        how2ChessMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void StartGame()
    {
        if (gameData.teamNames.Count != 0)
        {
            SceneManager.LoadScene("Welcome2Chess");    
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
        Debug.Log(team);
        gameData.whiteTeamIndex = team;
    }

    public void SetBlackTeam(int team)
    {
        Debug.Log(team);
        gameData.blackTeamIndex = team;
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
        mainMenu.SetActive(true);

        insultScript.DisplayRandomInsult();
    }

    public void SinglePlayerBUtton()
    {
        insultScript.insultText.text = "Not in this demo dummy.";
    }

    public void SaveTeam()
    {
        int lifeLineCount = 0;

        for (int i = 0; i < 16; i++)
        {
            if (tempTeam[i] != null && (tempTeam[i] == "StandardKing" || tempTeam[i] == "MpregKing"))
            {
                lifeLineCount++;
            }
        }

        if (lifeLineCount == 1)
        {
            string[] copy = new string[16];

            for (int i = 0; i < 16; i++)
            {
                copy[i] = tempTeam[i];
            }

            gameData.teamList.Add(copy);
            gameData.teamNames.Add(teamName.text);
        }
        else
        {
            if (lifeLineCount == 0)
            {
                StartCoroutine(showErrorText("Need a lifeline"));
            }
            else
            {
                StartCoroutine(showErrorText("Too many lifelines"));
            }
        }

    }

    public void updateCollectionMenu(GameObject clicked)
    {
        CollectionIcon ci = clicked.GetComponent<CollectionIcon>();

        collectionsImage.sprite = ci.sprite;
        collectionsImage.color = new Color(collectionsImage.color.r, collectionsImage.color.g, collectionsImage.color.b, 1f);
        flavorText.text = ci.flavorText;
        pieceDescText.text = ci.desc;
    }

    public void editMatTextStuff()
    {
        Debug.Log("Started");
        StartCoroutine(flashMatText(1));
        StartCoroutine(showErrorText("Max 39"));
    }

    public void updateMatText()
    {
        matText.text = "Material: " + matValue;
    }

    private void makeSlot(int slotNum, GameObject addTo)
    {
        GameObject slot = Instantiate(slotPrefab);
        TeamSlotRework teamSlot = slot.GetComponent<TeamSlotRework>();

        teamSlot.slotNum = slotNum;

        slot.transform.SetParent(addTo.gameObject.transform);
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
