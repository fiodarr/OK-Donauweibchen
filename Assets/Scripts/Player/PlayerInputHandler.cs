using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private InputActions inputActions;

    public Vector2 MoveInput { get; private set; }
    public Vector2 MousePosition { get; private set; }
    public float FishingRodTimePressed { get; private set; }
    
    public event Action AttackPressed;
    public event Action UtilityPressed;
    public event Action UtilityReleased;
    
    private bool fishingRodPressed = false;
    
    private void Awake()
    {
        inputActions = new InputActions();
    }

    private void OnEnable()
    {
        inputActions.Player.Enable();
    }

    private void OnDisable()
    {
        inputActions.Player.Disable();
    }

    private void Update()
    {
        MoveInput = inputActions.Player.Move.ReadValue<Vector2>();
        MousePosition = inputActions.Player.Look.ReadValue<Vector2>();

        if (inputActions.Player.Attack.WasPressedThisFrame())
            AttackPressed?.Invoke();
        
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
}