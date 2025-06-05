using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private Slider volumeSlider;

    private void Start()
    {
        // Set the initial value of the slider to the current volume level
        volumeSlider.value = AudioListener.volume;

        // Add a listener to detect slider changes
        volumeSlider.onValueChanged.AddListener(SetVolume);
    }

    private void SetVolume(float volume)
    {
        // Set the AudioListener volume to the value of the slider
        AudioListener.volume = volume;
    }
}