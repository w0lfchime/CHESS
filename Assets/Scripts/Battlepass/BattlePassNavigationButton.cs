using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BattlePassNavigationButton : MonoBehaviour
{
    [SerializeField] private BPSpawner battlePassSpawner;
    [SerializeField] private bool isNextButton = true;
    
    private Button button;
    
    private void Awake()
    {
        button = GetComponent<Button>();
        
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }
    
    private void OnButtonClick()
    {
        if (battlePassSpawner == null) return;
        
        if (isNextButton)
        {
            battlePassSpawner.NextPage();
        }
        else
        {
            battlePassSpawner.PreviousPage();
        }
    }
    
    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }
}
