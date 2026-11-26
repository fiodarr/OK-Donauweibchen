using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    public enum ControlScheme
    {
        KeyboardMouse,
        Controller,
        XboxController
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

            if (controlScheme == ControlScheme.Controller ||  controlScheme == ControlScheme.XboxController)
            {
                Vector2 moveInput = MoveInput;
                moveInput.Normalize();
                return moveInput;
            }
            
            return Vector2.zero;
        }
    }

    public float FishingRodTimePressed { get; private set; }

    public event Action controlSchemeChanged;
    
    public event Action NextPanel;
    public event Action InteractPressed;

    public event Action AttackPressed;

    public event Action SecondaryAttackPressed;
    public event Action UtilityPressed;
    public event Action UtilityReleased;
    

    public event Action UtilityAttack;

    public event Action Dash;

    public event Action Heal;

    public event Action MenuConfirm;
    public event Action MenuClose;

    public event Action DialogueNext;
    
    
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
        if (GameManager.Instance.CurrentState == GameState.Dialogue)
        {
            if (inputActions.Player.Interact.WasPressedThisFrame())
            {
                Debug.Log("new dialogue line");
                DialogueNext?.Invoke();
                NextPanel?.Invoke();
            }
        }

        if (GameManager.Instance.CurrentState != GameState.Gameplay) return;
        
        MoveInput = inputActions.Player.Move.ReadValue<Vector2>();
        MousePosition = inputActions.Player.Look.ReadValue<Vector2>();
        RightStickInput = inputActions.Player.ControllerStickLook.ReadValue<Vector2>().normalized;

        bool controlSchemeChanged = false;
        
        if (inputActions.InputScheme.KeyboardMouseInput.WasPressedThisFrame() && controlScheme != ControlScheme.KeyboardMouse)
        {
            controlScheme = ControlScheme.KeyboardMouse;
            GameManager.Instance.SetControlScheme(controlScheme);
            Debug.Log("Now using Keyboard Mouse");
        }

        if (inputActions.InputScheme.ControllerInput.WasPressedThisFrame() && controlScheme != ControlScheme.Controller)
        {
            controlScheme = ControlScheme.Controller;
            GameManager.Instance.SetControlScheme(controlScheme);
            Debug.Log("Now using PlayStation Controller");
        }

        if (inputActions.InputScheme.XboxInput.WasPressedThisFrame() && controlScheme != ControlScheme.XboxController)
        {
            controlScheme = ControlScheme.XboxController;
            GameManager.Instance.SetControlScheme(controlScheme);
            Debug.Log("Now using Xbox Controller");
        }

        if (inputActions.Player.Interact.WasPressedThisFrame())
        {
            InteractPressed?.Invoke();
            MenuClose?.Invoke();
        }
            

        if (inputActions.Player.Attack.WasPressedThisFrame())
        {
            AttackPressed?.Invoke();
        }
            

        if (inputActions.Player.SecondaryAttack.WasPressedThisFrame())
            SecondaryAttackPressed?.Invoke();

        if (inputActions.Player.Heal.WasPressedThisFrame())
        {
            Heal?.Invoke();
        }

        if (inputActions.Player.Evade.WasPressedThisFrame())
        {
            Dash?.Invoke();
            MenuConfirm?.Invoke();
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