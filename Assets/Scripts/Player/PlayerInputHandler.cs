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
    public Vector2 RightStickInput { get; private set; }
    
    public Vector2 AimDirection 
    {
        get
        {
            if (controlScheme == ControlScheme.KeyboardMouse)
            {
                return LookDirection;
            }
            else
            {
                return RightStickInput.normalized;
            }
        }
    }

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

    public event Action UtilityAttack;

    public event Action Dash;

    public event Action Heal;
    
    
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
        RightStickInput = inputActions.Player.ControllerStickLook.ReadValue<Vector2>().normalized;
        
        if (inputActions.InputScheme.KeyboardMouseInput.WasPressedThisFrame() && controlScheme != ControlScheme.KeyboardMouse)
        {
            controlScheme = ControlScheme.KeyboardMouse;
        }

        if (inputActions.InputScheme.ControllerInput.WasPressedThisFrame() && controlScheme != ControlScheme.Controller)
        {
            controlScheme = ControlScheme.Controller;
        }

        if (inputActions.Player.Interact.WasPressedThisFrame())
            InteractPressed?.Invoke();

        if (inputActions.Player.Attack.WasPressedThisFrame())
            AttackPressed?.Invoke();

        if (inputActions.Player.SecondaryAttack.WasPressedThisFrame())
            SecondaryAttackPressed?.Invoke();

        if (inputActions.Player.Heal.WasPressedThisFrame())
        {
            Heal?.Invoke();
        }

        if (inputActions.Player.Evade.WasPressedThisFrame())
        {
            Dash?.Invoke();
        }
        
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
        
        if ((controlScheme == ControlScheme.KeyboardMouse && inputActions.Player.UtilityAttack.WasPressedThisFrame())
            ||  (controlScheme != ControlScheme.KeyboardMouse && RightStickInput != Vector2.zero))
        {
            UtilityPressed?.Invoke();
            fishingRodPressed = true;
        }
        
        if ((controlScheme == ControlScheme.KeyboardMouse && inputActions.Player.UtilityAttack.WasReleasedThisFrame()) 
            || (controlScheme != ControlScheme.KeyboardMouse && RightStickInput == Vector2.zero) 
            && fishingRodPressed
            )
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