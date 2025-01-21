using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPusher : BaseEnemyObject
{
    // ------------------------------
    //
    //   An enemy that pushes other objects around while they move
    //
    //   Created: 01/08/2024
    //
    // ------------------------------

    // Public variables
    [Space(10)]
    public float MoveSpeed;
    public float MoveTime;
    public float MovePauseTime;

    // Private variables
    private Vector2[] moveDirections = new Vector2[]
    {
        -Vector2.right,
        Vector2.right,
        Vector2.up,
        -Vector2.up
    };
    private Vector2 currentDir;

    // Component references
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnActivate()
    {
        base.OnActivate();
        StartCoroutine(ChangeMoveTarget());
    }

    private IEnumerator ChangeMoveTarget()
    {
        // Constantly assign a new place for the enemy to move to
        currentDir = Vector2.zero;
        yield return new WaitForSeconds(MovePauseTime);
        currentDir = moveDirections[Random.Range(0, moveDirections.Length)];
        yield return new WaitForSeconds(MoveTime);

        StopCoroutine(ChangeMoveTarget());
        StartCoroutine(ChangeMoveTarget());
    }

    private void FixedUpdate()
    {
        if (Active)
        {
            // Moving around
            if (currentDir != Vector2.zero)
            {
                rb.velocity = currentDir * MoveSpeed;
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
        }
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        StopAllCoroutines();
    }
}
