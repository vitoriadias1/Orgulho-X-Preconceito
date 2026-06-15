using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Singleton GameManager to hold game state: pride, prejudice, and relationships.
/// Provides events and persistence hooks for UI and other systems.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Range(0f, 100f)]
    public float pride = 50f;
    [Range(0f, 100f)]
    public float prejudice = 50f;

    // Relationships: NPC name -> relationship value (0-100)
    public Dictionary<string, float> relationships = new Dictionary<string, float>();

    // Events for UI / other systems to subscribe
    public Action<float, float> OnStatsChanged;
    public Action<string, float> OnRelationshipChanged;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
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

        // Convert [-1,1] deltas to percentage scale (-100..100)
        pride = Mathf.Clamp(pride + choice.prideChange * 100f, 0f, 100f);
        prejudice = Mathf.Clamp(prejudice + choice.prejudiceChange * 100f, 0f, 100f);

        // Apply relationship changes; default relationship is 50 if not present
        foreach (var relChange in choice.relationshipChanges)
        {
            if (relChange == null || string.IsNullOrEmpty(relChange.npcName))
                continue;

            float old = relationships.ContainsKey(relChange.npcName) ? relationships[relChange.npcName] : 50f;
            float updated = Mathf.Clamp(old + relChange.changeAmount * 100f, 0f, 100f);
            relationships[relChange.npcName] = updated;

            // Notify listeners about relationship change
            OnRelationshipChanged?.Invoke(relChange.npcName, updated);
        }

        // Notify listeners about stats change
        OnStatsChanged?.Invoke(pride, prejudice);

        // Persist current state using static SaveSystem if available
        try
        {
            SaveSystem.SaveGame();
        }
        catch (Exception ex)
        {
            Debug.LogWarning("SaveSystem.SaveGame threw: " + ex.Message);
        }
    }
}