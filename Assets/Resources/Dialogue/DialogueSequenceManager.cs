using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manager that provides organized access to dialogue sequences by chapter and scene
/// </summary>
public class DialogueSequenceManager : MonoBehaviour
{
    [System.Serializable]
    public class DialogueSequence
    {
        public int chapterNumber;
        public string chapterName;
        public int sceneNumber;
        public string sceneName;
        public DialogueLine[] dialogueLines;
    }

    private static DialogueSequenceManager instance;
    private Dictionary<string, DialogueSequence> dialogueCache = new Dictionary<string, DialogueSequence>();

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);

        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Get dialogue sequence for a specific chapter and scene
    /// </summary>
    public static DialogueSequence GetDialogueSequence(int chapterNumber, int sceneNumber)
    {
        if (instance == null)
        {
            Debug.LogError("DialogueSequenceManager not initialized");
            return null;
        }

        string key = $"C{chapterNumber}_S{sceneNumber}";

        if (instance.dialogueCache.ContainsKey(key))
        {
            return instance.dialogueCache[key];
        }

        // Load all dialogue lines for this chapter/scene from Resources
        DialogueLine[] loadedLines = Resources.LoadAll<DialogueLine>($"Dialogues/Chapter{chapterNumber}/Scene{sceneNumber}");

        if (loadedLines.Length == 0)
        {
            Debug.LogWarning($"No dialogues found for Chapter {chapterNumber}, Scene {sceneNumber}");
            return null;
        }

        DialogueSequence sequence = new DialogueSequence
        {
            chapterNumber = chapterNumber,
            sceneNumber = sceneNumber,
            dialogueLines = loadedLines
        };

        instance.dialogueCache[key] = sequence;
        return sequence;
    }

    /// <summary>
    /// Preload all dialogues from a specific chapter
    /// </summary>
    public static void PreloadChapter(int chapterNumber)
    {
        DialogueLine[] allLines = Resources.LoadAll<DialogueLine>($"Dialogues/Chapter{chapterNumber}");
        Debug.Log($"Preloaded {allLines.Length} dialogues for Chapter {chapterNumber}");
    }

    /// <summary>
    /// Clear cache to free memory
    /// </summary>
    public static void ClearCache()
    {
        if (instance != null)
        {
            instance.dialogueCache.Clear();
        }
    }

    /// <summary>
    /// Get all loaded dialogue keys (for debugging)
    /// </summary>
    public static string[] GetLoadedDialogueKeys()
    {
        if (instance == null) return new string[0];
        string[] keys = new string[instance.dialogueCache.Keys.Count];
        instance.dialogueCache.Keys.CopyTo(keys, 0);
        return keys;
    }
}
