using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Updates UI sliders for pride, prejudice, and NPC relationships based on GameManager values.
/// </summary>
public class UIManager : MonoBehaviour
{
    [Header("Status Globais")]
    public Slider prideSlider;
    public Slider prejudiceSlider;

    [Header("Sistema de Relacionamento Dinâmico")]
    public GameObject npcStatusPrefab; // Prefab contendo um Text (Nome) e um Slider (Afinidade)
    public Transform container;        // Painel com um componente Vertical Layout Group

    // Dicionário local para o UIManager lembrar quais barrinhas de NPCs ele já criou na tela
    private Dictionary<string, Slider> npcSliders = new Dictionary<string, Slider>();

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
        if (GameManager.Instance != null)
        {
            // 1. Atualiza as barras globais que você já tinha
            if (prideSlider != null)
                prideSlider.value = GameManager.Instance.pride;
            if (prejudiceSlider != null)
                prejudiceSlider.value = GameManager.Instance.prejudice;

            // 2. 🔥 ATUALIZA OU CRIA AS BARRAS DE RELACIONAMENTO DOS NPCs:
            if (GameManager.Instance.relationships != null)
            {
                foreach (var kvp in GameManager.Instance.relationships)
                {
                    string npcName = kvp.Key;
                    float relationshipValue = kvp.Value;

                    // Se a barrinha desse NPC já foi criada antes, só atualiza o valor dela
                    if (npcSliders.ContainsKey(npcName))
                    {
                        if (npcSliders[npcName] != null)
                        {
                            npcSliders[npcName].value = relationshipValue;
                        }
                    }
                    else
                    {
                        // Se o NPC apareceu no dicionário agora, cria a barrinha dele dinamicamente!
                        CriarNovaBarraNPC(npcName, relationshipValue);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Cria visualmente um novo item de relacionamento na UI e o registra no dicionário.
    /// </summary>
    private void CriarNovaBarraNPC(string nome, float valorInicial)
    {
        if (npcStatusPrefab == null || container == null) return;

        // Instancia o prefab dentro do container (Vertical Layout Group)
        GameObject novoItem = Instantiate(npcStatusPrefab, container);
        
        // Procura pelos componentes de Texto e Slider dentro do Prefab que acabou de nascer
        Text textoNome = novoItem.GetComponentInChildren<Text>();
        Slider sliderAfinidade = novoItem.GetComponentInChildren<Slider>();

        if (textoNome != null)
        {
            textoNome.text = nome;
        }

        if (sliderAfinidade != null)
        {
            sliderAfinidade.maxValue = 100f; // Garante que o limite bate com o GameManager
            sliderAfinidade.value = valorInicial;
            
            // Salva a referência do slider associada ao nome do NPC para os próximos frames atualizarem direto
            npcSliders[nome] = sliderAfinidade;
        }
    }
}