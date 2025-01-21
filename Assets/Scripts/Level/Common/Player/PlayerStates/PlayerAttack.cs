using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : PlayerState
{
    // ------------------------------
    //
    //   State for the player swinging their weapon to attack in their current direction
    //
    //   Created: 28/05/2024
    //
    // ------------------------------

    private List<Collider2D> alreadyAttacked = new List<Collider2D>();

    public override void StartState()
    {
        player.currentAttackTimer = player.AttackTime;

        player.anim.SetBool("IsAttacking", true);
    }

    public override void UpdateState()
    {
        // Continue moving the player
        if (player.currentMoveInput != Vector2.zero)
        {
            player.prevMoveInput = player.currentMoveInput;
            player.currentAccel += player.MoveAccel;
        }
        else
        {
            player.currentAccel -= player.MoveAccel;
        }

        player.currentAccel = Mathf.Clamp01(player.currentAccel);
        player.velocity = Vector2.Lerp(Vector2.zero, player.prevMoveInput * player.MoveSpeed, player.currentAccel);

        // Check if they are walking over actual solid ground
        if (player.currentFloor == LayerMask.NameToLayer("Pit"))
        {
            if (player.currentMoveInput != Vector2.zero)
            {
                playerStateMachine.ChangeState(typeof(PlayerMovePit));
            }
            else
            {
                playerStateMachine.ChangeState(typeof(PlayerIdlePit));
            }
        }

        // Checking for attack swinging collisions
        AttackColCheck();

        // Tracking for how long the player swings for
        player.currentAttackTimer -= Time.deltaTime;
        if (player.currentAttackTimer <= 0)
        {
            FinishAttack();
        }

        // Check for harmful collisions
        player.CheckHarmfulCollision();
    }

    private void AttackColCheck()
    {
        // Get all of the objects that are getting attacked
        Collider2D[] attackCol = Physics2D.OverlapBoxAll(
            player.rb.position + (player.prevMoveInput * player.AttackDistance),
            new Vector2(player.col.radius * 2, player.col.radius * 2),
            0);

        foreach (Collider2D col in attackCol)
        {
            if (!alreadyAttacked.Contains(col))
            {
                switch (col.gameObject)
                {
                    // Knock other players around with your attack
                    case GameObject obj when col.gameObject.TryGetComponent(out PlayerGameplay otherPlayer):
                        if (otherPlayer != player)
                        {
                            if (otherPlayer.stateMachine.currentPlayerState.GetType() != typeof(PlayerKnockback)
                            && otherPlayer.stateMachine.currentPlayerState.GetType() != typeof(PlayerDead))
                            {
                                otherPlayer.knockbackContact = transform.position;
                                otherPlayer.knockbackSource = gameObject;
                                otherPlayer.stateMachine.ChangeState(typeof(PlayerKnockback));

                                alreadyAttacked.Add(col);
                            }
                        }
                        break;

                    // Deal damage to enemies if the player attacks them
                    case GameObject obj when col.gameObject.TryGetComponent(out BaseEnemyObject enemy):
                        if (enemy.MaxHealth > 0)
                        {
                            enemy.OnHurt(player);

                            alreadyAttacked.Add(col);
                        }
                        break;

                    // Misc. object interactions below
                    case GameObject obj when col.gameObject.TryGetComponent(out BouncyBall ball):
                        ball.OnAttack(player.rb.position);
                        alreadyAttacked.Add(col);
                        break;

                    case GameObject obj when col.gameObject.TryGetComponent(out Bush bush):
                        bush.OnAttack();
                        alreadyAttacked.Add(col);
                        break;
                }
            }
            else
            {
                continue;
            }
        }
    }

    private void FinishAttack()
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

    public override void EndState()
    {
        player.attackInput = false;
        alreadyAttacked.Clear();

        player.anim.SetBool("IsAttacking", false);
    }
}
