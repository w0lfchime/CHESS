using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class PawnathanAnimations : MonoBehaviour
{
    private Animation anim;
    public Vector2 randomTime;
    public Vector2Int randomCount;

    void Start()
    {
        anim = GetComponent<Animation>();
        StartCoroutine(loop());
    }

    void ForceNo()
    {
        anim.Play("no");
    }

    IEnumerator loop()
    {
        while (anim.isPlaying == true) yield return null;
        yield return new WaitForSeconds(Random.Range(randomTime.x, randomTime.y));

        List<AnimationState> animationStates = new List<AnimationState>();
        foreach (AnimationState state in anim)
        {
            animationStates.Add(state);
        }

        if (animationStates.Count == 0) yield return null;

        // Random.Range upper bound is exclusive
        AnimationState selectedAnimation = animationStates[Random.Range(0, animationStates.Count)];

        int count = Random.Range(randomCount.x, randomCount.y);
        for (int i = 0; i < count; i++){
            anim.Play(selectedAnimation.name);
            while (anim.isPlaying == true) yield return null;
        }

        StartCoroutine(loop());
    }


}
