using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBeehive : BaseEnemyObject
{
    // ------------------------------
    //
    //   Get too close to this enemy, and they'll summon a swarm of chasing bees!
    //
    //   Created: 24/07/2024
    //
    // ------------------------------

    // Public variables
    [Space(10)]
    public float BeeSpawnRadius;
    public float BeeSpawnDelay;
    public float BeeSpawnCooldown;

    // Private variables
    private bool isSpawningBee;

    // Component references
    private ObjectPooler beePool;
    private Rigidbody2D rb;

    private void Awake()
    {
        beePool = GetComponent<ObjectPooler>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (Active)
        {
            OnActivate();
        }
    }

    private IEnumerator SpawnBee(Transform target)
    {
        // Spawn in new projectile, and assign the player as its permanent target
        yield return new WaitForSeconds(BeeSpawnDelay);

        EnemyBeehiveBee newBee = beePool.SpawnObject(rb.position, Quaternion.identity).GetComponent<EnemyBeehiveBee>();
        newBee.AssignTarget(target);

        StartCoroutine(SpawnCooldown());
    }

    private IEnumerator SpawnCooldown()
    {
        yield return new WaitForSeconds(BeeSpawnCooldown);
        isSpawningBee = false;
    }

    private void Update()
    {
        // Spawn a homing bee projectile if the player comes near
        if (Active)
        {
            Collider2D[] radiusCheck = Physics2D.OverlapCircleAll(rb.position, BeeSpawnRadius);
            foreach (Collider2D hit in radiusCheck)
            {
                if (hit.GetComponent<PlayerGameplay>() != null)
                {
                    // If there are no more projectiles in the pool, then no more bees need to be spawned
                    if (beePool.SpawnedObjectCount < beePool.ObjectCount)
                    {
                        if (!isSpawningBee)
                        {
                            StartCoroutine(SpawnBee(hit.transform));
                            isSpawningBee = true;
                            break;
                        }
                        else
                        {
                            continue;
                        }
                    }
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, BeeSpawnRadius);
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        beePool.ClearAllObjects();
        StopAllCoroutines();
    }
}
