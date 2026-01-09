using UnityEngine;
using TMPro;

public class BattlePassItemDisplay : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private TextMeshPro nameText;
    [SerializeField] private Transform itemDisplayPoint;
    [SerializeField] private MeshRenderer spriteDisplay;
    [SerializeField] private Light spotLight;
    [SerializeField] private GameObject podium;
    [SerializeField] private TextMeshPro tierNumberText;

    [Header("Settings")]
    [SerializeField] private float modelRotationSpeed = 30f;
    [SerializeField] private float lightIntensityOn = 2f;
    [SerializeField] private float lightIntensityOff = 0f;
    [SerializeField] private float lightTransitionSpeed = 5f;
    [SerializeField] private float hoverScaleMultiplier = 1.1f;
    [SerializeField] private float scaleTransitionSpeed = 5f;

    private BattlePassItem currentItem;
    private GameObject spawnedModel;
    private GameObject modelWrapper;
    private bool isHovered = false;
    private float targetLightIntensity = 0f;
    private Vector3 originalScale = Vector3.one;
    private Vector3 targetScale = Vector3.one;

    private void Update()
    {
        // Smoothly transition light intensity
        if (spotLight != null)
        {
            spotLight.intensity = Mathf.Lerp(spotLight.intensity, targetLightIntensity, Time.deltaTime * lightTransitionSpeed);
        }

        // Rotate 3D model wrapper if present around its own pivot
        if (modelWrapper != null)
        {
            modelWrapper.transform.Rotate(0, modelRotationSpeed * Time.deltaTime, 0, Space.Self);
        }

        // Smoothly scale the entire display item
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, Time.deltaTime * scaleTransitionSpeed);
    }

    public void SetupItem(BattlePassItem item, int tierNumber)
    {
        currentItem = item;
        targetScale = Vector3.one;
        originalScale = Vector3.one;
        transform.localScale = originalScale;

        // Set name
        if (nameText != null)
        {
            nameText.text = item.itemName;
        }

        // Set tier number
        if (tierNumberText != null)
        {
            tierNumberText.text = tierNumber.ToString();
        }

        // Clear any existing display
        ClearDisplay();

        // Set up visual display
        if (item.modelPrefab != null)
        {
            // Create wrapper to ensure centered rotation
            modelWrapper = new GameObject("ModelWrapper");
            modelWrapper.transform.SetParent(itemDisplayPoint, false);
            modelWrapper.transform.localPosition = Vector3.zero;
            modelWrapper.transform.localRotation = Quaternion.Euler(item.rotationOffset);

            // Spawn 3D model inside wrapper
            spawnedModel = Instantiate(item.modelPrefab, modelWrapper.transform);
            spawnedModel.transform.localPosition = Vector3.zero;
            spawnedModel.transform.localRotation = Quaternion.identity;
            spawnedModel.transform.localScale = Vector3.one * item.displayScale;

            // Hide sprite display if using model
            if (spriteDisplay != null)
            {
                spriteDisplay.gameObject.SetActive(false);
            }
        }
        else if (item.itemSprite != null)
        {
            // Use sprite display with Quad
            if (spriteDisplay != null)
            {
                // Create a material with the sprite texture
                Material spriteMaterial = new Material(Shader.Find("Unlit/Transparent"));
                spriteMaterial.mainTexture = item.itemSprite.texture;
                spriteDisplay.material = spriteMaterial;

                spriteDisplay.gameObject.SetActive(true);
                spriteDisplay.transform.localScale = Vector3.one * item.displayScale;
            }
        }

        // Set up light
        if (spotLight != null)
        {
            spotLight.color = item.lightColor;
            spotLight.intensity = lightIntensityOff;
        }
    }

    private void ClearDisplay()
    {
        // Destroy existing model and wrapper
        if (spawnedModel != null)
        {
            Destroy(spawnedModel);
            spawnedModel = null;
        }

        if (modelWrapper != null)
        {
            Destroy(modelWrapper);
            modelWrapper = null;
        }

        // Clear sprite
        if (spriteDisplay != null)
        {
            spriteDisplay.gameObject.SetActive(false);

            // Clean up material
            if (spriteDisplay.material != null && Application.isPlaying)
            {
                Destroy(spriteDisplay.material);
            }
        }
    }

    private void OnMouseEnter()
    {
        isHovered = true;
        targetLightIntensity = lightIntensityOn;
        targetScale = originalScale * hoverScaleMultiplier;
    }

    private void OnMouseExit()
    {
        isHovered = false;
        targetLightIntensity = lightIntensityOff;
        targetScale = originalScale;
    }

    private void OnDestroy()
    {
        ClearDisplay();
    }
}
