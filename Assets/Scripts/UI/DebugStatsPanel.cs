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
    public GameObject npcContainer; // O painel (Container) onde os textos dos NPCs vão entrar
    public GameObject npcTextPrefab; // Um prefab de texto simples (UI Text)

    private void OnEnable()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.OnStatsChanged -= OnStatsChanged;
            GameManager.Instance.OnStatsChanged += OnStatsChanged;
            
            GameManager.Instance.OnRelationshipChanged -= OnRelationshipChanged;
            GameManager.Instance.OnRelationshipChanged += OnRelationshipChanged;
        }

        UpdateUI();
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
        UpdateUI();

        if (simulatePrideUpButton != null)
            simulatePrideUpButton.onClick.AddListener(() => SimulateChoice(10f, 0f)); 
        if (simulatePrejudiceUpButton != null)
            simulatePrejudiceUpButton.onClick.AddListener(() => SimulateChoice(0f, 10f));
            
        Invoke(nameof(UpdateUI), 0.1f);
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
        
        // 1. Atualiza os status da Elizabeth com segurança
        if (prideText != null) 
            prideText.text = GameManager.Instance.pride.ToString("0");
            
        if (prejudiceText != null) 
            prejudiceText.text = GameManager.Instance.prejudice.ToString("0");

        if (npcContainer == null || npcTextPrefab == null) return;

        // 2. LIMPEZA INTELIGENTE: Remove apenas clones antigos se eles não estiverem mais no save
        foreach (Transform child in npcContainer.transform)
        {
            if (child == null || child.gameObject == null) continue;

            // PROTEÇÃO ABSOLUTA: Nunca deixa deletar os componentes da Elizabeth por tabela
            if (prideText != null && child.gameObject == prideText.gameObject) continue;
            if (prejudiceText != null && child.gameObject == prejudiceText.gameObject) continue;

            // Se o dicionário resetou ou não contém esse NPC específico, remove da tela
            if (GameManager.Instance.relationships == null || !GameManager.Instance.relationships.ContainsKey(child.name))
            {
                Destroy(child.gameObject);
            }
        }

        // 3. ATUALIZAÇÃO SEM RECRIAÇÃO: Evita o glitch do Vertical Layout Group
        if (GameManager.Instance.relationships != null)
        {
            foreach (var kvp in GameManager.Instance.relationships)
            {
                string npcName = kvp.Key;
                float affinityValue = kvp.Value;
                string textoFormatado = npcName + ": " + affinityValue.ToString("0");

                // Tenta encontrar se já existe um objeto de texto com o nome desse NPC no container
                Transform textoExistente = npcContainer.transform.Find(npcName);

                if (textoExistente != null)
                {
                    // Se já existe, apenas atualiza o texto existente (Sem instanciar ou deletar nada!)
                    Text textComp = textoExistente.GetComponent<Text>();
                    if (textComp != null)
                    {
                        textComp.text = textoFormatado;
                    }
                }
                else
                {
                    // Se é um NPC novo, cria o texto e batiza o GameObject com o nome dele para futuras buscas
                    GameObject newTextObj = Instantiate(npcTextPrefab, npcContainer.transform);
                    if (newTextObj != null)
                    {
                        newTextObj.name = npcName; // Identificador usado pelo transform.Find()
                        Text textComp = newTextObj.GetComponent<Text>();
                        if (textComp != null)
                        {
                            textComp.text = textoFormatado;
                        }
                    }
                }
            }
        }
    }

    public void SimulateChoice(float prideDelta, float prejudiceDelta)
    {
        if (GameManager.Instance == null) return;
        
        ChoiceData c = new ChoiceData();
        c.prideChange = prideDelta;
        c.prejudiceChange = prejudiceDelta;
        GameManager.Instance.ApplyChoice(c);
    }
}