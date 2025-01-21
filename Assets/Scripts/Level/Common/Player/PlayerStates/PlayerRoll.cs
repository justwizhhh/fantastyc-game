using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRoll : PlayerState
{
    // ------------------------------
    //
    //   State for the player rolling around, with movement input being locked
    //
    //   Created: 19/05/2024
    //
    // ------------------------------

    public override void StartState()
    {
        player.currentRollTimer = player.RollTime;

        player.anim.SetBool("IsRolling", true);
    }

    public override void UpdateState()
    {
        // Only keep the player rolling for a set amount of time
        player.currentRollTimer -= Time.deltaTime;
        if (player.currentRollTimer <= 0)
        {
            if (player.currentMoveInput != Vector2.zero)
            {
                playerStateMachine.ChangeState(typeof(PlayerMove));
            }
            else
            {
                playerStateMachine.ChangeState(typeof(PlayerIdle));
            }
        }

        // Cancel the roll if they are just about to enter a pit
        if (player.currentFloor == LayerMask.NameToLayer("Pit"))
        {
            playerStateMachine.ChangeState(typeof(PlayerIdlePit));
        }

        // Check for harmful collisions
        player.CheckHarmfulCollision();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Collision with walls stops the roll
        if (isActive)
        {
            if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Foreground"))
            {
                Debug.Log(Vector2.Dot(player.currentMoveInput, collision.GetContact(0).normal));
                if (Vector2.Dot(player.currentMoveInput, collision.GetContact(0).normal) < 0) 
                {
                    player.SetKnockback(collision.GetContact(0).point, collision.gameObject, collision);
                    playerStateMachine.ChangeState(typeof(PlayerKnockback));
                }
            }
            else if (collision.gameObject.GetComponent<PlatformEffector2D>() == null)
            {
                playerStateMachine.ChangeState(typeof(PlayerIdle));
            }
        }
    }

    public override void FixedUpdateState()
    {
        // Apply a faster rolling velocity to the player's rigidbody
        player.velocity = player.prevMoveInput * player.RollSpeed;
    }

    public override void EndState()
    {
        player.rollInput = false;
        player.currentRollDelayTimer = player.RollDelayTime;

        player.anim.SetBool("IsRolling", false);
    }
}
