using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Serializable class representing a choice in dialogue.
/// </summary>
[System.Serializable]
public class ChoiceData
{
    public string choiceText;
    [Tooltip("Change to pride (0-1, added to current value)")]
    [Range(-1f, 1f)]
    public float prideChange = 0f;
    [Tooltip("Change to prejudice (0-1, added to current value)")]
    [Range(-1f, 1f)]
    public float prejudiceChange = 0f;
    public List<RelationshipChange> relationshipChanges = new List<RelationshipChange>();
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