using UnityEngine;

public class AudioManagerScript : MonoBehaviour
{
    public AudioSource audioSource;

    // AUDIO CLIP RANGES
    // 0-2: basic movement sounds
    // 3: amazon
    // 4: gun
    // 5-8: horse
    // 9-11: kiss
    // 12: nuke
    public AudioClip[] soundEffectList;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void playSoundEffect(int index)
    {
        audioSource.clip = soundEffectList[index];
        audioSource.Play();
    }
}
