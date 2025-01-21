using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OneWayDoor : BaseObject
{
    // ------------------------------
    //
    //   This door only lets players and objects go through one way
    //
    //   Created: 18/07/2024
    //
    // ------------------------------

    // Component references
    private Collider2D col;

    private void Awake()
    {
        col = GetComponent<Collider2D>();
    }

    public override void OnActivate()
    {
        base.OnActivate();
    }
    
    public override void OnDeactivate()
    {
        base.OnDeactivate();
    }
}
