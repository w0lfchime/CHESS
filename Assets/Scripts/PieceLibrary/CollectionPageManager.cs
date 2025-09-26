using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionPageManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI collectionTitle;
    [SerializeField] public Toggle colorToggle;
    [SerializeField] public Transform pieceButtonsHolder;
    [SerializeField] public Button backButton;
    public string currentColor = "BLACK";
}
