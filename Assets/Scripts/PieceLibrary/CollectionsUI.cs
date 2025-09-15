using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UIElements;
using TMPro;
using System.Collections.Generic;

public class CollectionsUI : MonoBehaviour
{
    [SerializeField] public Button buttonPrefab;
    [SerializeField] public PieceCollection[] collections;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonPrefab.gameObject.SetActive(false);
        foreach (PieceCollection collection in collections)
        {
            Button collectionButton = Instantiate(buttonPrefab);
            collectionButton.GetComponentInChildren<TextMeshProUGUI>().text = collection.name;
            List<Image> collectionIcons = new();

            foreach (Transform child in collectionButton.transform)
            {
                if (child.GetComponent<Image>())
                {
                    collectionIcons.Add(child.GetComponent<Image>());
                }
            }

            for (int i = 0; i < collectionIcons.Count; i++)
            {
                collectionIcons[i].sprite = collection.icons[i];
            }
            collectionButton.transform.SetParent(transform);

            collectionButton.transform.localScale = new Vector3(1, 1, 1);

            collectionButton.gameObject.SetActive(true);
        }
    }
}