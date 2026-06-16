using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages dialogue display and user interaction.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI speakerText;
    public TextMeshProUGUI dialogueText;
    public Image portraitImage;
    public GameObject choicePanel;
    public GameObject choiceButtonPrefab;
    public Transform choiceButtonContainer;

    private DialogueLine[] dialogueLines;
    private int currentLineIndex = 0;
    private Coroutine typewriterCoroutine;
    private bool isDialogueActive = false; // Controla se existe uma conversa rodando

    private void Start()
    {
        
    }

    private void Update()
    {
        // Se o diálogo estiver ativo e o jogador clicou com o botão esquerdo do mouse (Novo Sistema)
        if (isDialogueActive && UnityEngine.InputSystem.Pointer.current != null && 
            UnityEngine.InputSystem.Pointer.current.press.wasPressedThisFrame)
        {
            // Se as escolhas estiverem abertas na tela, bloqueia o clique para não pular o texto
            if (choicePanel.activeSelf) return;

            AdvanceOrSkipText();
        }
    }

    public void StartDialogue(DialogueLine[] lines)
    {
        dialogueLines = lines;
        currentLineIndex = 0;
        isDialogueActive = true; // Ativa o controle de cliques
        DisplayLine();
    }

    private void DisplayLine()
    {

        // Cancelar qualquer agendamento de avanço automático residual por segurança
        CancelInvoke(nameof(AdvanceDialogue));

        if (currentLineIndex >= dialogueLines.Length)
        {
            EndDialogue();
            return;
        }

        DialogueLine line = dialogueLines[currentLineIndex];
        speakerText.text = line.speaker;
        portraitImage.sprite = line.portrait;
        // Optionally play voice line here if you have an AudioSource

        // Stop any ongoing typewriter effect
        if (typewriterCoroutine != null)
            StopCoroutine(typewriterCoroutine);

        // Start typewriter effect for the dialogue text
        typewriterCoroutine = StartCoroutine(TypewriterEffect(line.text));

        // Handle choices
        if (line.choices != null && line.choices.Length > 0)
        {
            ShowChoices(line.choices);
        }
        else
        {
            HideChoices();

        }
    }

    private IEnumerator TypewriterEffect(string text)
    {
        dialogueText.text = "";
        foreach (char c in text.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.04f); // Adjust speed as needed
        }
        typewriterCoroutine = null;
    }

    private void ShowChoices(ChoiceData[] choices)
    {
        choicePanel.SetActive(true);
        // Clear existing choice buttons
        foreach (Transform child in choiceButtonContainer)
        {
            Destroy(child.gameObject);
        }

        // Instantiate buttons for each choice
        foreach (ChoiceData choice in choices)
        {
            // Instancia o modelo de botão (Prefab) dentro do container
            GameObject btnObj = Instantiate(choiceButtonPrefab, choiceButtonContainer);
        
            // Pega o componente de texto do botão e coloca o texto correto do JSON
            TextMeshProUGUI buttonText = btnObj.GetComponentInChildren<TextMeshProUGUI>();
            if (buttonText != null)
                {
            // Usando 'choiceText' que bate com o seu JSON!
                buttonText.text = choice.choiceText; 
                }

            // ADICIONADO: Configura o botão para acionar a função OnChoiceSelected ao ser clicado
            Button btnComponent = btnObj.GetComponent<Button>();
            if (btnComponent != null)
            {
                btnComponent.onClick.AddListener(() => OnChoiceSelected(choice));
            }
        }
    }

    /// <summary>
    /// Called by PlayerController when player clicks/presses space.
    /// Skips typewriter or advances to next line if text is already displayed.
    /// </summary>
    public void AdvanceOrSkipText()
    {
        if (typewriterCoroutine != null)
        {
            // Skip typewriter effect - show full text immediately
            StopCoroutine(typewriterCoroutine);
            typewriterCoroutine = null;
            DialogueLine line = dialogueLines[currentLineIndex];
            dialogueText.text = line.text;
        }
        else if (choicePanel.activeSelf == false)
        {
            // Text already displayed and no choices - advance to next line
            AdvanceDialogue();
        }
    }

    private void HideChoices()
    {
        choicePanel.SetActive(false);
    }

    private void OnChoiceSelected(ChoiceData choice)
        {
            HideChoices();
    
            // Apply the choice effects to GameManager
            GameManager.Instance.ApplyChoice(choice);

            // Advance to next line
            AdvanceDialogue();
        }
    private void AdvanceDialogue()
    {
        currentLineIndex++;
        DisplayLine();
    }

    private void EndDialogue()
    {
        // Hide dialogue UI
        isDialogueActive = false; // Desativa o controle de cliques pois acabou
        speakerText.text = "";
        dialogueText.text = "";
        portraitImage.sprite = null;
        HideChoices();
        // Optionally, trigger next event (e.g., load next scene, enable player movement)
        Debug.Log("Dialogue ended.");

        // 2. LINHA MÁGICA: Carrega a nova cena de encerramento automaticamente!
        SceneManager.LoadScene("Final");
    }
}