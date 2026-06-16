using UnityEngine;

/// <summary>
/// ScriptableObject representing a line of dialogue.
/// </summary>
[CreateAssetMenu(fileName = "NewDialogueLine", menuName = "Dialogue/DialogueLine")]
public class DialogueLine : ScriptableObject
{
    public string speaker;
    [TextArea(3, 10)]
    public string text;
    public Sprite portrait;
    public AudioClip voice;
    public ChoiceData[] choices; // Optional: if null or empty, no choices shown
}