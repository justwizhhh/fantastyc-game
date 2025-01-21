using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdle : PlayerState
{
    // ------------------------------
    //
    //   State for the player standing still
    //
    //   Created: 19/05/2024
    //
    // ------------------------------

    public override void StartState()
    {
        player.anim.SetBool("IsMoving", false);
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
            playerStateMachine.ChangeState(typeof(PlayerMove));
        }

        // Check if the player is trying to attack
        if (player.attackInput && player.currentAttackDelayTimer <= 0)
        {
            playerStateMachine.ChangeState(typeof(PlayerAttack));
        }

        // Check for harmful collisions
        player.CheckHarmfulCollision();

        // Check if they are walking over actual solid ground
        if (player.currentFloor == LayerMask.NameToLayer("Pit"))
        {
            playerStateMachine.ChangeState(typeof(PlayerIdlePit));
        }

        player.currentAccel = Mathf.Clamp01(player.currentAccel);
        player.velocity = Vector2.Lerp(Vector2.zero, player.prevMoveInput * player.MoveSpeed, player.currentAccel);
    }

    public override void FixedUpdateState()
    {

    }

    public override void EndState()
    {
        player.rollInput = false;
    }
}
