using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBeehiveBee : MonoBehaviour
{
    // ------------------------------
    //
    //   The homing bee projectile that gets "shot out" of the beehive
    //
    //   Created: 24/07/2024
    //
    // ------------------------------

    // Public variables
    public float MaxSpeed;
    public float MaxTurnSpeed;

    // Private variables
    public Transform target;

    // Component references
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void AssignTarget(Transform newTarget)
    {
        target = newTarget;
        transform.LookAt(target.position);
    }

    private void FixedUpdate()
    {
        // (Try to) move towards the target
        rb.velocity = transform.right.normalized * MaxSpeed;

        Vector3 lookAtPos = (Vector2)target.position - rb.position;
        float lookAtAngle = Mathf.Atan2(lookAtPos.y, lookAtPos.x) * Mathf.Rad2Deg;

        Quaternion lookAtQuaternion = Quaternion.Euler(0, 0, lookAtAngle);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookAtQuaternion, MaxTurnSpeed);
    }
}
