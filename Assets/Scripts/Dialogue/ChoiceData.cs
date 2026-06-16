using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serializable class representing a choice in dialogue.
/// </summary>
[System.Serializable]
public class ChoiceData
{
    // Precisa ser exatamente igual ao nome no JSON!
    public string choiceText; 
    
    public float prideChange;
    public float prejudiceChange;
    public List<RelationshipChange> relationshipChanges;
}

/// <summary>
/// Serializable class for changing relationship with an NPC.
/// </summary>
[System.Serializable]
public class RelationshipChange
{
    public string npcName;
    [Tooltip("Change to relationship (0-1, added to current value)")]
    [Range(-1f, 1f)]
    public float changeAmount = 0f;
}