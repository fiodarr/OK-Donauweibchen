using UnityEngine;

public class GameManager: MonoBehaviour
{
    public static GameManager Instance;
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private TextAsset startingDialogue;
    
    public GameState CurrentState { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        dialogueManager.StartDialogue(startingDialogue);
        
    }

    public void SetState(GameState state)
    {
        CurrentState = state;
        if (CurrentState != GameState.Gameplay)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
