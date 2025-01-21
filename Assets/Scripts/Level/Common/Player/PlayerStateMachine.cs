using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerStateMachine : MonoBehaviour
{
    // ------------------------------
    //
    //   Class for sorting through and updating what state the player is currently in
    //
    //   Created: 19/05/2024
    //
    // ------------------------------

    [SerializeField] private List<PlayerState> PlayerStates = new List<PlayerState>();
    public PlayerState previousPlayerState;
    public PlayerState currentPlayerState;

    // Private variables
    private PlayerGameplay player;

    private void Awake()
    {
        player = GetComponent<PlayerGameplay>();
    }

    private void Start()
    {
        previousPlayerState = PlayerStates[0];
        currentPlayerState = PlayerStates[0];
        currentPlayerState.isActive = true;
    }

    private void FixedUpdate()
    {
        currentPlayerState.FixedUpdateState();
    }

    private void Update()
    {
        currentPlayerState.UpdateState();
    }

    public void ChangeState(Type newState)
    {
        if (currentPlayerState != null)
        {
            currentPlayerState.EndState();
            currentPlayerState.isActive = false;
        }
        previousPlayerState = currentPlayerState;
        currentPlayerState = PlayerStates.Find(x => x.GetType() == newState);
        currentPlayerState.StartState();
        currentPlayerState.isActive = true;
    }
}
