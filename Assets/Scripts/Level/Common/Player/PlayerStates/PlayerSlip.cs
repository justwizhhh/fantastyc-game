using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSlip : PlayerState
{
    // ------------------------------
    //
    //   State for the player slipping on a banana peel, or similar surface
    //
    //   Created: 18/07/2024
    //
    // ------------------------------

    private Vector2 startVelocity;
    private float currentAccel;

    public override void StartState()
    {
        startVelocity = player.prevMoveInput * player.SlipSpeed;
        player.velocity = startVelocity;
        currentAccel = 0;

        player.anim.SetBool("IsHovering", true);
    }

    public override void UpdateState()
    {
        if (currentAccel < 1)
        {
            // Cancel the slip animation if they are just about to enter a pit
            if (player.currentFloor == LayerMask.NameToLayer("Pit"))
            {
                playerStateMachine.ChangeState(typeof(PlayerIdlePit));
            }
        }
        else
        {
            playerStateMachine.ChangeState(typeof(PlayerIdle));
        }
    }

    public override void FixedUpdateState()
    {
        if (currentAccel < 1)
        {
            player.velocity = Vector2.Lerp(startVelocity, Vector2.zero, currentAccel);
            currentAccel += player.SlipAccel;
        }
    }

    public override void EndState()
    {
        player.velocity = Vector2.zero;
        player.currentAccel = 0;

        player.anim.SetBool("IsHovering", false);
    }
}
