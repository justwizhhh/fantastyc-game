using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterProjectile : MonoBehaviour
{
    // ------------------------------
    //
    //   A basic projectile that gets created by the shooter objects
    //
    //   Created: 09/07/2024
    //
    // ------------------------------

    // Public variables
    public float MaxSpeed;
    public bool Bounce;
    public bool PhaseThrough;

    private Vector2 currentVelocity;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        currentVelocity = transform.right * MaxSpeed;
        rb.MovePosition(rb.position + currentVelocity);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Checking for all collisions, whether or not the projectile can phase through walls or not
        if (!PhaseThrough)
        {
            // Checking if the ball can bounce off of the current surface
            if (Bounce)
            {
                Vector2 reflectDirection = Vector2.Reflect(currentVelocity, collision.GetContact(0).normal);

                float reflectAngle = Mathf.Atan2(reflectDirection.y, reflectDirection.x) * Mathf.Rad2Deg;
                transform.rotation = Quaternion.Euler(0, 0, reflectAngle);
            }
            else
            {
                gameObject.SetActive(false);
            }
        }
    }
}
