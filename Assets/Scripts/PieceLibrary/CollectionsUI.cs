using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
//using UnityEditor.UIElements;
using TMPro;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Collections;
using System;

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
    private string currentCollection;
    private ChessPieceData currentPiece;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        allCollectionsPage.SetActive(true);
        foreach (string collection in Enum.GetNames(typeof(Collections)))
        {
            Button collectionButton = Instantiate(collectionButtonPrefab);
            CollectionButtonManager buttonManager = collectionButton.GetComponent<CollectionButtonManager>();
            buttonManager.collectionTitleText.text = collection;

            for (int i = 0; i < buttonManager.icons.Length; i++)
            {
                int count = 0;
                foreach (ChessPieceData piece in PieceLibrary.Instance.GetAllData())
                {
                    if (piece.collection.ToString() == collection)
                    {
                        count++;
                        buttonManager.icons[i].sprite = piece.image;
                        if(i >= count) break;
                    }
                }
            }
            collectionButton.transform.SetParent(buttonHolder);

            collectionButton.transform.localScale = Vector3.one;
            collectionButton.transform.localPosition = new Vector3(collectionButton.transform.localPosition.x, collectionButton.transform.localPosition.y, 0f);

            collectionButton.onClick.AddListener(() => SetCollection(collection));

            collectionButton.gameObject.SetActive(true);
        }
    }

    public void SetCollection(string collectionName)
    {
        foreach (string collection in Enum.GetNames(typeof(Collections)))
        {
            if (collection == collectionName)
            {
                currentCollection = collection;
            }
        }

        allCollectionsPage.SetActive(false);

        GameObject singleCollectionPage = Instantiate(singleCollectionPagePrefab);
        CollectionPageManager collectionPageManager = singleCollectionPage.GetComponent<CollectionPageManager>();

        collectionPageManager.collectionTitle.text = currentCollection;

        collectionPageManager.currentColor = collectionPageManager.colorToggle.isOn ? "BLACK" : "WHITE";

        ChessPieceData firstPiece = null;

        foreach (ChessPieceData piece in PieceLibrary.Instance.GetAllData())
        {
            if(collectionName != piece.collection.ToString()) continue;
            if(firstPiece==null) firstPiece = piece;
            Button pieceButton = Instantiate(pieceButtonPrefab);
            PieceButtonManager pieceButtonManager = pieceButton.GetComponent<PieceButtonManager>();

            pieceButtonManager.icon.sprite = collectionPageManager.currentColor == "BLACK" ? piece.image : piece.image;

            pieceButton.onClick.AddListener(() => SetPiece(piece.pieceName, singleCollectionPage));

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

        SetPiece(firstPiece.pieceName, singleCollectionPage);
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

        currentPiece = PieceLibrary.Instance.GetPrefab(pieceName);

        GameObject piecePage = Instantiate(pieceInfoDisplayPrefab);
        PieceDisplayManager pieceDisplayManager = piecePage.GetComponent<PieceDisplayManager>();
        pieceDisplayManager.currentAbility = 0;

        pieceDisplayManager.titleText.text = currentPiece.pieceName;
        pieceDisplayManager.taglineText.text = currentPiece.tagline;
        pieceDisplayManager.materialValueText.text = "+" + currentPiece.materialValue.ToString();
        pieceDisplayManager.descriptionText.text = currentPiece.description;
        pieceDisplayManager.abilityNameText.text = currentPiece.abilities[pieceDisplayManager.currentAbility].name;

        if (pieceDisplayManager.currentAbility >= currentPiece.abilities.Count - 1)
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
            if (pieceDisplayManager.currentAbility < currentPiece.abilities.Count)
            {
                pieceDisplayManager.nextAbilityButton.interactable = true;
            }
            pieceDisplayManager.abilityNameText.text = currentPiece.abilities[pieceDisplayManager.currentAbility].name;
            StartCoroutine(ChangeMoveDiagram());
        });

        pieceDisplayManager.nextAbilityButton.onClick.AddListener(() =>
        {
            pieceDisplayManager.currentAbility++;
            if (pieceDisplayManager.currentAbility >= currentPiece.abilities.Count - 1)
            {
                pieceDisplayManager.nextAbilityButton.interactable = false;
            }
            if (pieceDisplayManager.currentAbility > 0)
            {
                pieceDisplayManager.previousAbilityButton.interactable = true;
            }
            pieceDisplayManager.abilityNameText.text = currentPiece.abilities[pieceDisplayManager.currentAbility].name;
            StartCoroutine(ChangeMoveDiagram());
        });

        GameObject newMeshObject = new GameObject("PieceModel");
        MeshFilter meshFilter = newMeshObject.AddComponent<MeshFilter>();
        MeshRenderer meshRenderer = newMeshObject.AddComponent<MeshRenderer>();

        bool isModelBlack = singleCollectionPage.GetComponent<CollectionPageManager>().currentColor == "BLACK";
        meshFilter.mesh = currentPiece.model;
        newMeshObject.GetComponent<MeshFilter>().mesh = currentPiece.model;
        GameObject pieceModelHolder = pieceDisplayManager.pieceModelHolder;

        newMeshObject.transform.SetParent(pieceDisplayManager.pieceModelHolder.transform);
        newMeshObject.transform.localPosition = Vector3.zero;
        newMeshObject.transform.localEulerAngles = new Vector3(-90f, 0f ,0f);
        newMeshObject.transform.localScale *= 2000f;
        newMeshObject.layer = 5;

        pieceModelHolder.AddComponent<Animator>();
        pieceModelHolder.GetComponent<Animator>().runtimeAnimatorController = pieceAnimator;

        Material[] matList = currentPiece.whiteMaterialList.ToArray();
        matList[0] = whitePieceMaterial;
        newMeshObject.GetComponent<MeshRenderer>().materials = matList;
        newMeshObject.gameObject.layer = LayerMask.NameToLayer("BlackOutline");

        newMeshObject.SetActive(true);

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
                Elements element = currentPiece.abilities[currentPDM.currentAbility].actions[0].visualGrid[i].values[j];
                Color baseColor;

                if ((i + j) % 2 == 0)
                {
                    baseColor = new Color(0.255f, 0.255f, 0.255f, 1.000f);
                }
                else
                {
                    baseColor = new Color(0.294f, 0.294f, 0.294f, 1.000f);
                }

                switch (element)
                {
                    case Elements.CanMove:
                        baseColor = baseColor;
                        break;
                    case Elements.CantMove:
                        baseColor = Color.green*3 * baseColor;
                        break;
                    case (Elements)2:
                        baseColor = Color.yellow*3 * baseColor;
                        break;
                    default:
                        baseColor = Color.red*3 * baseColor;
                        break;
                }

                currentPDM.pieceMoveDiagram.transform.GetComponentsInChildren<Image>()[imgCount].color = baseColor;

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
                pieceButtonManager.icon.sprite = pieceButtonManager.pieceInfo.pieceData.image;
            }
            else
            {
                pieceButtonManager.icon.sprite = pieceButtonManager.pieceInfo.pieceData.image;
            }
        }

        PieceDisplayManager currentPDM = currentCPM.transform.GetComponentInChildren<PieceDisplayManager>();

        if (currentCPM.currentColor == "BLACK")
        {
            Material[] matList = currentPiece.whiteMaterialList.ToArray();
            matList[0] = blackPieceMaterial;
            currentPDM.pieceModelHolder.transform.GetChild(0).GetComponent<MeshRenderer>().materials = matList;
            currentPDM.pieceModelHolder.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("WhiteOutline");
        }
        else
        {
            Material[] matList = currentPiece.whiteMaterialList.ToArray();
            matList[0] = whitePieceMaterial;
            currentPDM.pieceModelHolder.transform.GetChild(0).GetComponent<MeshRenderer>().materials = matList;
            currentPDM.pieceModelHolder.transform.GetChild(0).gameObject.layer = LayerMask.NameToLayer("BlackOutline");
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