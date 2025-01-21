using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseObject : MonoBehaviour
{
    // ------------------------------
    //
    //   Base object for all interactive gameplay objects
    //   The rest of the object logic only gets instantiated once Gameplay Mode is initiated
    //
    //   Created: 15/06/2024
    //
    // ------------------------------

    // Public variables
    [Header("Base Object Settings")]
    public bool Active = false;
    public bool Animated = false;
    public bool Deleted = false;

    private void Start()
    {
        if (Active)
        {
            OnActivate();
        }
    }

    public virtual void OnActivate() 
    {
        Active = true;
        Animated = true;
    }

    public virtual void OnDeactivate()
    {
        Active = false;
        Animated = false;
    }
}
