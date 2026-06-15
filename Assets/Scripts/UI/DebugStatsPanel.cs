using System;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Small debug panel to show pride/prejudice and simulate choices during development.
/// Connect UI in the inspector (Texts and optional buttons).
/// </summary>
public class DebugStatsPanel : MonoBehaviour
{
    public Text prideText;
    public Text prejudiceText;

    public Button simulatePrideUpButton;
    public Button simulatePrejudiceUpButton;

    private void Start()
    {
        UpdateUI();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStatsChanged += OnStatsChanged;
            GameManager.Instance.OnRelationshipChanged += OnRelationshipChanged;
        }

        if (simulatePrideUpButton != null)
            simulatePrideUpButton.onClick.AddListener(() => SimulateChoice(0.1f, 0f));
        if (simulatePrejudiceUpButton != null)
            simulatePrejudiceUpButton.onClick.AddListener(() => SimulateChoice(0f, 0.1f));
    }

    private void OnDestroy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStatsChanged -= OnStatsChanged;
            GameManager.Instance.OnRelationshipChanged -= OnRelationshipChanged;
        }
    }

    private void OnStatsChanged(float pride, float prejudice)
    {
        UpdateUI();
    }

    private void OnRelationshipChanged(string name, float value)
    {
        // For debug we simply refresh UI; could display a list instead
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (GameManager.Instance == null) return;
        if (prideText != null) prideText.text = "Pride: " + GameManager.Instance.pride.ToString("0");
        if (prejudiceText != null) prejudiceText.text = "Prejudice: " + GameManager.Instance.prejudice.ToString("0");
    }

    public void SimulateChoice(float prideDelta, float prejudiceDelta)
    {
        ChoiceData c = new ChoiceData();
        c.prideChange = prideDelta;
        c.prejudiceChange = prejudiceDelta;
        GameManager.Instance.ApplyChoice(c);
    }
}