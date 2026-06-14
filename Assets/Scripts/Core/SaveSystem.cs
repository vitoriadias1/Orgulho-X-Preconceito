using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Handles saving and loading game state to/from a JSON file.
/// </summary>
public static class SaveSystem
{
    private static string savePath => Path.Combine(Application.persistentDataPath, "savegame.json");

    [System.Serializable]
    private class SaveData
    {
        public float pride;
        public float prejudice;
        public List<RelationshipData> relationships;
    }

    [System.Serializable]
    private class RelationshipData
    {
        public string npcName;
        public float value;
    }

    /// <summary>
    /// Saves the current game state from GameManager.
    /// </summary>
    public static void SaveGame()
    {
        SaveData data = new SaveData
        {
            pride = GameManager.Instance.pride,
            prejudice = GameManager.Instance.prejudice,
            relationships = new List<RelationshipData>()
        };

        foreach (var kvp in GameManager.Instance.relationships)
        {
            data.relationships.Add(new RelationshipData { npcName = kvp.Key, value = kvp.Value });
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"Game saved to {savePath}");
    }

    /// <summary>
    /// Loads the game state into GameManager.
    /// Returns true if load was successful, false otherwise.
    /// </summary>
    public static bool LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning($"Save file not found at {savePath}");
            return false;
        }

        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        GameManager.Instance.pride = data.pride;
        GameManager.Instance.prejudice = data.prejudice;
        GameManager.Instance.relationships.Clear();

        foreach (var relData in data.relationships)
        {
            GameManager.Instance.relationships[relData.npcName] = relData.value;
        }

        Debug.Log($"Game loaded from {savePath}");
        return true;
    }
}