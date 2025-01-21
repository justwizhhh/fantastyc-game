using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : PlayerState
{
    // ------------------------------
    //
    //   State for the player running around
    //
    //   Created: 19/05/2024
    //
    // ------------------------------

    public override void StartState()
    {
        player.anim.SetBool("IsMoving", true);
    }

    public override void UpdateState()
    {
        // Apply velocity to the player's rigidbody if input is being processed
        if (player.currentMoveInput != Vector2.zero)
        {
            if (player.rollInput && player.currentRollDelayTimer <= 0)
            {
                playerStateMachine.ChangeState(typeof(PlayerRoll));
            }
            else
            {
                player.rollInput = false;
                
                player.prevMoveInput = player.currentMoveInput;
                player.currentAccel += player.MoveAccel;
            }
        }
        else
        {
            playerStateMachine.ChangeState(typeof(PlayerIdle));
        }

        // Check if the player is trying to attack
        if (player.attackInput && player.currentAttackDelayTimer <= 0)
        {
            playerStateMachine.ChangeState(typeof(PlayerAttack));
        }

        // Check for harmful collisions
        player.CheckHarmfulCollision();

        // Check if they are walking over a pit
        if (player.currentFloor == LayerMask.NameToLayer("Pit"))
        {
            playerStateMachine.ChangeState(typeof(PlayerMovePit));
        }
    }

    public override void FixedUpdateState()
    {
        player.currentAccel = Mathf.Clamp01(player.currentAccel);
        player.velocity = Vector2.Lerp(Vector2.zero, player.prevMoveInput * player.MoveSpeed, player.currentAccel);
    }
}
