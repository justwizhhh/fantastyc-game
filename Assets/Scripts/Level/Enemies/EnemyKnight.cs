using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class EnemyKnight : BaseEnemyObject
{
    // ------------------------------
    //
    //   A patrolling knight enemy, that moves along a set path, but gives chase to players if they get too close
    //
    //   Created: 01/08/2024
    //
    // ------------------------------

    // Public variables
    [Space(10)]
    public float PatrolMoveSpeed;
    public Vector2 PatrolStartDirection;
    public float PatrolMaxDistance;
    public float PatrolWaitTime;

    [Space(10)]
    public float ChaseMoveSpeed;
    public float ChaseMaxTime;
    public float ChaseMaxDistance;
    public float ChaseDelayTime;

    [Space(10)]
    public float KnockbackSpeed;
    public float KnockbackTime;

    // Private variables
    private Vector2 currentDir;
    private Transform currentTarget;
    private Transform knockbackSource;

    private enum States
    {
        Patrolling,
        Chasing,
        Waiting,
        Stunned
    };
    private States currentState;

    // Component references
    private Rigidbody2D rb;
    private Collider2D col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        col = rb.GetComponent<Collider2D>();
    }

    private void Start()
    {
        if (Active)
        {
            OnActivate();
        }
    }

    public override void OnActivate()
    {
        base.OnActivate();
        if (currentDir == Vector2.zero)
        {
            currentDir = PatrolStartDirection.normalized;
        }
        currentState = States.Patrolling;
    }

    private IEnumerator StartChase()
    {
        currentState = States.Waiting;
        yield return new WaitForSeconds(ChaseDelayTime);
        currentState = States.Chasing;

        StopCoroutine(StartChase());
    }

    private IEnumerator Chase()
    {
        yield return new WaitForSeconds(ChaseMaxTime);
        StartCoroutine(EndChase());

        StopCoroutine(Chase());
    }

    private IEnumerator EndChase()
    {
        currentState = States.Waiting;
        yield return new WaitForSeconds(ChaseDelayTime);
        currentState = States.Patrolling;

        StopCoroutine(EndChase());
    }

    private void FixedUpdate()
    {
        // Moving the knight around the scene
        switch (currentState)
        {
            default:
            case States.Waiting:
                rb.velocity = Vector2.zero;
                break;

            case States.Patrolling:
                rb.velocity = currentDir * PatrolMoveSpeed;
                break;

            case States.Chasing:
                rb.velocity = ((Vector2)currentTarget.position - rb.position).normalized * ChaseMoveSpeed;
                break;

            case States.Stunned:
                rb.velocity = (rb.position - (Vector2)knockbackSource.position).normalized * KnockbackSpeed;
                break;
        }
    }

    private void Update()
    {
        // Checking if the player is getting too close / too far away
        if (Active)
        {
            switch (currentState)
            {
                case States.Patrolling:
                    Collider2D[] playerPatrolCheck = Physics2D.OverlapCircleAll(rb.position, PatrolMaxDistance);
                    foreach (Collider2D col in playerPatrolCheck)
                    {
                        if (col.gameObject.TryGetComponent(out PlayerGameplay player))
                        {
                            if (player.stateMachine.currentPlayerState.GetType() != typeof(PlayerDead))
                            {
                                currentTarget = player.transform;
                                StartCoroutine(StartChase());
                            }
                        }
                    }
                    break;

                case States.Chasing:
                    // If the player gets too far away, the knight just gives up
                    if (Vector2.Distance(rb.position, currentTarget.position) > ChaseMaxDistance)
                    {
                        StopCoroutine(Chase());
                        StartCoroutine(EndChase());
                    }
                    break;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (currentState != States.Chasing)
        {
            if (collision.gameObject.layer == LayerMask.NameToLayer("Foreground"))
            {
                currentDir *= -1;
            }
            else
            {
                Physics2D.IgnoreCollision(col, collision.collider);
            }
        }
    }

    private IEnumerator HurtKnockback()
    {
        States prevState = currentState;
        currentState = States.Stunned;
        yield return new WaitForSeconds(KnockbackTime);
        currentState = prevState;

        StopCoroutine(HurtKnockback());
    }

    public override void OnHurt(PlayerGameplay player)
    {
        if (currentState != States.Stunned)
        {
            base.OnHurt(player);

            if (health > 0)
            {
                knockbackSource = player.transform;
                StartCoroutine(HurtKnockback());
            }
        }
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        StopAllCoroutines();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, PatrolMaxDistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, ChaseMaxDistance);
    }
}
