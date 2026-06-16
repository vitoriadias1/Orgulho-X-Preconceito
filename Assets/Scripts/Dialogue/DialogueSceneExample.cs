using UnityEngine;
using System.Collections;

public class DialogueSceneExample : MonoBehaviour
{
    [Header("Conecte o seu DialogueManager da Hierarchy aqui")]
    public DialogueManager dialogueManager;

    private void Start()
    {
        // Mini atraso de segurança para a Unity inicializar os objetos
        StartCoroutine(StartDialogueRoutine());
    }

    private IEnumerator StartDialogueRoutine()
    {
        yield return new WaitForSeconds(0.5f);

        // Busca a sequência única contendo todas as falas da pasta Resources/Dialogues
        DialogueSequenceManager.DialogueSequence sequence = DialogueSequenceManager.GetDialogueSequence(1, 1);

        if (sequence != null && sequence.dialogueLines != null && sequence.dialogueLines.Length > 0)
        {
            Debug.Log($"✓ Roteiro carregado com sucesso! Enviando {sequence.dialogueLines.Length} falas para o DialogueManager.");
            
            if (dialogueManager != null)
            {
                // 🚀 O PULO DO GATO: Passamos a lista (.dialogueLines) direto para o seu StartDialogue original!
                dialogueManager.StartDialogue(sequence.dialogueLines);
            }
            else
            {
                Debug.LogError("Erro: O campo Dialogue Manager está vazio no Inspector!");
            }
        }
        else
        {
            Debug.LogError("Erro: Nenhuma fala (.asset) encontrada em Assets/Resources/Dialogues/");
        }
    }
}