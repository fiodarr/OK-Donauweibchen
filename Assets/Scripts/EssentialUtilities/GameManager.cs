using System;
using System.Collections;
using UnityEngine;

public class GameManager: MonoBehaviour
{
    public static GameManager Instance;
    
    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private TextAsset startingDialogue;
    [SerializeField] private FleingMermaid mermaid;
    
    [SerializeField, Range(0.01f, 1f)] private float aimingTimeScale = 0.6f;
    [SerializeField] private float aimSlowInDuration = 0.25f;
    [SerializeField] private float aimSlowOutDuration = 0.12f;

    private float currentAimingModifier = 1f;
    private float targetAimingModifier = 1f;

    private Coroutine aimingTimeScaleCoroutine;
    
    public event Action controlSchemeChanged;
    public GameState CurrentState { get; private set; }
    private PlayerInputHandler.ControlScheme currentControlScheme;

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
    public PlayerInputHandler.ControlScheme CurrentControlScheme
    {
        get => currentControlScheme;
    }
    public void SetControlScheme(PlayerInputHandler.ControlScheme controlScheme)
    {
        currentControlScheme = controlScheme;
        controlSchemeChanged?.Invoke();
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
        switch (CurrentState)
        {
            case GameState.RestSpot:
                Time.timeScale = 1;
                break;
            case GameState.Gameplay:
                Time.timeScale = 1 * timeModifier * currentAimingModifier;
                break;
            default:
                Time.timeScale = 0;
                break;
        }
        
        if (CurrentState != GameState.Gameplay)
        {
            Time.timeScale = 0 * timeModifier;
        }
        else
        {
            Time.timeScale = 1 * timeModifier * currentAimingModifier;
        }
    }

    private void FleeMermaid()
    {
        dialogueManager.OnCurrentDialogueComplete -= FleeMermaid;
        mermaid.Flee();
    }
    
    public void SetAimingSlowMotion(bool aiming)
    {
        float target = aiming ? aimingTimeScale : 1f;

        if (Mathf.Approximately(target, targetAimingModifier))
            return;

        targetAimingModifier = target;

        if (aimingTimeScaleCoroutine != null)
        {
            StopCoroutine(aimingTimeScaleCoroutine);
        }

        float duration = aiming
            ? aimSlowInDuration
            : aimSlowOutDuration;

        aimingTimeScaleCoroutine = StartCoroutine(
            TransitionAimingTimeScale(target, duration)
        );
    }
    
    private IEnumerator TransitionAimingTimeScale(float target, float duration)
    {
        float start = currentAimingModifier;
        float elapsed = 0f;

        if (duration <= 0f)
        {
            currentAimingModifier = target;
            UpdateTimeScale();
            aimingTimeScaleCoroutine = null;
            yield break;
        }

        while (elapsed < duration)
        {
            elapsed += Time.unscaledDeltaTime;

            float t = Mathf.Clamp01(elapsed / duration);
            float smoothT = Mathf.SmoothStep(0f, 1f, t);

            currentAimingModifier = Mathf.Lerp(start, target, smoothT);

            UpdateTimeScale();

            yield return null;
        }

        currentAimingModifier = target;
        UpdateTimeScale();

        aimingTimeScaleCoroutine = null;
    }
    
}
