using UnityEngine;

public class pieceInfoPicture : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public static pieceInfoPicture instance;
    void Start()
    {
        instance = this;
    }

    public void setPicture(Sprite pic)
    {
        GetComponent<UnityEngine.UI.Image>().sprite = pic;
    }
}
