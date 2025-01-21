using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseEnemyObject : BaseObject
{
    // ------------------------------
    //
    //   Base class for all enemies that players can place in the game
    //
    //   Created: 08/07/2024
    //
    // ------------------------------

    // Public variables
    [Space(20)]
    [Header("Enemy Settings")]
    public int MaxHealth;

    protected int health;

    private void Start()
    {
        health = MaxHealth;
    }

    public override void OnActivate()
    {
        base.OnActivate();
    }

    public virtual void OnHurt(PlayerGameplay player) 
    {
        health--;
        if (health <= 0)
        {
            OnDeath();
        }
    }

    public virtual void OnDeath() 
    {
        gameObject.SetActive(false);
        Deleted = true;
    }
}
