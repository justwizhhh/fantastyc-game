using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWin : PlayerState
{
    // ------------------------------
    //
    //   State for the player having reached the end of the current level
    //
    //   Created: 04/06/2024
    //
    // ------------------------------

    public override void StartState()
    {
        player.prevMoveInput = Vector2.down;
        player.currentMoveInput = Vector2.down;
        player.velocity = Vector2.zero;

        player.isMoving = false;
        player.attackInput = false;
        player.rollInput = false;

        player.anim.SetTrigger("Win");
    }

    public override void UpdateState()
    {
        // No inputs processed here
    }

    public override void FixedUpdateState()
    {

    }

    public override void EndState()
    {

    }
}
