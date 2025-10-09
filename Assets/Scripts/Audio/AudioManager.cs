using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager instance;

    [Header("Audio Clips")]
    public AudioClip[] placementSounds;
    public AudioClip[] takeSounds;

    [Header("Audio Source")]
    public AudioSource audioSource;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void PlayPlacementSound()
    {
        if (placementSounds.Length == 0 || audioSource == null) return;
        int randomIndex = Random.Range(0, placementSounds.Length);
        audioSource.PlayOneShot(placementSounds[randomIndex]);
    }

    public void PlayTakeSound()
    {
        if (takeSounds.Length == 0 || audioSource == null) return;
        int randomIndex = Random.Range(0, takeSounds.Length);
        audioSource.PlayOneShot(takeSounds[randomIndex]);
    }
}
