using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWanderer : BaseEnemyObject
{
    // ------------------------------
    //
    //   A simple enemy that just wanders around
    //
    //   Created: 22/07/2024
    //
    // ------------------------------

    // Public variables
    public float MoveSpeed;
    public Vector2 StartDirection;

    // Private variables
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
        if (currentDir == Vector2.zero)
        {
            currentDir = StartDirection.normalized;
        }
    }

    private void FixedUpdate()
    {
        if (Active)
        {
            rb.velocity = currentDir * MoveSpeed;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Foreground"))
        {
            currentDir *= -1;
        }
    }
}
