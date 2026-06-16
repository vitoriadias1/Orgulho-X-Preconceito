using UnityEngine;

/// <summary>
/// Example script showing how to load and play dialogue sequences
/// Attach this to a scene manager or dialogue trigger
/// </summary>
public class DialogueSceneExample : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    
    [Header("Dialogue Configuration")]
    [SerializeField] private int chapterNumber = 1;
    [SerializeField] private int sceneNumber = 1;

    private void Start()
    {
    // Em vez de chamar a função direto, usamos uma Coroutine com atraso
    StartCoroutine(IniciarDialogoComAtraso());
    }

    private System.Collections.IEnumerator IniciarDialogoComAtraso()
    {
    // Espera 0.2 segundos para o DialogueSequenceManager se inicializar por completo
    yield return new WaitForSeconds(0.2f);

    // Agora sim, buscamos os gerenciadores de forma ultra segura
    DialogueSequenceManager sequenceManager = Object.FindAnyObjectByType<DialogueSequenceManager>();

    if (sequenceManager != null)
        {
        // Dispara a sequência com segurança
        PlayDialogueSequence(1, 1);
        }
    else
        {
        Debug.LogError("O script DialogueSceneExample não encontrou o DialogueSequenceManager na cena!");
        }
    }
    /// <summary>
    /// Load and play a dialogue sequence
    /// </summary>
    public void PlayDialogueSequence(int chapter, int scene)
    {
        DialogueSequenceManager.DialogueSequence sequence = 
            DialogueSequenceManager.GetDialogueSequence(chapter, scene);

        if (sequence != null && sequence.dialogueLines.Length > 0)
        {
            if (dialogueManager != null)
            {
                dialogueManager.StartDialogue(sequence.dialogueLines);
                Debug.Log($"Playing: Chapter {chapter} - Scene {scene}: {sequence.sceneName}");
            }
            else
            {
                Debug.LogError("DialogueManager not assigned!");
            }
        }
        else
        {
            Debug.LogWarning($"No dialogue sequence found for Chapter {chapter}, Scene {scene}");
        }
    }

    /// <summary>
    /// Preload an entire chapter for better performance
    /// </summary>
    public void PreloadChapterDialogues(int chapter)
    {
        DialogueSequenceManager.PreloadChapter(chapter);
        Debug.Log($"Preloaded Chapter {chapter}");
    }

    /// <summary>
    /// Example: Play Chapter I, Scene 1 (Ball Arrival)
    /// </summary>
    public void PlayBallScene()
    {
        PlayDialogueSequence(1, 1);
    }

    /// <summary>
    /// Example: Play Chapter I, Scene 2 (Darcy Observes Elizabeth)
    /// </summary>
    public void PlayDarcyObservesElizabeth()
    {
        PlayDialogueSequence(1, 2);
    }

    /// <summary>
    /// Example: Play Chapter II, Scene 1 (Meeting with Wickham)
    /// </summary>
    public void PlayWickhamMeeting()
    {
        PlayDialogueSequence(2, 1);
    }

    /// <summary>
    /// Example: Play Chapter III, Scene 1 (Library at Rosings)
    /// </summary>
    public void PlayRosingsLibrary()
    {
        PlayDialogueSequence(3, 1);
    }

    /// <summary>
    /// Example: Play Chapter IV, Scene 1 (Arrival at Pemberley)
    /// </summary>
    public void PlayPemberleyArrival()
    {
        PlayDialogueSequence(4, 1);
    }

    /// <summary>
    /// Example: Play Chapter IV, Scene 2 (The Governess)
    /// </summary>
    public void PlayGovernessScene()
    {
        PlayDialogueSequence(4, 2);
    }

    /// <summary>
    /// Example: Play Chapter IV, Scene 3 (The Encounter)
    /// </summary>
    public void PlayPemberleyEncounter()
    {
        PlayDialogueSequence(4, 3);
    }

    /// <summary>
    /// Example: Play Chapter V, Scene 1 (Jane's Letter)
    /// </summary>
    public void PlayJanesLetter()
    {
        PlayDialogueSequence(5, 1);
    }

    /// <summary>
    /// Example: Play Chapter V, Scene 3 (Darcy Discovers the Scandal)
    /// </summary>
    public void PlayScandalDiscovery()
    {
        PlayDialogueSequence(5, 3);
    }

    /// <summary>
    /// Example: Play Chapter VIII, Scene 2 (The Second Proposal - Final Scene)
    /// </summary>
    public void PlayFinalProposal()
    {
        PlayDialogueSequence(8, 2);
    }
}
