using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.PlayerSettings;

public class Bumper : BaseObject
{
    // ------------------------------
    //
    //   A bumper that bumps players, enemies and objects away
    //
    //   Created: 19/07/2024
    //
    // ------------------------------

    // Public variables
    public float MaxBumpForce;

    public override void OnActivate()
    {
        base.OnActivate();
    }

    private void GeneralCollision(Collision2D collision)
    {
        // Check if any objects are touching the bumper, and try to push them away
        if (collision.gameObject.TryGetComponent(out PlayerGameplay player))
        {
            player.SetKnockback((Vector2)transform.position, gameObject, collision);
            player.stateMachine.ChangeState(typeof(PlayerKnockback));
        }
        else
        {
            collision.rigidbody.AddForce((collision.gameObject.transform.position - transform.position).normalized * MaxBumpForce);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.rigidbody != null)
        {
            GeneralCollision(collision);
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.rigidbody != null)
        {
            GeneralCollision(collision);
        }
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
    }
}
