using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDead : PlayerState
{
    // ------------------------------
    //
    //   State for the player having been killed
    //
    //   Created: 24/05/2024
    //
    // ------------------------------

    public override void StartState()
    {
        if (player.currentFloor != LayerMask.NameToLayer("Pit"))
        {
            player.velocity = player.currentMoveInput * -player.DeathKnockbackSpeed;
            player.anim.SetTrigger("IsDead");
        }
        else
        {
            player.anim.SetTrigger("IsDeadInPit");
        }

        player.currentMoveInput = Vector2.zero;
        player.col.enabled = false;
    }

    public override void UpdateState()
    {
        // No inputs processed here
        player.currentAccel -= player.DeathKnockbackAccel;
        player.currentAccel = Mathf.Clamp01(player.currentAccel);
        player.velocity = Vector2.Lerp(Vector2.zero, player.prevMoveInput * -player.DeathKnockbackSpeed, player.currentAccel);
    }

    public override void FixedUpdateState()
    {

    }

    public override void EndState()
    {

    }
}
