using UnityEngine;
using TMPro;

public class InsultScript : MonoBehaviour
{
    [Header("Insults")]
    public string[] insults;

    [Header("UI Reference")]
    public TextMeshProUGUI insultText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DisplayRandomInsult();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public string GetRandomInsult()
    {
        if (insults.Length == 0) return "Checkmate, stupid";
        int randomIndex = Random.Range(0, insults.Length);
        return insults[randomIndex];
    }

    public void DisplayRandomInsult()
    {
        if (insultText != null)
        {
            insultText.text = GetRandomInsult();
        }
    }

    public void ClearInsult()
    {
        if (insultText != null)
        {
            insultText.text = "";
        }
    }
}
