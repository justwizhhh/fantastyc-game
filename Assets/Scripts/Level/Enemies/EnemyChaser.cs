using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyChaser : BaseEnemyObject
{
    // ------------------------------
    //
    //   Chases the nearest player (or tries its best to) until it dies 
    //
    //   Created: 08/07/2024
    //
    // ------------------------------

    // Public variables
    [Space(10)]
    public float MaxSpeed;
    public float MaxAccel;

    // Private variables
    private Vector2 startPos;
    private Vector2 currentVelocity;

    private List<PlayerGameplay> players = new List<PlayerGameplay>();
    private List<float> playerDistances = new List<float>();
    private Vector2 currentTarget;

    // Component references
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        // Find all of the players that we are gonna target
        players.AddRange(FindObjectsByType<PlayerGameplay>(FindObjectsInactive.Include, FindObjectsSortMode.None));
        for (int i = 0; i < players.Count; i++)
        {
            playerDistances.Add(0);
        }
    }

    public override void OnActivate()
    {
        base.OnActivate();
        startPos = rb.position;
        currentVelocity = Vector2.zero;
    }

    private void FixedUpdate()
    {
        if (Active)
        {
            // Constantly monitor how far away each player in the level is, and target the closest one
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].stateMachine.currentPlayerState.GetType() != typeof(PlayerDead))
                {
                    playerDistances[i] = (players[i].rb.position - rb.position).sqrMagnitude;
                }
                else
                {
                    playerDistances[i] = Mathf.Infinity;
                }
            }

            // Assign that as the chase target
            currentTarget = players[Array.IndexOf(playerDistances.ToArray(), playerDistances.Min())].rb.position;

            // And them move towards that target
            currentVelocity = Vector2.ClampMagnitude(currentVelocity + ((currentTarget - rb.position) * MaxAccel), MaxSpeed);
            rb.position += currentVelocity;
        }
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        currentVelocity = Vector2.zero;
    }
}
