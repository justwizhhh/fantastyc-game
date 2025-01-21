using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovePit : PlayerState
{
    // ------------------------------
    //
    //   State for the player running around while hovering above a pit
    //
    //   Created: 24/05/2024
    //
    // ------------------------------

    public override void StartState()
    {
        if (playerStateMachine.previousPlayerState.GetType() != typeof(PlayerIdlePit))
        {
            player.currentCoyoteTime = player.CoyoteTime;
        }

        player.anim.SetBool("IsHovering", true);
    }

    public override void UpdateState()
    {
        // Apply velocity to the player's rigidbody if input is being processed
        if (player.currentMoveInput != Vector2.zero)
        {
            player.prevMoveInput = player.currentMoveInput;
            player.currentAccel += player.MoveAccel;

            // Check if they are walking over actual solid ground
            if (player.currentFloor != LayerMask.NameToLayer("Pit"))
            {
                playerStateMachine.ChangeState(typeof(PlayerMove));
            }
        }
        else
        {
            playerStateMachine.ChangeState(typeof(PlayerIdlePit));
        }

        // Check for harmful collisions
        player.CheckHarmfulCollision();

        // Check if the player has been positioned above a pit for too long
        if (player.currentCoyoteTime <= 0)
        {
            playerStateMachine.ChangeState(typeof(PlayerDead));
            player.currentCoyoteTime = 0;
        }
    }

    public override void FixedUpdateState()
    {
        player.currentAccel = Mathf.Clamp01(player.currentAccel);
        player.velocity = Vector2.Lerp(Vector2.zero, player.prevMoveInput * player.MoveSpeed, player.currentAccel);
    }

    public override void EndState()
    {
        player.rollInput = false;

        player.anim.SetBool("IsHovering", false);
    }
}
