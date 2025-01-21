using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKnockback : PlayerState
{
    // ------------------------------
    //
    //   State for the player getting knocked back by something
    //
    //   Created: 19/05/2024
    //
    // ------------------------------

    // The "bounce" direction the player will be moving in to get away from the object/wall
    private Vector2 direction;

    public override void StartState()
    {
        RaycastHit2D[] bounceRayHits = Physics2D.RaycastAll(
            player.transform.position,
            (player.knockbackContact - (Vector2)player.transform.position).normalized);

        foreach (RaycastHit2D hit in bounceRayHits)
        {
            if (hit.collider.gameObject == player.knockbackSource)
            {
                direction = hit.normal;
                break;
            }
        }

        player.currentKnockbackTimer = player.KnockbackTime;

        // Snap the player's direction to have them look at the wall
        player.prevMoveInput = new Vector2(
            Mathf.Round(-direction.x),
            Mathf.Round(-direction.y));

        player.anim.SetBool("IsStunned", true);
    }

    public override void UpdateState()
    {
        // Only keep the player rolling for a set amount of time
        player.currentKnockbackTimer -= Time.deltaTime;
        if (player.currentKnockbackTimer <= 0)
        {
            if (player.currentFloor == LayerMask.NameToLayer("Pit"))
            {
                playerStateMachine.ChangeState(typeof(PlayerIdlePit));
            }
            else
            {
                playerStateMachine.ChangeState(typeof(PlayerIdle));
            }
        }

        // Check for harmful collisions
        player.CheckHarmfulCollision();
    }

    public override void FixedUpdateState()
    {
        // Apply a faster rolling velocity to the player's rigidbody
        player.velocity = direction * player.KnockbackSpeed;
    }

    public override void EndState()
    {
        player.velocity = Vector2.zero;
        player.currentAccel = 0;

        player.anim.SetBool("IsStunned", false);
    }
}
