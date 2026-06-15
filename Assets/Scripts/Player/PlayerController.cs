using UnityEngine;

/// <summary>
/// Visual Novel style input controller - handles dialogue navigation and UI interactions.
/// No character movement - this is a narrative-focused game.
/// </summary>
public class PlayerController : MonoBehaviour
{
    private DialogueManager dialogueManager;

    private void Start()
    {
        dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager == null)
        {
            Debug.LogError("DialogueManager not found in scene!");
        }
    }

    private void Update()
    {
        HandleDialogueInput();
    }

    /// <summary>
    /// Handle dialogue navigation: click to advance or skip text.
    /// Supports both the old Input manager and the new Input System.
    /// </summary>
    private void HandleDialogueInput()
    {
        if (dialogueManager == null) return;

        bool advance = false;

#if ENABLE_INPUT_SYSTEM && !ENABLE_LEGACY_INPUT_MANAGER
        // New Input System
        if (UnityEngine.InputSystem.Mouse.current != null && UnityEngine.InputSystem.Mouse.current.leftButton.wasPressedThisFrame)
            advance = true;
        if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
            advance = true;
        if (UnityEngine.InputSystem.Keyboard.current != null && UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            OpenMenu();
            return;
        }
#else
        // Old Input Manager
        if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space))
            advance = true;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            OpenMenu();
            return;
        }
#endif

        if (advance)
            dialogueManager.AdvanceOrSkipText();
    }

    private void OpenMenu()
    {
        Debug.Log("Opening menu...");
        // TODO: Implement pause menu / options menu
    }
}