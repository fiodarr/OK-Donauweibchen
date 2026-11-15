
using UnityEngine;

public class TrigerDialogueActivator: MonoBehaviour
{
    [SerializeField] private TextAsset textAsset;
    [SerializeField] private DialogueManager  dialogueManager;
    [SerializeField] private GameObject toBeContinuedObject;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            dialogueManager.StartDialogue(textAsset);
            dialogueManager.OnCurrentDialogueComplete += EndGame;
        }
    }

    private void EndGame()
    {
        GameManager.Instance.SetState(GameState.Paused);
        toBeContinuedObject.SetActive(true);
    }
    
}
