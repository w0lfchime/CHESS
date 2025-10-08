using TMPro;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TitleScreenButtons : MonoBehaviour
{
    public TextMeshProUGUI matText;
    public TextMeshProUGUI teamName;
    public GameObject mainMenu;
    public GameObject teamCreateMenu;
    public GameObject teamSelectMenu;
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
        blackDropDown.AddOptions(gameData.blackTeamNames);
        whiteDropDown.AddOptions(gameData.whiteTeamNames);

        teamSelectMenu.SetActive(true);
        mainMenu.SetActive(false);
    }

    public void StartGame()
    {
        if (gameData.whiteTeamNames.Count != 0 && gameData.blackTeamNames.Count != 0)
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
        mainMenu.SetActive(true);
    }

    public void SaveTeam()
    {
        int lifeLineCount = 0;

        for (int i = 0; i < 16; i++)
        {
            if (tempTeam[i] != null && tempTeam[i] == "StandardKing")
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

            if (teamOn == 0)
            {
                gameData.whiteTeams.Add(copy);
                gameData.whiteTeamNames.Add(teamName.text);
            }
            else
            {
                gameData.blackTeams.Add(copy);
                gameData.blackTeamNames.Add(teamName.text);
            }
        }
        
    }

    public void updateMatText()
    {
        matText.text = "Material Value: " + matValue;
    }

    private void makeSlot(int slotNum, GameObject addTo)
    {
        GameObject slot = Instantiate(slotPrefab);
        TeamSlotRework teamSlot = slot.GetComponent<TeamSlotRework>();

        teamSlot.slotNum = slotNum;

        slot.transform.SetParent(addTo.transform);
    }
}
