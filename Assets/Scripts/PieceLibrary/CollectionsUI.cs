using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UIElements;
using TMPro;
using System.Collections.Generic;

public class CollectionsUI : MonoBehaviour
{
    [SerializeField] public Button collectionButtonPrefab;
    [SerializeField] public Button pieceButtonPrefab;
    [SerializeField] public GameObject singleCollectionPagePrefab;
    [SerializeField] public GameObject pieceInfoDisplayPrefab;
    [SerializeField] public Transform buttonHolder;
    [SerializeField] public Transform mainCanvas;
    [SerializeField] public GameObject allCollectionsPage;
    [SerializeField] public PieceCollection[] collections;
    private PieceCollection currentCollection;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        allCollectionsPage.SetActive(true);
        collectionButtonPrefab.gameObject.SetActive(false);
        foreach (PieceCollection collection in collections)
        {
            Button collectionButton = Instantiate(collectionButtonPrefab);
            CollectionButtonManager buttonManager = collectionButton.GetComponent<CollectionButtonManager>();
            buttonManager.collectionTitleText.text = collection.name;

            for (int i = 0; i < buttonManager.icons.Length; i++)
            {
                buttonManager.icons[i].sprite = collection.icons[i];
            }
            collectionButton.transform.SetParent(buttonHolder);

            collectionButton.transform.localScale = Vector3.one;

            collectionButton.onClick.AddListener(() => SetCollection(collection.name));

            collectionButton.gameObject.SetActive(true);
        }
    }

    public void SetCollection(string collectionName)
    {
        foreach (PieceCollection collection in collections)
        {
            if (collection.name == collectionName)
            {
                currentCollection = collection;
            }
        }

        allCollectionsPage.SetActive(false);

        GameObject singleCollectionPage = Instantiate(singleCollectionPagePrefab);
        CollectionPageManager collectionPageManager = singleCollectionPage.GetComponent<CollectionPageManager>();

        collectionPageManager.collectionTitle.text = currentCollection.name;

        foreach (PieceInfo piece in currentCollection.pieces)
        {
            Button pieceButton = Instantiate(pieceButtonPrefab);
            PieceButtonManager pieceButtonManager = pieceButton.GetComponent<PieceButtonManager>();

            pieceButtonManager.pieceInfo = piece;

            pieceButtonManager.icon.sprite = piece.whiteIcon;

            pieceButton.onClick.AddListener(() => SetPiece(piece.name));

            pieceButton.transform.SetParent(collectionPageManager.pieceButtonsHolder);

            pieceButton.transform.localScale = Vector3.one;

            pieceButton.gameObject.SetActive(true);
        }

        collectionPageManager.colorToggle.onValueChanged.AddListener((value) => ChangePieceIconColors());

        singleCollectionPage.transform.SetParent(mainCanvas);

        singleCollectionPage.transform.localPosition = Vector3.zero;

        singleCollectionPage.transform.localScale = Vector3.one;

        singleCollectionPage.gameObject.SetActive(true);
    }

    public void SetPiece(string pieceName)
    {
        Debug.Log(pieceName);
    }

    public void ChangePieceIconColors()
    {
        CollectionPageManager currentCPM = mainCanvas.GetComponentInChildren<CollectionPageManager>();
        foreach (Button pieceButton in currentCPM.pieceButtonsHolder.GetComponentsInChildren<Button>())
        {
            PieceButtonManager pieceButtonManager = pieceButton.GetComponent<PieceButtonManager>();
            if (pieceButtonManager.icon.sprite == pieceButtonManager.pieceInfo.whiteIcon)
            {
                pieceButtonManager.icon.sprite = pieceButtonManager.pieceInfo.blackIcon;
            }
            else
            {
                pieceButtonManager.icon.sprite = pieceButtonManager.pieceInfo.whiteIcon;
            }
        }
    }
}