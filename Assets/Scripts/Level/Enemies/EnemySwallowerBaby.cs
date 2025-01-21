using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySwallowerBaby : BaseEnemyObject
{
    // ------------------------------
    //
    //   The baby version of the swallower enemy. These guys don't swallow projectiles!
    //
    //   Created: 26/07/2024
    //
    // ------------------------------

    // Public variables
    [Space(10)]
    public float MaxSpeed;
    [Space(10)]
    public float MaxEggSpeed;
    public float EggAccel;
    public float EggCrackTime;

    // Private variables
    private bool isInsideEgg;
    private EnemySwallower parent;

    private List<PlayerGameplay> players = new List<PlayerGameplay>();
    private List<float> playerDistances = new List<float>();
    private Vector2 currentTarget;

    // Component references
    private CircleCollider2D col;
    private Rigidbody2D rb;

    private void Awake()
    {
        col = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    private IEnumerator ComeOutOfEgg()
    {
        yield return new WaitForSeconds(EggCrackTime);
        isInsideEgg = false;
    }

    public void Spawn(EnemySwallower newParent, List<PlayerGameplay> newPlayers)
    {
        // Give the baby object information about who spawned them in, without searching the entire scene for the same objects again
        if (parent == null)
        {
            parent = newParent;
        }
        
        if (players.Count == 0)
        {
            players = newPlayers;
            for (int i = 0; i < players.Count; i++)
            {
                playerDistances.Add(0);
            }
        }

        // Put the enemy in a temporary immobile egg state and push them away from their "parent"
        isInsideEgg = true;
        rb.velocity = new Vector2(UnityEngine.Random.Range(-1, 1), UnityEngine.Random.Range(-1, 1)).normalized * MaxEggSpeed;
        StartCoroutine(ComeOutOfEgg());
    }

    private void FixedUpdate()
    {
        if (parent != null && parent.Active)
        {
            if (!isInsideEgg)
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

                // And then slowly glide towards that target
                rb.velocity = (currentTarget - rb.position).normalized * MaxSpeed;
            }
            else
            {
                rb.velocity = Vector2.Lerp(rb.velocity, Vector2.zero, EggAccel);
            }
        }
        else
        {
            rb.velocity = Vector2.zero;
        }
    }
}
