using UnityEngine;

[CreateAssetMenu(fileName = "NewBattlePassItem", menuName = "Battle Pass/Battle Pass Item")]
public class BattlePassItem : ScriptableObject
{
    [Header("Item Information")]
    [Tooltip("Name of the battle pass item")]
    public string itemName;

    [Header("Visual Settings")]
    [Tooltip("3D Model prefab for the item (optional)")]
    public GameObject modelPrefab;

    [Tooltip("2D Image/Sprite for the item (optional - used if no model is provided)")]
    public Sprite itemSprite;

    [Tooltip("Color of the light that shines below the item when hovered")]
    public Color lightColor = Color.white;

    [Header("Display Settings")]
    [Tooltip("Scale modifier for the display item")]
    public float displayScale = 1f;

    [Tooltip("Rotation offset for 3D models")]
    public Vector3 rotationOffset = Vector3.zero;
}
