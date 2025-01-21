using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputSpreader : MonoBehaviour
{
    // ------------------------------
    //
    //   Takes all inputs processed in the 'PlayerInput' component, and spreads them to the player's gameplay and editor variants
    //
    //   Created: 10/07/2024
    //
    // ------------------------------

    // Public variables
    public bool Active;

    [Space(10)]
    public PlayerGameplay GameplayPlayer;
    public PlayerEditor EditorPlayer;

    // Input components
    private PlayerInput input;

    private void Awake()
    {
        input = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        EventManager.SwitchToEditing += EventManager_SwitchToEditing;
        EventManager.SwitchToGameplay += EventManager_SwitchToGameplay;
    }

    public void ProcessHeldInputs()
    {
        input.DeactivateInput();
        input.ActivateInput();
    }

    private void EventManager_SwitchToGameplay()
    {
        input.SwitchCurrentActionMap("GameplayMode");
    }

    private void EventManager_SwitchToEditing()
    {
        input.SwitchCurrentActionMap("EditingMode");
    }

    // Gameplay inputs
    public void Gameplay_Movement(InputAction.CallbackContext context)
    {
        if (Active) { GameplayPlayer.OnMovement(context); }
    }

    public void Gameplay_Attack(InputAction.CallbackContext context)
    {
        if (Active) { GameplayPlayer.OnAttack(context); }
    }

    public void Gameplay_Roll(InputAction.CallbackContext context)
    {
        if (Active) { GameplayPlayer.OnRoll(context); }
    }

    public void Gameplay_Die(InputAction.CallbackContext context)
    {
        if (Active) { GameplayPlayer.OnDie(context); }
    }

    public void Gameplay_Flag(InputAction.CallbackContext context)
    {
        GameplayPlayer.OnFlag(context);
    }

    // Editor inputs
    public void Editor_Movement(InputAction.CallbackContext context)
    {
        if (Active) { EditorPlayer.OnMovement(context); }
    }

    public void Editor_Select(InputAction.CallbackContext context)
    {
        if (Active) { EditorPlayer.OnSelect(context); }
    }

    public void Editor_Tweak(InputAction.CallbackContext context)
    {
        if (Active) { EditorPlayer.OnTweak(context); }
    }

    public void Editor_Rotate(InputAction.CallbackContext context)
    {
        if (Active) { EditorPlayer.OnRotate(context); }
    }

    private void OnDisable()
    {
        EventManager.SwitchToEditing -= EventManager_SwitchToEditing;
        EventManager.SwitchToGameplay -= EventManager_SwitchToGameplay;
    }
}
