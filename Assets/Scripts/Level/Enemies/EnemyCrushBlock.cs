using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyCrushBlock : BaseEnemyObject
{
    // ------------------------------
    //
    //   A thwomp-like enemy that tries to crush nearby players
    //
    //   Created: 22/07/2024
    //
    // ------------------------------

    // TODO - Add a delay to after the crushing is done maybe. fix bug with increased delay time

    // Public variables
    [Space(10)]
    public float MaxSpeed;
    public float MaxAccel;
    public float ColCheckMargin;
    public float CrushDelayTime;

    // Private variables
    private bool isCrushing;

    private Vector2 currentDirection;
    private Transform currentTarget;
    private Vector2 currentVelocity;

    private Vector2[] checkDirections = new Vector2[]
    {
        -Vector2.right,
        Vector2.right,
        Vector2.up,
        -Vector2.up
    };

    // Component references
    private BoxCollider2D col;
    private Rigidbody2D rb;

    private void Awake()
    {
        col = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // First, lets try finding a player to crush
    private void Update()
    {
        if (Active)
        {
            if (!isCrushing)
            {
                List<RaycastHit2D> playerChecks = new List<RaycastHit2D>();

                foreach (Vector2 direction in checkDirections)
                {
                    RaycastHit2D[] hits = Physics2D.BoxCastAll(
                        rb.position, 
                        new Vector2(col.size.x - ColCheckMargin, col.size.y - ColCheckMargin), 
                        0, direction);
                    playerChecks.AddRange(hits);
                }

                foreach (RaycastHit2D hit in playerChecks)
                {
                    if (hit.collider.gameObject.activeInHierarchy && hit.collider.gameObject.GetComponent<PlayerGameplay>() != null)
                    {
                        // Check if there is a wall between the enemy and the player
                        RaycastHit2D[] wallCheck = Physics2D.LinecastAll(hit.point, rb.position);
                        bool isWallInbetween = false;
                        
                        foreach (RaycastHit2D wall in wallCheck)
                        {
                            if (wall.collider.gameObject.layer == LayerMask.NameToLayer("Foreground"))
                            {
                                isWallInbetween = true;
                            }
                        }

                        if (!isWallInbetween)
                        {
                            currentTarget = hit.transform;
                            StartCoroutine(StartCrushing());
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

    // Give the player some time to react...
    private IEnumerator StartCrushing()
    {
        yield return new WaitForSeconds(CrushDelayTime);

        // Take the target direction and snap it to the nearest cardinal point
        Vector2 nonSnappedDir = (currentTarget.position - rb.transform.position).normalized;
        if (Mathf.Abs(nonSnappedDir.x) > Mathf.Abs(nonSnappedDir.y))
        {
            currentDirection = new Vector2(Mathf.Sign(nonSnappedDir.x), 0);
        }
        else
        {
            currentDirection = new Vector2(0, Mathf.Sign(nonSnappedDir.y));
        }
        
        isCrushing = true;
    }

    // If crushing has been initiated, we start moving the crusher around the level
    private void FixedUpdate()
    {
        if (Active)
        {
            if (isCrushing)
            {
                currentVelocity += currentDirection * MaxAccel;
                rb.velocity = Vector2.ClampMagnitude(currentVelocity, MaxSpeed);
            }
        }
    }

    // Stop moving if the enemy has reached a wall
    private void ColCheck(Collision2D collision)
    {
        if (collision.collider.gameObject.layer == LayerMask.NameToLayer("Foreground"))
        {
            // TO-DO : find a better way to solve the overlap here because this is lazy
            rb.position += -rb.velocity.normalized * 0.1f;

            currentVelocity = Vector2.zero;
            rb.velocity = Vector2.zero;

            isCrushing = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ColCheck(collision);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        ColCheck(collision);
    }
}
