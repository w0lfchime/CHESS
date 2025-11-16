using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
//using UnityEditor.UIElements;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;

public class CollectionsUI : MonoBehaviour
{
    [SerializeField] public Button collectionButtonPrefab;
    [SerializeField] public Button pieceButtonPrefab;
    [SerializeField] public GameObject singleCollectionPagePrefab;
    [SerializeField] public GameObject pieceInfoDisplayPrefab;
    [SerializeField] public Transform buttonHolder;
    [SerializeField] public Transform parent;
    [SerializeField] public GameObject allCollectionsPage;
    [SerializeField] public RuntimeAnimatorController pieceAnimator;
    [SerializeField] public PieceCollection[] collections;
    [SerializeField] public Material whitePieceMaterial;
    [SerializeField] public Material blackPieceMaterial;
    private PieceCollection currentCollection;
    private PieceDisplayInfo currentPiece;

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
            collectionButton.transform.localPosition = new Vector3(collectionButton.transform.localPosition.x, collectionButton.transform.localPosition.y, 0f);

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

        collectionPageManager.currentColor = collectionPageManager.colorToggle.isOn ? "BLACK" : "WHITE";

        foreach (PieceDisplayInfo piece in currentCollection.pieces)
        {
            Button pieceButton = Instantiate(pieceButtonPrefab);
            PieceButtonManager pieceButtonManager = pieceButton.GetComponent<PieceButtonManager>();

            pieceButtonManager.pieceInfo = piece;

            pieceButtonManager.icon.sprite = collectionPageManager.currentColor == "BLACK" ? piece.blackIcon : piece.whiteIcon;

            pieceButton.onClick.AddListener(() => SetPiece(piece.name, singleCollectionPage));

            pieceButton.transform.SetParent(collectionPageManager.pieceButtonsHolder);

            pieceButton.transform.localScale = Vector3.one;

            pieceButton.gameObject.SetActive(true);
        }

        collectionPageManager.colorToggle.onValueChanged.AddListener((value) => ChangePieceColors());

        collectionPageManager.backButton.onClick.AddListener(() => BackToCollections());

        singleCollectionPage.transform.SetParent(parent);

        singleCollectionPage.transform.localPosition = Vector3.zero;

        singleCollectionPage.transform.localScale = Vector3.one;

        singleCollectionPage.gameObject.SetActive(true);

        SetPiece(currentCollection.pieces[0].name, singleCollectionPage);
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

        foreach (PieceDisplayInfo piece in currentCollection.pieces)
        {
            if (piece.name == pieceName)
            {
                currentPiece = piece;
            }
        }

        GameObject piecePage = Instantiate(pieceInfoDisplayPrefab);
        PieceDisplayManager pieceDisplayManager = piecePage.GetComponent<PieceDisplayManager>();
        pieceDisplayManager.currentAbility = 0;

        pieceDisplayManager.titleText.text = currentPiece.name;
        pieceDisplayManager.taglineText.text = currentPiece.tagLine;
        pieceDisplayManager.materialValueText.text = "+" + currentPiece.materialValue.ToString();
        pieceDisplayManager.descriptionText.text = currentPiece.description;
        pieceDisplayManager.abilityNameText.text = currentPiece.pieceData.abilities[pieceDisplayManager.currentAbility].name;

        if (pieceDisplayManager.currentAbility >= currentPiece.pieceData.abilities.Count - 1)
        {
            pieceDisplayManager.nextAbilityButton.interactable = false;
        }
        if (pieceDisplayManager.currentAbility <= 0)
        {
            pieceDisplayManager.previousAbilityButton.interactable = false;
        }

        pieceDisplayManager.previousAbilityButton.onClick.AddListener(() =>
        {
            pieceDisplayManager.currentAbility--;
            if (pieceDisplayManager.currentAbility <= 0)
            {
                pieceDisplayManager.previousAbilityButton.interactable = false;
            }
            if (pieceDisplayManager.currentAbility < currentPiece.pieceData.abilities.Count)
            {
                pieceDisplayManager.nextAbilityButton.interactable = true;
            }
            pieceDisplayManager.abilityNameText.text = currentPiece.pieceData.abilities[pieceDisplayManager.currentAbility].name;
            StartCoroutine(ChangeMoveDiagram());
        });

