using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PushBlock : BaseObject
{
    // ------------------------------
    //
    //   A large block that can slowly be pushed around by the player
    //
    //   Created: 18/07/2024
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
        rb.velocity = Vector2.zero;
    }

    private Vector2 SnapToNearestCardinal(Vector2 vector)
    {
        // Take the incoming vector, and snap/round it to the nearest cardinal (4-way) direction
        Vector2[] directions =
        {
            Vector2.up,
            Vector2.down,
            Vector2.left,
            Vector2.right,
        };
        float[] dotProducts =
        {
            Vector2.Dot(vector, Vector2.up),
            Vector2.Dot(vector, Vector2.down),
            Vector2.Dot(vector, Vector2.left),
            Vector2.Dot(vector, Vector2.right)
        };

        int biggestDot = 0;
        for (int i = 0; i < dotProducts.Length; i++)
        {
            if (dotProducts[i] > dotProducts[biggestDot])
            {
                biggestDot = i;
            }
            else
            {
                continue;
            }
        }

        return directions[biggestDot];
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Check if the player is trying to push the block
        if (Active)
        {
            PlayerGameplay player = collision.gameObject.GetComponent<PlayerGameplay>();
            if (player != null)
            {
                rb.velocity = (SnapToNearestCardinal((rb.transform.position - player.transform.position).normalized) * MaxPushSpeed) * Time.deltaTime;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        // Stop moving the block if the object has stopped pushing it
        PlayerGameplay player = collision.gameObject.GetComponent<PlayerGameplay>();
        if (player != null)
        {
            rb.velocity = Vector2.zero;
        }
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
    }
}
