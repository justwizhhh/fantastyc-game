using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EnemySwarmer : BaseEnemyObject
{
    // ------------------------------
    //
    //   The flies that the swarmer egg spawns. They move around frantically and in random directions
    //
    //   Created: 29/07/2024
    //
    // ------------------------------

    // Public variables
    [Space(10)]
    public float MoveSpeed;
    public float MaxMoveDistance;

    // Private variables
    private EnemySwarmerEgg sourceEgg;
    private Vector2 currentMoveTarget;

    // Component references
    private CircleCollider2D col;
    private Rigidbody2D rb;

    private void Awake()
    {
        col = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    public void Spawn(EnemySwarmerEgg newSourceEgg)
    {
        if (sourceEgg == null)
        {
            sourceEgg = newSourceEgg;
        }

        rb.simulated = true;
        StartCoroutine(ChangeMoveTarget());
    }

    private IEnumerator ChangeMoveTarget()
    {
        // Constantly assign a new place for the enemy to move to
        currentMoveTarget = rb.position 
            + new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized 
            * Random.Range(0f, MaxMoveDistance);

        yield return null;
        StopCoroutine(ChangeMoveTarget());
    }

    private void FixedUpdate()
    {
        if (sourceEgg != null && sourceEgg.Active)
        {
            // Frantically moving around
            if (currentMoveTarget != Vector2.zero)
            {
                if (rb.position != currentMoveTarget)
                {
                    rb.position = Vector2.MoveTowards(rb.position, currentMoveTarget, MoveSpeed);
                }
                else
                {
                    StartCoroutine(ChangeMoveTarget());
                }
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out EnemySwarmer fly))
        {
            Physics2D.IgnoreCollision(col, fly.GetComponent<CircleCollider2D>());
        }
    }
}
