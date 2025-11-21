using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PieceInfoName : MonoBehaviour
{
    public TextMeshProUGUI displayText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    // Update is called once per frame
    void Update()
    {

    }

    public void disTex(string text)
    {
        displayText.text = text;
    }

    public void ClearText()
    {
        if (displayText != null)
        {
            displayText.text = "";
        }
    }
}
