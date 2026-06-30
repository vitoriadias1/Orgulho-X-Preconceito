using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton GameManager to hold game state: pride, prejudice, and relationships.
/// Provides events and persistence hooks for UI and other systems.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Status Globais (Escala 0 a 100)")]
    [Range(0f, 100f)]
    public float pride = 50f;
    [Range(0f, 100f)]
    public float prejudice = 50f;

    [Header("Progresso do Diálogo")]
    public int savedDialogueIndex = 0;

    // Relationships: NPC name -> relationship value (0-100)
    public Dictionary<string, float> relationships = new Dictionary<string, float>();

    // Events for UI / other systems to subscribe
    public Action<float, float> OnStatsChanged;
    public Action<string, float> OnRelationshipChanged;

    /// <summary>
    /// Reseta todos os status para o padrão de um novo jogo.
    /// </summary>
    public void ResetarParaNovoJogo()
    {
        pride = 50f;
        prejudice = 50f;
        savedDialogueIndex = 0;
        
        if (relationships != null)
        {
            relationships.Clear();
        }

        // Força a UI a atualizar com os valores zerados/padrão
        OnStatsChanged?.Invoke(50f, 50f);
        
        Debug.Log("🔄 GameManager: Status resetados para um Novo Jogo!");
    }

    private void Awake()
    {
        // Garante o padrão Singleton (apenas um GameManager existirá e ele não morre entre as cenas)
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // 🔥 O SEGREDO: Carregamos o jogo no Start! 
        // Isso garante que a UI e o Painel de Debug já acordaram e estão prontos para receber os dados salvos.
        if (!SaveSystem.LoadGame())
        {
            ResetarParaNovoJogo();
        }
    }

    /// <summary>
    /// Apply the effects of a choice made in dialogue.
    /// ChoiceData.prideChange and prejudiceChange are expected in range [-1,1].
    /// This method converts them to 0-100 scale and clamps values.
    /// </summary>
    /// <param name="choice">The choice data containing attribute and relationship changes.</param>
    public void ApplyChoice(ChoiceData choice)
    {
        if (choice == null) return;

        // Convert [-1,1] deltas to percentage scale (-100..100) e limita entre 0 e 100
        pride = Mathf.Clamp(pride + choice.prideChange * 100f, 0f, 100f);
        prejudice = Mathf.Clamp(prejudice + choice.prejudiceChange * 100f, 0f, 100f);

        // Apply relationship changes; default relationship is 50 if not present
        if (choice.relationshipChanges != null)
        {
            foreach (var relChange in choice.relationshipChanges)
            {
                if (relChange == null || string.IsNullOrEmpty(relChange.npcName))
                    continue;

                // Se o NPC não existia no dicionário, ele começa com 50 pontos (neutro)
                float old = relationships.ContainsKey(relChange.npcName) ? relationships[relChange.npcName] : 50f;
                
                // Converte a mudança [-1, 1] para a escala de 0-100 e aplica o limite
                float updated = Mathf.Clamp(old + relChange.changeAmount * 100f, 0f, 100f);
                relationships[relChange.npcName] = updated;

                // Notifica a UI / Painel de Debug que a afinidade desse NPC mudou
                OnRelationshipChanged?.Invoke(relChange.npcName, updated);
            }
        }

        // Notifica a UI / Painel de Debug que o Orgulho e Preconceito mudaram
        OnStatsChanged?.Invoke(pride, prejudice);

        // Tenta salvar o progresso automaticamente no arquivo JSON
        try
        {
            SaveSystem.SaveGame(savedDialogueIndex);
        }
        catch (Exception ex)
        {
            Debug.LogWarning("Erro ao tentar salvar o jogo: " + ex.Message);
        }
    }
}