using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BananaPeel : BaseObject
{
    // ------------------------------
    //
    //   This slips up players (and enemies) upon contact
    //
    //   Created: 08/07/2024
    //
    // ------------------------------

    public override void OnActivate()
    {
        base.OnActivate();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player is walking over the banana peel
        PlayerGameplay player = collision.gameObject.GetComponent<PlayerGameplay>();
        if (player != null)
        {
            player.stateMachine.ChangeState(typeof(PlayerSlip));
            gameObject.SetActive(false);
            Deleted = false;
        }

        // Check if the enemy is walking over it (WIP)
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
    }
}
