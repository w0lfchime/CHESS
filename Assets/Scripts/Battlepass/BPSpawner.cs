using UnityEngine;
using System.Collections.Generic;

public class BPSpawner : MonoBehaviour
{
    [Header("Battle Pass Items")]
    [Tooltip("List of all battle pass items in order")]
    [SerializeField] private List<BattlePassItem> battlePassItems = new List<BattlePassItem>();

    [Header("Display Settings")]
    [Tooltip("Prefab for displaying individual battle pass items")]
    [SerializeField] private GameObject itemDisplayPrefab;

    [Tooltip("Parent transform where items will be spawned")]
    [SerializeField] private Transform itemDisplayParent;

    [Tooltip("Maximum number of items to show at once")]
    [SerializeField] private int maxItemsPerPage = 5;

    [Tooltip("Spacing between items")]
    [SerializeField] private float itemSpacing = 2f;

    [Header("Navigation")]
    [SerializeField] private GameObject previousButton;
    [SerializeField] private GameObject nextButton;

    private List<GameObject> currentDisplayedItems = new List<GameObject>();
    private int currentPageIndex = 0;
    private int totalPages = 0;

    private void Start()
    {
        CalculateTotalPages();
        DisplayCurrentPage();
        UpdateNavigationButtons();
    }

    private void CalculateTotalPages()
    {
        if (battlePassItems.Count == 0)
        {
            totalPages = 0;
            return;
        }

        totalPages = Mathf.CeilToInt((float)battlePassItems.Count / maxItemsPerPage);
    }

    private void DisplayCurrentPage()
    {
        // Clear existing items
        ClearDisplayedItems();

        // Calculate start and end indices for current page
        int startIndex = currentPageIndex * maxItemsPerPage;
        int endIndex = Mathf.Min(startIndex + maxItemsPerPage, battlePassItems.Count);

        // Spawn items for current page
        for (int i = startIndex; i < endIndex; i++)
        {
            GameObject itemDisplay = Instantiate(itemDisplayPrefab, itemDisplayParent);

            // Position the item
            float xPosition = (i - startIndex) * itemSpacing;
            itemDisplay.transform.localPosition = new Vector3(xPosition, 0, 0);

            // Setup the item display
            BattlePassItemDisplay displayComponent = itemDisplay.GetComponent<BattlePassItemDisplay>();
            if (displayComponent != null)
            {
                displayComponent.SetupItem(battlePassItems[i], i + 1); // +1 for 1-based tier numbering
            }

            currentDisplayedItems.Add(itemDisplay);
        }

        // Center the display parent if needed
        CenterDisplay(endIndex - startIndex);
    }

    private void CenterDisplay(int itemCount)
    {
        if (itemDisplayParent == null) return;

        // Calculate offset to center items
        float totalWidth = (itemCount - 1) * itemSpacing;
        float offset = -totalWidth / 2f;

        Vector3 currentPos = itemDisplayParent.localPosition;
        itemDisplayParent.localPosition = new Vector3(offset, currentPos.y, currentPos.z);
    }

    private void ClearDisplayedItems()
    {
        foreach (GameObject item in currentDisplayedItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        currentDisplayedItems.Clear();
    }

    private void UpdateNavigationButtons()
    {
        // Enable/disable navigation buttons based on current page
        if (previousButton != null)
        {
            previousButton.SetActive(currentPageIndex > 0);
        }

        if (nextButton != null)
        {
            nextButton.SetActive(currentPageIndex < totalPages - 1);
        }
    }

    public void NextPage()
    {
        if (currentPageIndex < totalPages - 1)
        {
            currentPageIndex++;
            DisplayCurrentPage();
            UpdateNavigationButtons();
        }
    }

    public void PreviousPage()
    {
        if (currentPageIndex > 0)
        {
            currentPageIndex--;
            DisplayCurrentPage();
            UpdateNavigationButtons();
        }
    }

    // Call this if you add/remove items from the battle pass at runtime
    public void RefreshDisplay()
    {
        CalculateTotalPages();

        // Clamp current page to valid range
        currentPageIndex = Mathf.Clamp(currentPageIndex, 0, Mathf.Max(0, totalPages - 1));

        DisplayCurrentPage();
        UpdateNavigationButtons();
    }
}
