using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages dialogue display and user interaction.
/// </summary>
public class DialogueManager : MonoBehaviour
{
    [Header("UI References")]
    public Text speakerText;
    public Text dialogueText;
    public Image portraitImage;
    public GameObject choicePanel;
    public GameObject choiceButtonPrefab;
    public Transform choiceButtonContainer;

    private DialogueLine[] dialogueLines;
    private int currentLineIndex = 0;
    private Coroutine typewriterCoroutine;

    private void Start()
    {
        // Example: Load dialogue lines from Resources or set via inspector
        // For MVP, we assume they are set in the inspector via array.
        // Alternatively, we can load from Resources folder.
        // We'll leave it to be set in inspector.
    }

    /// <summary>
    /// Start displaying the dialogue.
    /// </summary>
    /// <param name="lines">Array of DialogueLine objects to display in sequence.</param>
    public void StartDialogue(DialogueLine[] lines)
    {
        dialogueLines = lines;
        currentLineIndex = 0;
        DisplayLine();
    }

    private void DisplayLine()
    {
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
            // Auto-advance after a short delay if no choices
            Invoke(nameof(AdvanceDialogue), 2f);
        }
    }

    private IEnumerator TypewriterEffect(string text)
    {
        dialogueText.text = "";
        foreach (char c in text.ToCharArray())
        {
            dialogueText.text += c;
            yield return new WaitForSeconds(0.05f); // Adjust speed as needed
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
            GameObject buttonObj = Instantiate(choiceButtonPrefab, choiceButtonContainer);
            Text buttonText = buttonObj.GetComponentInChildren<Text>();
            if (buttonText != null)
                buttonText.text = choice.choiceText;

            // Add listener to handle choice selection
            Button btn = buttonObj.GetComponent<Button>();
            btn.onClick.AddListener(() => OnChoiceSelected(choice));
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
        speakerText.text = "";
        dialogueText.text = "";
        portraitImage.sprite = null;
        HideChoices();
        // Optionally, trigger next event (e.g., load next scene, enable player movement)
        Debug.Log("Dialogue ended.");
    }
}