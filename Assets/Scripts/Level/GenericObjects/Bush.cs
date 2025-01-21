using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bush : BaseObject
{
    // ------------------------------
    //
    //   A bush that can be destroyed by the player, but also regrow
    //
    //   Created: 19/07/2024
    //
    // ------------------------------

    // Component references
    private SpriteRenderer sr;
    private Collider2D col;

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
    }

    public override void OnActivate()
    {
        base.OnActivate();
    }

    public void OnAttack()
    {
        sr.enabled = false;
        col.enabled = false;
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        sr.enabled = true;
        col.enabled = true;
    }
}
