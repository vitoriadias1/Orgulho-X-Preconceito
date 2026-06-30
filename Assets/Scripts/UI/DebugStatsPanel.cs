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

    private void OnEnable()
    {
        // 🔥 Se inscreve nos eventos ANTES do Start para garantir que não perderá o sinal de Load do SaveSystem
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStatsChanged += OnStatsChanged;
            GameManager.Instance.OnRelationshipChanged += OnRelationshipChanged;
        }
    }

    private void OnDisable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStatsChanged -= OnStatsChanged;
            GameManager.Instance.OnRelationshipChanged -= OnRelationshipChanged;
        }
    }

    private void Start()
    {
        // Força uma atualização inicial
        UpdateUI();

        if (simulatePrideUpButton != null)
            simulatePrideUpButton.onClick.AddListener(() => SimulateChoice(0.1f, 0f));
        if (simulatePrejudiceUpButton != null)
            simulatePrejudiceUpButton.onClick.AddListener(() => SimulateChoice(0f, 0.1f));
            
        // 🔥 Garante uma atualização com delay mínimo para sincronizar perfeitamente com o Start do GameManager
        Invoke(nameof(UpdateUI), 0.05f);
    }

    private void OnStatsChanged(float pride, float prejudice)
    {
        UpdateUI();
    }

    private void OnRelationshipChanged(string name, float value)
    {
        UpdateUI();
    }

    public void UpdateUI()
    {
        if (GameManager.Instance == null) return;
        
        if (prideText != null) 
            prideText.text = "Orgulho: " + GameManager.Instance.pride.ToString("0");
            
        if (prejudiceText != null) 
            prejudiceText.text = "Preconceito: " + GameManager.Instance.prejudice.ToString("0");
    }

    public void SimulateChoice(float prideDelta, float prejudiceDelta)
    {
        ChoiceData c = new ChoiceData();
        c.prideChange = prideDelta;
        c.prejudiceChange = prejudiceDelta;
        GameManager.Instance.ApplyChoice(c);
    }
}