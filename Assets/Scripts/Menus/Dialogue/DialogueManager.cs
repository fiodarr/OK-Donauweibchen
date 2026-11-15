using System;
using TMPro;
using UnityEngine;

public class DialogueManager: MonoBehaviour
{
    private string[] dialogueLines;
    [SerializeField] private GameObject dialogueWindow;
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private PlayerInputHandler inputHandler;
    private int currentLine;
    private int linesMax;
    
    public event Action OnCurrentDialogueComplete;

   public void StartDialogue(TextAsset textAsset)
    {
        inputHandler.DialogueNext += NextLine;
        GameManager.Instance.SetState(GameState.Dialogue);
        dialogueLines = DialogueParser.GetDialogueLines(textAsset);
        dialogueWindow.SetActive(true);
        linesMax = dialogueLines.Length;
        currentLine = 0;
        dialogueText.text = dialogueLines[currentLine];
    }

    private void NextLine()
    {
        currentLine++;
        if (currentLine >= linesMax) EndDialogue();
        else dialogueText.text = dialogueLines[currentLine];
    }

    private void EndDialogue()
    {
        dialogueWindow.SetActive(false);
        GameManager.Instance.SetState(GameState.Gameplay);
        inputHandler.DialogueNext -= NextLine;
        OnCurrentDialogueComplete?.Invoke();
    }
}
