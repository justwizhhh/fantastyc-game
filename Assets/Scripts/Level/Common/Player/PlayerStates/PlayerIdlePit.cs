using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdlePit : PlayerState
{
    // ------------------------------
    //
    //   State for the player standing still while above a pit
    //
    //   Created: 24/05/2024
    //
    // ------------------------------

    public override void StartState()
    {
        if (playerStateMachine.previousPlayerState.GetType() != typeof(PlayerMovePit))
        {
            player.currentCoyoteTime = player.CoyoteTime;
        }

        player.anim.SetBool("IsHovering", true);
    }

    public override void UpdateState()
    {
        // If the player is not moving, slowly set their velocity to zero and stop them
        if (player.currentMoveInput == Vector2.zero)
        {
            player.currentAccel -= player.MoveAccel;
        }
        else
        {
            playerStateMachine.ChangeState(typeof(PlayerMovePit));
        }

        // Check for harmful collisions
        player.CheckHarmfulCollision();

        player.currentAccel = Mathf.Clamp01(player.currentAccel);
        player.velocity = Vector2.Lerp(Vector2.zero, player.prevMoveInput * player.MoveSpeed, player.currentAccel);

        // Check if they are walking over actual solid ground
        if (player.currentFloor != LayerMask.NameToLayer("Pit"))
        {
            playerStateMachine.ChangeState(typeof(PlayerIdle));
        }

        // Check if the player has been positioned above a pit for too long
        if (player.currentCoyoteTime <= 0)
        {
            playerStateMachine.ChangeState(typeof(PlayerDead));
            player.currentCoyoteTime = 0;
        }
    }

    public override void FixedUpdateState()
    {

    }

    public override void EndState()
    {
        player.anim.SetBool("IsHovering", false);
    }
}
