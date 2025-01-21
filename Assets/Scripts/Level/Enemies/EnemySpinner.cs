using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpinner : BaseEnemyObject
{
    // ------------------------------
    //
    //   Projectiles that touch this enemy get blasted away at a random direction
    //
    //   Created: 22/07/2024
    //
    // ------------------------------

    // Public variables
    [Space(10)]
    public float MaxReflectAngle;
    [Space(10)]
    public float MoveSpeed;
    public float MoveTime;
    public float MovePauseTime;

    // Private variables
    private Vector2[] moveDirections = new Vector2[]
    {
        -Vector2.right,
        Vector2.right,
        Vector2.up,
        -Vector2.up
    };
    private Vector2 currentDir;

    // Component references
    private CircleCollider2D col;
    private Rigidbody2D rb;

    private void Awake()
    {
        col = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        if (Active)
        {
            OnActivate();
        }
    }

    public override void OnActivate()
    {
        base.OnActivate();
        StartCoroutine(ChangeMoveTarget());
    }

    private IEnumerator ChangeMoveTarget()
    {
        // Constantly assign a new place for the enemy to move to
        currentDir = Vector2.zero;
        yield return new WaitForSeconds(MovePauseTime);
        currentDir = moveDirections[Random.Range(0, moveDirections.Length)];
        yield return new WaitForSeconds(MoveTime);

        StopCoroutine(ChangeMoveTarget());
        StartCoroutine(ChangeMoveTarget());
    }

    private void FixedUpdate()
    {
        if (Active)
        {
            // Moving around
            if (currentDir != Vector2.zero)
            {
                rb.velocity = currentDir * MoveSpeed;
            }
            else
            {
                rb.velocity = Vector2.zero;
            }
            
            // Projectile deflection
            Collider2D[] colCheck = Physics2D.OverlapCircleAll(rb.position, col.radius);
            foreach (Collider2D hit in colCheck)
            {
                if (hit.gameObject.TryGetComponent(out ShooterProjectile projectile))
                {
                    projectile.transform.eulerAngles += new Vector3(0, 0, 180 + Random.Range(-MaxReflectAngle, MaxReflectAngle));

                    // Move the projectile out of the enemy's collider, to give the projectile room to continue moving again
                    projectile.transform.position = (Vector3)rb.position - (transform.right.normalized * (col.radius * 2));
                }
            }
        }
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        StopAllCoroutines();
    }
}
