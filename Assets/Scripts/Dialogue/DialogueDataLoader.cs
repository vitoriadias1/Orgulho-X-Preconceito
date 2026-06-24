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
        // 1. Caminho da pasta onde você colocou os múltiplos arquivos JSON
        string jsonFolderPath = "Assets/Scripts/Data/";
        string baseOutputPath = "Assets/DialogueData/";

        if (!Directory.Exists(jsonFolderPath))
        {
            Debug.LogError($"JSON folder not found at {jsonFolderPath}. Certifique-se de que a pasta existe!");
            return;
        }

        // 2. Busca AUTOMATICAMENTE por todos os arquivos .json dentro daquela pasta
        string[] jsonFiles = Directory.GetFiles(jsonFolderPath, "*.json");

        if (jsonFiles.Length == 0)
        {
            Debug.LogWarning($"Nenhum arquivo JSON encontrado em {jsonFolderPath}");
            return;
        }

        int totalLinesGenerated = 0;
        int filesProcessed = 0;

        // 3. Loop que vai ler arquivo por arquivo encontrado
        foreach (string jsonPath in jsonFiles)
        {
            string json = File.ReadAllText(jsonPath);
            DialogueDataWrapper data = JsonUtility.FromJson<DialogueDataWrapper>(json);

            if (data == null || data.chapters == null)
            {
                Debug.LogWarning($"Falha ao decodificar ou arquivo vazio: {jsonPath}");
                continue;
            }

            filesProcessed++;

            foreach (var chapter in data.chapters)
            {
                // 4. Cria uma SUBPASTA dedicada para este capítulo (Ex: Assets/DialogueData/Chapter_1)
                string chapterOutputPath = Path.Combine(baseOutputPath, $"Chapter_{chapter.chapterNumber}");
                if (!Directory.Exists(chapterOutputPath))
                {
                    Directory.CreateDirectory(chapterOutputPath);
                }

                // O contador reseta a cada capítulo/arquivo para manter os nomes padronizados (00, 01, 02...)
                int dialogueCounter = 0; 

                foreach (var scene in chapter.scenes)
                {
                    foreach (var lineData in scene.dialogueLines)
                    {
                        // Create DialogueLine ScriptableObject
                        DialogueLine dialogueLine = ScriptableObject.CreateInstance<DialogueLine>();
                        dialogueLine.speaker = lineData.speaker;
                        dialogueLine.text = lineData.text;
                        dialogueLine.portrait = null; 
                        dialogueLine.voice = null; 

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

                        // 5. Salva o asset dentro da subpasta do capítulo correspondente
                        string fileName = $"Dialogue_C{chapter.chapterNumber}S{scene.sceneNumber}_{dialogueCounter:00}_{CleanFileName(lineData.speaker)}.asset";
                        string filePath = Path.Combine(chapterOutputPath, fileName);
                        
                        AssetDatabase.CreateAsset(dialogueLine, filePath);
                        dialogueCounter++;
                        totalLinesGenerated++;
                    }
                }
            }
        }

        // Atualiza a aba Project da Unity para exibir os novos arquivos
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"<color=green><b>✓ SUCESSO!</b></color> Processou {filesProcessed} arquivos JSON e gerou {totalLinesGenerated} linhas de diálogo organizadas em subpastas!");
    }

    private static string CleanFileName(string fileName)
    {
        string cleaned = fileName.Replace(" ", "").Replace(".", "");
        return System.Text.RegularExpressions.Regex.Replace(cleaned, @"[^a-zA-Z0-9_]", "");
    }
#endif
}