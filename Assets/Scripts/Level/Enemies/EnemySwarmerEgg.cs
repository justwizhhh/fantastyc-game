using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySwarmerEgg : BaseObject
{
    // ------------------------------
    //
    //   When placed, this egg will spawn lots of small swarmer flies that haphazardly move around the level
    //
    //   Created: 29/07/2024
    //
    // ------------------------------

    // Public variables
    [Space(10)]
    public float CrackTime;
    public float MaxSpawnOffset;
    public float MinSpawnOffset;

    // Private variables
    private bool isCracked;

    // Component references
    private ObjectPooler flyPool;

    private SpriteRenderer sr;
    private CircleCollider2D col;

    private void Awake()
    {
        flyPool = GetComponent<ObjectPooler>();
        sr = GetComponent<SpriteRenderer>();
        col = GetComponent<CircleCollider2D>();
    }

    public override void OnActivate()
    {
        base.OnActivate();
        if (!isCracked)
        {
            StartCoroutine(StartCracking());
        }
    }

    private IEnumerator StartCracking()
    {
        yield return new WaitForSeconds(CrackTime);
        for (int i = 0; i < flyPool.ObjectCount; i++)
        {
            GameObject newFly = flyPool.SpawnObject(
                transform.position + new Vector3(Random.Range(-1f, 1f), Random.Range(-1f, 1f), 0).normalized 
                    * Random.Range(MinSpawnOffset, MaxSpawnOffset), 
                Quaternion.identity);

            newFly.GetComponent<EnemySwarmer>().Spawn(this);
        }

        isCracked = true;
        sr.enabled = false;
        col.enabled = false;
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        StopAllCoroutines();
        if (flyPool.SpawnedObjectCount <= 0)
        {
            Deleted = true;
        }
    }
}
