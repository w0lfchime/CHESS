using System.Collections;
using UnityEngine;
using UnityEngine.UI;  // ADD THIS

public class pieceInfoMoveset : MonoBehaviour
{
    public static pieceInfoMoveset instance;
    public GameObject movesetGrid = null;
    private Image[] imageComponents;  // Cache the images once

    void Start()
    {
        instance = this;
        movesetGrid = this.gameObject;
        // Get all Image components from children ONCE at start
        imageComponents = this.GetComponentsInChildren<Image>();
    }

    public void disPic(Wrapper<Elements>[] vGrid)
    {
        StartCoroutine(setPicture(vGrid));
    }

    public IEnumerator setPicture(Wrapper<Elements>[] visualGrid)
    {
        yield return new WaitForEndOfFrame();
        int imgCount = 0;  // Initialize counter

        for (int i = 0; i < 11; i++)
        {
            for (int j = 0; j < 11; j++)
            {
                if (imgCount >= imageComponents.Length) break;  // Safety check

                Elements element = visualGrid[i].values[j];
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
                imageComponents[imgCount].color = baseColor * colorMult;
                imgCount++;  // INCREMENT the counter
            }
        }
    }
}