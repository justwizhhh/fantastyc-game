using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shooter : BaseObject
{
    // ------------------------------
    //
    //   A simple version of the projectile shooting obstacles
    //
    //   Created: 08/07/2024
    //
    // ------------------------------

    // Public variables
    [Space(10)]
    public float ShootTime;

    // Component references
    private ObjectPooler projectilePool;

    private void Awake()
    {
        projectilePool = GetComponent<ObjectPooler>();
    }

    public override void OnActivate()
    {
        base.OnActivate();
        StartCoroutine(Shoot());
    }

    private IEnumerator Shoot()
    {
        projectilePool.SpawnObject(transform.position, transform.rotation);
        yield return new WaitForSeconds(ShootTime);

        StopCoroutine(Shoot());
        StartCoroutine(Shoot());
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        projectilePool.ClearAllObjects();
        StopAllCoroutines();
    }
}
