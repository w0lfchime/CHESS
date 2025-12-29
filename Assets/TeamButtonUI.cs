using TMPro;
using UnityEngine;

public class TeamButtonUI : MonoBehaviour
{
    public TMP_Text textAsset;
    public void EditButton()
    {
        TitleScreenButtons.Instance.EditTeam(textAsset.text);
    }
    public void RemoveButton()
    {
        Destroy(gameObject);
        TitleScreenButtons.Instance.RemoveTeam(textAsset.text);
    }
}
