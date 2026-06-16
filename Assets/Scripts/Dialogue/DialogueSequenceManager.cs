using System.Collections.Generic;
using UnityEngine;

public class DialogueSequenceManager : MonoBehaviour
{
    [System.Serializable]
    public class DialogueSequence
    {
        public int chapterNumber;
        public int sceneNumber;
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

    public static DialogueSequence GetDialogueSequence(int chapterNumber, int sceneNumber)
    {
        if (instance == null)
            {
            instance = Object.FindFirstObjectByType<DialogueSequenceManager>();
            }

        if (instance == null)
            {
            Debug.LogError("DialogueSequenceManager não encontrado na cena!");
            return null;
            }

        string key = "Linear_Script";

        if (instance.dialogueCache.ContainsKey(key))
        {
            return instance.dialogueCache[key];
        }

        // Carrega todos os ScriptableObjects (.asset) da pasta de uma vez só
        DialogueLine[] loadedLines = Resources.LoadAll<DialogueLine>("Dialogues");

        if (loadedLines == null || loadedLines.Length == 0)
        {
            return null;
        }

        DialogueSequence sequence = new DialogueSequence
        {
            chapterNumber = 1,
            sceneNumber = 1,
            dialogueLines = loadedLines
        };

        instance.dialogueCache[key] = sequence;
        return sequence;
    }
}