using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public enum ControlScheme
    {
        KeyboardMouse,
        Controller
    }
    private InputActions inputActions;

    private ControlScheme controlScheme;
    
    public Vector2 MoveInput { get; private set; }
    public Vector2 MousePosition { get; private set; }

    public Vector2 LookDirection
    {
        get
        {
            if (controlScheme == ControlScheme.KeyboardMouse)
            {
                Vector2 playerScreenPosition = Camera.main.WorldToScreenPoint(transform.position);

                return (MousePosition - playerScreenPosition).normalized;
            }

            if (controlScheme == ControlScheme.Controller)
            {
                Vector2 moveInput = MoveInput;
                moveInput.Normalize();
                return moveInput;
            }
            
            return Vector2.zero;
        }
    }

    public float FishingRodTimePressed { get; private set; }

    public event Action InteractPressed;

    public event Action AttackPressed;

    public event Action SecondaryAttackPressed;
    public event Action UtilityPressed;
    public event Action UtilityReleased;
    
    private bool fishingRodPressed = false;
    
    private void Awake()
    {
        inputActions = new InputActions();
        controlScheme = ControlScheme.Controller;
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
        inputActions.InputScheme.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
        inputActions.InputScheme.Disable();
    }

    private void Update()
    {
        if (GameManager.Instance.CurrentState != GameState.Gameplay) return;
        
        MoveInput = inputActions.Player.Move.ReadValue<Vector2>();
        MousePosition = inputActions.Player.Look.ReadValue<Vector2>();
        
        if (inputActions.InputScheme.KeyboardMouseInput.WasPressedThisFrame() && controlScheme != ControlScheme.KeyboardMouse)
        {
            controlScheme = ControlScheme.KeyboardMouse;
            Debug.Log("Scheme Changed" + controlScheme);
        }

        if (inputActions.InputScheme.ControllerInput.WasPressedThisFrame() && controlScheme != ControlScheme.Controller)
        {
            controlScheme = ControlScheme.Controller;
            Debug.Log("Scheme Changed" + controlScheme);
        }

        if (inputActions.Player.Interact.WasPressedThisFrame())
            InteractPressed?.Invoke();

        if (inputActions.Player.Attack.WasPressedThisFrame())
            AttackPressed?.Invoke();

        if (inputActions.Player.SecondaryAttack.WasPressedThisFrame())
            SecondaryAttackPressed?.Invoke();
        
        UpdateFishingRodInput();
    }

    private void UpdateFishingRodInput()
    {
        if (fishingRodPressed)
        {
            FishingRodTimePressed += Time.deltaTime;
        }
        else
        {
            FishingRodTimePressed = 0;
        }
        
        if (inputActions.Player.UtilityAttack.WasPressedThisFrame())
        {
            UtilityPressed?.Invoke();
            fishingRodPressed = true;
        }

        if (inputActions.Player.UtilityAttack.WasReleasedThisFrame())
        {
            UtilityReleased?.Invoke();
            fishingRodPressed = false;
        }

        
    }
    
    public InputActions GetInputActions()
    {
        return inputActions;
    }

    public ControlScheme GetControlScheme()
    {
        return controlScheme;
    }

    public void CallUpdate()
    {
        Update();
    }
}