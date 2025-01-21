using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerState : MonoBehaviour
{
    // ------------------------------
    //
    //   Base template class for the different states the player can be (in gameplay mode)
    //
    //   Created: 19/05/2024
    //
    // ------------------------------

    [HideInInspector] public bool isActive = false;

    protected PlayerStateMachine playerStateMachine;
    protected PlayerGameplay player;

    private void Awake()
    {
        playerStateMachine = GetComponent<PlayerStateMachine>();
        player = GetComponent<PlayerGameplay>();
    }

    public virtual void StartState()
    {

    }

    public virtual void UpdateState()
    {

    }

    public virtual void FixedUpdateState()
    {

    }

    public virtual void EndState()
    {

    }
}
