using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using System.IO;
#endif

/// <summary>
/// Loads dialogue data from JSON and creates DialogueLine ScriptableObjects
/// </summary>
public class DialogueDataLoader : MonoBehaviour
{
    [System.Serializable]
    public class DialogueChapter
    {
        public int chapterNumber;
        public string chapterName;
        public DialogueScene[] scenes;
    }

    [System.Serializable]
    public class DialogueScene
    {
        public int sceneNumber;
        public string sceneName;
        public DialogueLineData[] dialogueLines;
    }

    [System.Serializable]
    public class DialogueLineData
    {
        public string speaker;
        public string text;
        public string portrait;
        public ChoiceDataWrapper[] choices;
    }

    [System.Serializable]
    public class ChoiceDataWrapper
    {
        public string choiceText;
        public float prideChange;
        public float prejudiceChange;
        public RelationshipChangeWrapper[] relationshipChanges;
    }

    [System.Serializable]
    public class RelationshipChangeWrapper
    {
        public string npcName;
        public float changeAmount;
    }

    [System.Serializable]
    public class DialogueDataWrapper
    {
        public DialogueChapter[] chapters;
    }

#if UNITY_EDITOR
    [MenuItem("Tools/Generate Dialogue ScriptableObjects from JSON")]
    public static void GenerateDialoguesFromJSON()
    {
        // Altere para o nome exato do seu arquivo JSON:
        string jsonPath = "Assets/Scripts/Data/Dialogue_1_1.json";
        string outputPath = "Assets/DialogueData/";

        if (!File.Exists(jsonPath))
        {
            Debug.LogError($"JSON file not found at {jsonPath}");
            return;
        }

        // Create output directory if it doesn't exist
        if (!Directory.Exists(outputPath))
        {
            Directory.CreateDirectory(outputPath);
        }

        string json = File.ReadAllText(jsonPath);
        DialogueDataWrapper data = JsonUtility.FromJson<DialogueDataWrapper>(json);

        if (data == null || data.chapters == null)
        {
            Debug.LogError("Failed to parse JSON or no chapters found");
            return;
        }

        int dialogueCounter = 0;

        foreach (var chapter in data.chapters)
        {
            foreach (var scene in chapter.scenes)
            {
                foreach (var lineData in scene.dialogueLines)
                {
                    // Create DialogueLine ScriptableObject
                    DialogueLine dialogueLine = ScriptableObject.CreateInstance<DialogueLine>();
                    dialogueLine.speaker = lineData.speaker;
                    dialogueLine.text = lineData.text;
                    dialogueLine.portrait = null; // Portrait loading would need sprite references
                    dialogueLine.voice = null; // Voice audio loading would need audio references

                    // Convert choices
                    if (lineData.choices != null && lineData.choices.Length > 0)
                    {
                        dialogueLine.choices = new ChoiceData[lineData.choices.Length];

                        for (int i = 0; i < lineData.choices.Length; i++)
                        {
                            var choiceWrapper = lineData.choices[i];
                            ChoiceData choice = new ChoiceData
                            {
                                choiceText = choiceWrapper.choiceText,
                                prideChange = choiceWrapper.prideChange,
                                prejudiceChange = choiceWrapper.prejudiceChange,
                                relationshipChanges = new List<RelationshipChange>()
                            };

                            if (choiceWrapper.relationshipChanges != null)
                            {
                                foreach (var relChange in choiceWrapper.relationshipChanges)
                                {
                                    choice.relationshipChanges.Add(new RelationshipChange
                                    {
                                        npcName = relChange.npcName,
                                        changeAmount = relChange.changeAmount
                                    });
                                }
                            }

                            dialogueLine.choices[i] = choice;
                        }
                    }
                    else
                    {
                        dialogueLine.choices = new ChoiceData[0];
                    }

                    // Save as asset
                    string fileName = $"Dialogue_C{chapter.chapterNumber}S{scene.sceneNumber}_{dialogueCounter:00}_{CleanFileName(lineData.speaker)}.asset";
                    string filePath = Path.Combine(outputPath, fileName);
                    AssetDatabase.CreateAsset(dialogueLine, filePath);
                    dialogueCounter++;
                }
            }
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log($"✓ Generated {dialogueCounter} dialogue lines from JSON!");
    }

    private static string CleanFileName(string fileName)
    {
        string cleaned = fileName.Replace(" ", "").Replace(".", "");
        return System.Text.RegularExpressions.Regex.Replace(cleaned, @"[^a-zA-Z0-9_]", "");
    }
#endif
}
