using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BouncyBall : BaseObject
{
    // ------------------------------
    //
    //   A ball that can be pushed, launched, and bounced around with ease 
    //
    //   Created: 19/07/2024
    //
    // ------------------------------

    // Public variables
    public float MaxPushSpeed;

    // Component references
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnActivate()
    {
        base.OnActivate();
        rb.velocity = Vector3.zero;
    }

    public void OnAttack(Vector3 playerPos)
    {
        // Apply sudden velocity if the player is attacking the ball
        if (Active)
        {
            rb.velocity = (rb.transform.position - playerPos).normalized * MaxPushSpeed;
        }
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        rb.velocity = Vector2.zero;
    }
}
