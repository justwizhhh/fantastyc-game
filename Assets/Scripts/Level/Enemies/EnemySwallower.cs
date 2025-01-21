using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySwallower : BaseEnemyObject
{
    // ------------------------------
    //
    //   Projectiles that touch this enemy get swallowed, and a baby swallower egg gets spat out
    //
    //   Created: 26/07/2024
    //
    // ------------------------------

    // Public variables
    [Space(10)]
    public float MaxSpeed;
    public float EggSpawnTime;

    // Private variables
    private bool isSpawning;

    private List<PlayerGameplay> players = new List<PlayerGameplay>();
    private List<float> playerDistances = new List<float>();
    private Vector2 currentTarget;

    // Component references
    private ObjectPooler eggPool;
    private CircleCollider2D col;
    private Rigidbody2D rb;

    private void Awake()
    {
        eggPool = GetComponent<ObjectPooler>();
        col = GetComponent<CircleCollider2D>();
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
    }

    private IEnumerator SpawnEgg()
    {
        // The enemy pauses for a second to spawn the new baby egg object
        yield return new WaitForSeconds(EggSpawnTime);

        EnemySwallowerBaby baby = eggPool.SpawnObject(rb.position, Quaternion.identity).GetComponent<EnemySwallowerBaby>();
        baby.Spawn(this, players);

        isSpawning = false;
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

            // And then slowly glide towards that target
            rb.velocity = (currentTarget - rb.position).normalized * MaxSpeed;

            // Projectile detection and swallowing
            Collider2D[] colCheck = Physics2D.OverlapCircleAll(rb.position, col.radius);
            foreach (Collider2D hit in colCheck)
            {
                if (hit.gameObject.TryGetComponent(out ShooterProjectile projectile))
                {
                    if (!isSpawning)
                    {
                        projectile.gameObject.SetActive(false);
                        isSpawning = true;
                        StartCoroutine(SpawnEgg());

                        break;
                    }
                }
            }
        }
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        rb.velocity = Vector2.zero;
        StopAllCoroutines();
    }
}
