using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using UnityEditor.UIElements;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

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
    private PieceInfo currentPiece;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        allCollectionsPage.SetActive(true);
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

            pieceButton.onClick.AddListener(() => SetPiece(piece.name, singleCollectionPage));

            pieceButton.transform.SetParent(collectionPageManager.pieceButtonsHolder);

            pieceButton.transform.localScale = Vector3.one;

            pieceButton.gameObject.SetActive(true);
        }

        SetPiece(currentCollection.pieces[0].name, singleCollectionPage);

        collectionPageManager.colorToggle.onValueChanged.AddListener((value) => ChangePieceIconColors());

        collectionPageManager.backButton.onClick.AddListener(() => BackToCollections());

        singleCollectionPage.transform.SetParent(mainCanvas);

        singleCollectionPage.transform.localPosition = Vector3.zero;

        singleCollectionPage.transform.localScale = Vector3.one;

        singleCollectionPage.gameObject.SetActive(true);
    }

    public void SetPiece(string pieceName, GameObject singleCollectionPage)
    {
        foreach (Transform child in singleCollectionPage.transform)
        {
            if (child.GetComponent<PieceDisplayManager>() != null)
            {
                Destroy(child.gameObject);
            }
        }

        foreach (PieceInfo piece in currentCollection.pieces)
        {
            if (piece.name == pieceName)
            {
                currentPiece = piece;
            }
        }

        GameObject piecePage = Instantiate(pieceInfoDisplayPrefab);
        PieceDisplayManager pieceDisplayManager = piecePage.GetComponent<PieceDisplayManager>();

        pieceDisplayManager.titleText.text = currentPiece.name;
        pieceDisplayManager.taglineText.text = currentPiece.tagLine;
        pieceDisplayManager.descriptionText.text = currentPiece.tagLine;
        pieceDisplayManager.abilityNameText.text = currentPiece.abilityName;
        pieceDisplayManager.abilityNameDescription.text = currentPiece.abilityDescription;

        piecePage.transform.SetParent(singleCollectionPage.transform);

        piecePage.transform.localPosition = new Vector3(-475, -12.5f, 0f);
        piecePage.transform.localScale = Vector3.one;

        piecePage.gameObject.SetActive(true);
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

    public void BackToCollections()
    {
        foreach (Transform child in mainCanvas.transform)
        {
            if (child.GetComponent<CollectionPageManager>() != null)
            {
                Destroy(child.gameObject);
            }
        }
        allCollectionsPage.SetActive(true);
    }

    public void BackToMain()
    {
        SceneManager.LoadScene("Main Menu");
    }
}