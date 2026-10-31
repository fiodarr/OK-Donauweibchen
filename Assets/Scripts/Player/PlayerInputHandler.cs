
using UnityEngine;

public class PlayerInputHandler : MonoBehaviour
{
    private InputActions inputActions;

    public Vector2 MoveInput { get; private set; }

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
    }
}
