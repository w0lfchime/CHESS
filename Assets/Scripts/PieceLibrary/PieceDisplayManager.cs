using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PieceDisplayManager : MonoBehaviour
{
    [SerializeField] public TextMeshProUGUI titleText;
    [SerializeField] public TextMeshProUGUI taglineText;
    [SerializeField] public TextMeshProUGUI materialValueText;
    [SerializeField] public TextMeshProUGUI descriptionText;
    [SerializeField] public TextMeshProUGUI abilityNameText;
    [SerializeField] public GameObject pieceModelHolder;
    [SerializeField] public Button previousAbilityButton;
    [SerializeField] public Button nextAbilityButton;
    [SerializeField] public GameObject pieceMoveDiagram;
    public int currentAbility;
}
