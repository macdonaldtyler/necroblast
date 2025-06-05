using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphicsQualityManager : MonoBehaviour
{
    [SerializeField] private Slider graphicsQualitySlider;
    [SerializeField] private Text qualityLabel; // Optional UI label for feedback

    private void Start()
    {
        // Set slider range to match available quality levels
        graphicsQualitySlider.maxValue = QualitySettings.names.Length - 1;

        // Load saved quality level or default to Medium (index 2)
        int savedQuality = PlayerPrefs.GetInt("GraphicsQuality", 2);
        graphicsQualitySlider.value = savedQuality;
        SetGraphicsQuality(savedQuality);

        // Add listener to update quality when slider changes
        graphicsQualitySlider.onValueChanged.AddListener(delegate 
        { 
            SetGraphicsQuality(Mathf.RoundToInt(graphicsQualitySlider.value)); 
        });
    }

    public void SetGraphicsQuality(int level)
    {
        QualitySettings.SetQualityLevel(level);
        PlayerPrefs.SetInt("GraphicsQuality", level);
        PlayerPrefs.Save();

        // Update UI label if available
        if (qualityLabel != null)
        {
            qualityLabel.text = $"Quality: {QualitySettings.names[level]}";
        }

        Debug.Log($"Graphics Quality Set to: {QualitySettings.names[level]}");
    }
}
