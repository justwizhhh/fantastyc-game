using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningBoard : BaseObject
{
    // ------------------------------
    //
    //   An object that slowly spins around. That's it
    //
    //   Created: 09/07/2024
    //
    // ------------------------------

    // Public variables
    [Space(10)]
    public float SpinDirection;
    public float MaxSpinSpeed;

    // Private variables
    private Quaternion startRot = Quaternion.identity;

    // Component references
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnActivate()
    {
        base.OnActivate();

        if (startRot == Quaternion.identity) 
        {
            startRot = transform.rotation;
        }
        else
        {
            transform.rotation = startRot;
        }
    }

    private void FixedUpdate()
    {
        if (Active)
        {
            rb.MoveRotation(rb.rotation + (MaxSpinSpeed * SpinDirection));
        }
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();

        transform.rotation = startRot;
    }
}
