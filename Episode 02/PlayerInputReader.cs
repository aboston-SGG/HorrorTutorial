using UnityEngine;
using System;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "Input/Player Input Reader")]
public class PlayerInputReader : ScriptableObject
{
    public event Action<Vector2> MoveEvent;

    private InputSystem_Actions actions;

    // EnableInput and DisableInput methods are used to manage the input actions for the player. They ensure that the input system is properly initialized and cleaned up when the scriptable object is enabled or disabled.
    public void EnableInput()
    {
        if (actions == null)
        {
            actions = new InputSystem_Actions();
            actions.Player.Move.performed += OnMove;
            actions.Player.Move.canceled += OnMove;
        }

        actions.Player.Enable();
    }

    public void DisableInput()
    {
        if (actions == null) return;
        actions.Player.Disable();
    }

    // OnDisable is called when the scriptable object is disabled. It ensures that the input actions are properly cleaned up and unsubscribed from events to prevent memory leaks or unexpected behavior.
    private void OnDisable()
    {
        if (actions == null) return;

        DisableInput();
        actions.Player.Move.performed -= OnMove;
        actions.Player.Move.canceled -= OnMove;
        actions.Dispose();
        actions = null;
    }

    // OnMove is a callback method that is invoked when the player performs or cancels a move action. It reads the input value and invokes the MoveEvent with the input vector, allowing other scripts to respond to player movement.
    private void OnMove(InputAction.CallbackContext context)
    {
        Vector2 input = context.ReadValue<Vector2>();
        MoveEvent?.Invoke(input);
    }
}
