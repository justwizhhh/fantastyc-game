using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlternatingShooter : BaseObject
{
    // ------------------------------
    //
    //   This container object consists of two shooters that shoot fowards and backwards
    //
    //   Created: 30/07/2024
    //
    // ------------------------------

    // Private variables
    private RotatingShooter[] childShooters;

    private void Awake()
    {
        childShooters = GetComponentsInChildren<RotatingShooter>();
    }

    public override void OnActivate()
    {
        base.OnActivate();
        foreach (RotatingShooter shooter in childShooters)
        {
            shooter.Active = true;
            shooter.OnActivate();
        }
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        foreach (RotatingShooter shooter in childShooters)
        {
            shooter.Active = false;
            shooter.OnDeactivate();
        }
    }
}
