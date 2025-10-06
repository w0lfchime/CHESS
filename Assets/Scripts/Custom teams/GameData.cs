using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameData : MonoBehaviour
{
    public GameObject mainMenu;
    public GameObject teamSelectMenu;
    public GameObject whiteTeamContent;
    public GameObject blackTeamContent;
    public GameObject slotPrefab;
    public TextMeshProUGUI[] matText = new TextMeshProUGUI[2];
    public string[] whiteTeamIds = new string[16];
    public string[] blackTeamIds = new string[16];
    public int maxMaterial = 39;
    public int[] matValues = new int[2];

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void MoveToTeamSelect()
    {
        // Makes team slots
        for (int i = 0; i < 16; i++)
        {
            makeSlot(1, i, blackTeamContent);
            makeSlot(0, i, whiteTeamContent);
        }

        mainMenu.SetActive(false);
        teamSelectMenu.SetActive(true);
    }

    public void StartGame()
    {
        int whiteLifelineCount = 0;
        int blackLifelineCount = 0;

        for (int i = 0; i < 16; i++)
        {
            if (whiteTeamIds[i] == "StandardKing")
            {
                whiteLifelineCount++;
            }

            if (blackTeamIds[i] == "StandardKing")
            {
                blackLifelineCount++;
            }
        }

        if (blackLifelineCount == 1 && whiteLifelineCount == 1)
        { 
            SceneManager.LoadScene("Welcome2Chess");
        }
    }

    public void updateMatText(int index)
    {
        matText[index].text = "Material Value: " + matValues[index];
    }

    private void makeSlot(int team, int slotNum, GameObject addTo)
    {
        GameObject slot = Instantiate(slotPrefab);
        TeamSlotRework teamSlot = slot.GetComponent<TeamSlotRework>();

        teamSlot.slotNum = slotNum;
        teamSlot.team = team;

        slot.transform.SetParent(addTo.transform);
    }
}
