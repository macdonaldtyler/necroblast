using UnityEngine;
using UnityEngine.UI;

public class BrightnessController : MonoBehaviour
{
    public Light sceneLight;
    public Slider brightnessSlider;

    void Start()
    {
        // Make sure the slider value is initialized within the expected range
        brightnessSlider.value = sceneLight.intensity;
        brightnessSlider.onValueChanged.AddListener(UpdateBrightness);
    }

    void UpdateBrightness(float value)
    {
        sceneLight.intensity = value; // Adjust light intensity based on slider value
    }
}
