using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public AudioMixer mixer;      
    public Slider slider;         
    public string parameter = "MasterVolume"; 

    void Start()
    {
        // Load saved volume OR default to current mixer setting
        if (PlayerPrefs.HasKey(parameter))
        {
            float savedVolume = PlayerPrefs.GetFloat(parameter);
            slider.value = savedVolume;
            SetVolume(savedVolume);
        }
        else
        {
            float db;
            if (mixer.GetFloat(parameter, out db))
                slider.value = Mathf.Pow(10f, db / 20f); // convert dB -> linear
        }

        slider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float v)  // slider value 0..1
    {
        // Save immediately on change
        PlayerPrefs.SetFloat(parameter, v);
        PlayerPrefs.Save();

        // Convert slider value -> dB
        if (v <= 0.0001f)
        {
            mixer.SetFloat(parameter, -80f); 
        }
        else
        {
            mixer.SetFloat(parameter, Mathf.Log10(v) * 20f);
        }
    }
}
