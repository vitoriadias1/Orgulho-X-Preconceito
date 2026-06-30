using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement; // 🔥 Adicionado para ler a cena atual

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
        public int dialogueIndex;
        public string sceneName; // 🔥 Guardará o nome da cena/capítulo atual
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
    public static void SaveGame(int currentLine)
    {
        SaveData data = new SaveData
        {
            pride = GameManager.Instance.pride,
            prejudice = GameManager.Instance.prejudice,
            dialogueIndex = currentLine,
            sceneName = SceneManager.GetActiveScene().name, // 🔥 Captura a cena atual
            relationships = new List<RelationshipData>()
        };

        foreach (var kvp in GameManager.Instance.relationships)
        {
            data.relationships.Add(new RelationshipData { npcName = kvp.Key, value = kvp.Value });
        }

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log($"Game saved to {savePath} (Scene: {data.sceneName})");
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

        // Aplica os status globais
        GameManager.Instance.pride = data.pride;
        GameManager.Instance.prejudice = data.prejudice;
        GameManager.Instance.savedDialogueIndex = data.dialogueIndex;

        // Limpa e repopula o dicionário de relacionamentos do GameManager
        GameManager.Instance.relationships.Clear();
        foreach (var relData in data.relationships)
        {
            GameManager.Instance.relationships[relData.npcName] = relData.value;
            
            GameManager.Instance.OnRelationshipChanged?.Invoke(relData.npcName, relData.value);
        }

        GameManager.Instance.OnStatsChanged?.Invoke(data.pride, data.prejudice);

        Debug.Log($"Game loaded from {savePath}. Dialogue Index: {data.dialogueIndex}");
        return true;
    }

    /// <summary>
    /// Retorna o nome da cena que está salva no arquivo JSON (útil para o botão "Continuar" do Menu Principal).
    /// </summary>
    public static string ObterCenaSalva()
    {
        if (!File.Exists(savePath)) return "";
        string json = File.ReadAllText(savePath);
        SaveData data = JsonUtility.FromJson<SaveData>(json);
        return data.sceneName;
    }
}