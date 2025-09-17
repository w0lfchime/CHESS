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
    [SerializeField] public Transform buttonHolder;
    [SerializeField] public PieceCollection[] collections;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttonPrefab.gameObject.SetActive(false);
        foreach (PieceCollection collection in collections)
        {
            Button collectionButton = Instantiate(buttonPrefab);
            CollectionButtonManager buttonManager = collectionButton.GetComponent<CollectionButtonManager>();
            buttonManager.collectionTitleText.text = collection.name;

            for (int i = 0; i < buttonManager.icons.Length; i++)
            {
                buttonManager.icons[i].sprite = collection.icons[i];
            }
            collectionButton.transform.SetParent(buttonPrefab.transform.parent);

            collectionButton.transform.localScale = new Vector3(1, 1, 1);

            collectionButton.gameObject.SetActive(true);
        }
    }
}