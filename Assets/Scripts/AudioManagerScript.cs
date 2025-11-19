using UnityEngine;

public class AudioManagerScript : MonoBehaviour
{
    public AudioSource audioSource;
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
