using System;
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private InputActions inputActions;

    public Vector2 MoveInput { get; private set; }
    public Vector2 MousePosition { get; private set; }

    public event Action AttackPressed;

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
    }
}