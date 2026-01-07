using UnityEngine;
using System.Collections.Generic;
using TMPro;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

[System.Serializable]
public class TutorialPiece
{
    public string text;
    public Transform point;
    public float timeAmount = -1;
}
public class Tutorial : MonoBehaviour
{
    public CanvasGroup uiGroup;
    public bool running = true;
    public List<TutorialPiece> tutorialPiece = new List<TutorialPiece>();
    public float speed;
    public TMP_Text text;
    private int step = 0;

    void Update()
    {
        if(!running) return;

        transform.position+=(tutorialPiece[step].point.position-transform.position)*Time.deltaTime*speed;
        transform.eulerAngles+=(tutorialPiece[step].point.eulerAngles-transform.eulerAngles)*Time.deltaTime*speed;
        transform.localScale+=(tutorialPiece[step].point.localScale-transform.localScale)*Time.deltaTime*speed;
        text.text = tutorialPiece[step].text;

        if(tutorialPiece[step].timeAmount!=-1) {
            uiGroup.interactable = false;
            tutorialPiece[step].timeAmount-=Time.deltaTime;
            if(tutorialPiece[step].timeAmount<=0) NextStep();
        }else uiGroup.interactable = true;
    }

    public void StartTutorial()
    {
        running = true;
    }

    public void NextStep()
    {
        if(tutorialPiece.Count-1 == step) running = false;
        if(!running) return;
        step++;
    }
}
