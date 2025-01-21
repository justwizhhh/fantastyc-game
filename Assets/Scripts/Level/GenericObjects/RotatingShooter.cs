using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotatingShooter : BaseObject
{
    // ------------------------------
    //
    //   A customisable, rotating version of the shooter object
    //
    //   Created: 30/07/2024
    //
    // ------------------------------

    // Public variables
    [Space(10)]
    public float[] ShootAngles;

    [Space(10)]
    public float PauseTime;
    public float DelayTime;
    public float RotationAmount;
    public float RotationSpeed;

    // Private variables
    private bool isRotating;
    private Vector3 originalRotation;
    private float rotationTarget;

    // Component references
    private ObjectPooler projectilePool;

    private void Awake()
    {
        projectilePool = GetComponent<ObjectPooler>();
    }

    private void Start()
    {
        originalRotation = transform.localEulerAngles;
    }

    public override void OnActivate()
    {
        base.OnActivate();
        rotationTarget = originalRotation.z + RotationAmount;
        if (rotationTarget >= 360)
        {
            rotationTarget -= 360;
        }
        StartCoroutine(StartShoot());
    }

    private IEnumerator StartShoot()
    {
        // If delay time has been specified, wait for it to be over before shooting begins
        yield return new WaitForSeconds(DelayTime);
        StopCoroutine(StartShoot());
        StartCoroutine(Shoot());
    }

    private IEnumerator Shoot()
    {
        // Shoot all of the projectiles in the specified directions in 'ShootAngles'
        foreach (float angle in  ShootAngles)
        {
            projectilePool.SpawnObject(transform.position, Quaternion.Euler(0, 0, transform.eulerAngles.z + angle));
        }
        yield return new WaitForSeconds(PauseTime);

        StopCoroutine(Shoot());
        isRotating = true;
    }

    private void StopRotation()
    {
        // Wrap the angle target around to make sure it can continue spinning
        rotationTarget += RotationAmount;
        if (rotationTarget >= 360)
        {
            rotationTarget -= 360;
        }
        StartCoroutine(Shoot());
        isRotating = false;
    }

    private void Update()
    {
        // When we are not shooting, adjust into the next rotation
        if (Active)
        {
            if (isRotating)
            {
                // Stop rotating, continue shooting
                if (Mathf.RoundToInt(transform.localEulerAngles.z) == Mathf.RoundToInt(rotationTarget))
                {
                    transform.localEulerAngles = new Vector3(0, 0, rotationTarget);
                    StopRotation();
                }
                else
                {
                    transform.localEulerAngles += new Vector3(0, 0, RotationSpeed * Time.deltaTime);
                }
            }
        }
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        transform.localEulerAngles = originalRotation;
        projectilePool.ClearAllObjects();
        StopAllCoroutines();
    }
}
