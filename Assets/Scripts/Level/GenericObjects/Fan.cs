using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan : BaseObject
{
    // ------------------------------
    //
    //   A fan which blows away all other objects away
    //
    //   Created: 19/07/2024
    //
    // ------------------------------

    // Public variables
    public float MaxFanPower;

    // Component references
    private BoxCollider2D col;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
    }

    public override void OnActivate()
    {
        base.OnActivate();
    }

    private void FixedUpdate()
    {
        if (Active)
        {
            RaycastHit2D[] colCheck = Physics2D.BoxCastAll(transform.position, col.size, 0, (Vector2)transform.right);
            foreach (RaycastHit2D col in colCheck)
            {
                if (col.rigidbody != null)
                {
                    if (col.rigidbody.bodyType == RigidbodyType2D.Dynamic)
                    {
                        col.rigidbody.AddForce(transform.right * MaxFanPower);
                    }
                    else
                    {
                        continue;
                    }
                }
                else
                {
                    continue;
                }
            }
        }
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
    }
}
