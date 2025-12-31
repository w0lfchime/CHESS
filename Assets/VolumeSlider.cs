using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public AudioMixer mixer;      // assign in Inspector
    public Slider slider;         // assign in Inspector
    public string parameter = "MasterVolume"; // mixer exposed param

    void Start()
    {
        // set slider from mixer if needed
        float db;
        if (mixer.GetFloat(parameter, out db))
            slider.value = Mathf.Pow(10f, db / 20f);

        slider.onValueChanged.AddListener(SetVolume);
    }

    public void SetVolume(float v)  // slider value 0..1
    {
        if (v <= 0.0001f)
            mixer.SetFloat(parameter, -80f);   // effectively mute
        else
            mixer.SetFloat(parameter, Mathf.Log10(v) * 20f);
    }
}
