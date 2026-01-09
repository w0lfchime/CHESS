using UnityEngine;

[CreateAssetMenu(fileName = "New ActionList", menuName = "Chess/Action List")]
public class ActionList : ScriptableObject
{
    public Action[] Actions;
}