        pieceDisplayManager.nextAbilityButton.onClick.AddListener(() =>
        {
            pieceDisplayManager.currentAbility++;
            if (pieceDisplayManager.currentAbility >= currentPiece.pieceData.abilities.Count - 1)
            {
                pieceDisplayManager.nextAbilityButton.interactable = false;
            }
            if (pieceDisplayManager.currentAbility > 0)
            {
                pieceDisplayManager.previousAbilityButton.interactable = true;
            }
            pieceDisplayManager.abilityNameText.text = currentPiece.pieceData.abilities[pieceDisplayManager.currentAbility].name;
            StartCoroutine(ChangeMoveDiagram());
        });

        bool isModelBlack = singleCollectionPage.GetComponent<CollectionPageManager>().currentColor == "BLACK";
        GameObject pieceModel = Instantiate(currentPiece.pieceObject);
        GameObject pieceModelHolder = pieceDisplayManager.pieceModelHolder;

        pieceModel.transform.SetParent(pieceDisplayManager.pieceModelHolder.transform);
        pieceModel.transform.localPosition = Vector3.zero;
        pieceModel.transform.localEulerAngles = new Vector3(-90f, 0f ,0f);
        pieceModel.transform.localScale *= 30f;
        pieceModel.layer = 5;

        pieceModelHolder.AddComponent<Animator>();
        pieceModelHolder.transform.localPosition = new Vector3(-90f, 15f, -100f);
        pieceModelHolder.GetComponent<Animator>().runtimeAnimatorController = pieceAnimator;

        pieceModel.GetComponent<MeshRenderer>().material = isModelBlack ? blackPieceMaterial : whitePieceMaterial;

        pieceModel.SetActive(true);

        piecePage.transform.SetParent(singleCollectionPage.transform);

        piecePage.transform.localPosition = new Vector3(-190, -5f, 0f);
        piecePage.transform.localScale = Vector3.one;

        piecePage.gameObject.SetActive(true);

        StartCoroutine(ChangeMoveDiagram());
    }

    private IEnumerator ChangeMoveDiagram()
    {
        yield return new WaitForEndOfFrame();
        CollectionPageManager currentCPM = parent.GetComponentInChildren<CollectionPageManager>();
        PieceDisplayManager currentPDM = currentCPM.transform.GetComponentInChildren<PieceDisplayManager>();

        int imgCount = 0;
        for (int i = 0; i < 11; i++)
        {
            for (int j = 0; j < 11; j++)
            {
                Elements element = currentPiece.pieceData.abilities[currentPDM.currentAbility].actions[0].visualGrid[i].values[j];
                Color baseColor;
                switch (element)
                {
                    case Elements.CanMove:
                        baseColor = Color.black;
                        break;
                    case Elements.CantMove:
                        baseColor = Color.green;
                        break;
                    case (Elements)2:
                        baseColor = Color.yellow;
                        break;
                    default:
                        baseColor = Color.red;
                        break;
                }

                bool darkTile = (i + j) % 2 == 0;
                float colorMult = darkTile ? 0.85f : 0.95f;
                currentPDM.pieceMoveDiagram.transform.GetComponentsInChildren<Image>()[imgCount].color = baseColor * colorMult;

                imgCount++;
            }
        }
    }

    public void ChangePieceColors()
    {
        CollectionPageManager currentCPM = parent.GetComponentInChildren<CollectionPageManager>();
        if (currentCPM.currentColor == "BLACK")
        {
            currentCPM.currentColor = "WHITE";
        }
        else
        {
            currentCPM.currentColor = "BLACK";
        }

        foreach (Button pieceButton in currentCPM.pieceButtonsHolder.GetComponentsInChildren<Button>())
        {
            PieceButtonManager pieceButtonManager = pieceButton.GetComponent<PieceButtonManager>();
            if (currentCPM.currentColor == "BLACK")
            {
                pieceButtonManager.icon.sprite = pieceButtonManager.pieceInfo.blackIcon;
            }
            else
            {
                pieceButtonManager.icon.sprite = pieceButtonManager.pieceInfo.whiteIcon;
            }
        }

        PieceDisplayManager currentPDM = currentCPM.transform.GetComponentInChildren<PieceDisplayManager>();

        if (currentCPM.currentColor == "BLACK")
        {
            currentPDM.pieceModelHolder.transform.GetChild(0).GetComponent<MeshRenderer>().material = blackPieceMaterial;
        }
        else
        {
            currentPDM.pieceModelHolder.transform.GetChild(0).GetComponent<MeshRenderer>().material = whitePieceMaterial;
        }
    }

    public void BackToCollections()
    {
        foreach (Transform child in parent.transform)
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