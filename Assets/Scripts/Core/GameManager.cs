using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Singleton GameManager to hold game state: pride, prejudice, and relationships.
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
    /// </summary>
    /// <param name="choice">The choice data containing attribute and relationship changes.</param>
    public void ApplyChoice(ChoiceData choice)
    {
        pride = Mathf.Clamp01(pride + choice.prideChange) * 100f;
        prejudice = Mathf.Clamp01(prejudice + choice.prejudiceChange) * 100f;

        foreach (var relChange in choice.relationshipChanges)
        {
            if (relationships.ContainsKey(relChange.npcName))
            {
                relationships[relChange.npcName] = Mathf.Clamp01(relationships[relChange.npcName] + relChange.changeAmount) * 100f;
            }
            else
            {
                relationships[relChange.npcName] = Mathf.Clamp01(relChange.changeAmount) * 100f;
            }
        }
    }
}