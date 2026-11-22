using UnityEngine;

public class GameManager: MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private TextAsset startingDialogue;
    [SerializeField] private FleingMermaid mermaid;
    
    public GameState CurrentState { get; private set; }
    public PlayerInputHandler.ControlScheme currentControlScheme;

    public float timeModifier = 1f;

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
        dialogueManager.OnCurrentDialogueComplete += FleeMermaid;
        
    }

    public void SetState(GameState state)
    {
        CurrentState = state;
        UpdateTimeScale();
    }

    public void SetTimeModifier(float modifier)
    {
        timeModifier = modifier;
        UpdateTimeScale();
    }

    public void UpdateTimeScale()
    {
        if (CurrentState != GameState.Gameplay)
        {
            Time.timeScale = 0 * timeModifier;
        }
        else
        {
            Time.timeScale = 1 * timeModifier;
        }
    }

    private void FleeMermaid()
    {
        dialogueManager.OnCurrentDialogueComplete -= FleeMermaid;
        mermaid.Flee();
    }
}
