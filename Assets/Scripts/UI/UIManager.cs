using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates UI sliders for pride and prejudice based on GameManager values.
/// </summary>
public class UIManager : MonoBehaviour
{
    public Slider prideSlider;
    public Slider prejudiceSlider;

    private void Start()
    {
        // Initialize sliders to current GameManager values
        UpdateUI();
    }

    private void Update()
    {
        // Update UI every frame (could be optimized to update only when values change)
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (prideSlider != null)
            prideSlider.value = GameManager.Instance.pride;
        if (prejudiceSlider != null)
            prejudiceSlider.value = GameManager.Instance.prejudice;
    }
}