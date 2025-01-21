using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingBoard : BaseObject
{
    // ------------------------------
    //
    //   An object that moves between a fixed set of points
    //
    //   Created: 08/07/2024
    //
    // ------------------------------

    // Public variables
    [Space(10)]
    public float MaxSpeed;
    public float MaxPauseTime;

    // Private variables
    private Vector2 startPos;

    private bool isMoving;
    private List<Transform> nodes = new List<Transform>();
    private int currentNodeID;

    // Component references
    private Rigidbody2D rb;

    private void Awake()
    {
        foreach (Transform tr in transform)
        {
            nodes.Add(tr);
        }

        rb = GetComponent<Rigidbody2D>();
    }

    public override void OnActivate()
    {
        base.OnActivate();
        startPos = rb.position;
        isMoving = true;
        currentNodeID = 0;

        if (transform.childCount > 0 )
        {
            transform.DetachChildren();
        }
    }

    // Let the object pause a bit before starting to move towards the next target position on its path
    private IEnumerator Pause()
    {
        yield return new WaitForSeconds( MaxPauseTime );

        if (currentNodeID < nodes.Count - 1)
        {
            currentNodeID++;
        }
        else
        {
            currentNodeID = 0;
        }

        isMoving = true;
    }

    private void Update()
    {
        if (isMoving)
        {
            rb.position += ((Vector2)nodes[currentNodeID].position - rb.position).normalized * (MaxSpeed * Time.deltaTime);
            if (rb.position == (Vector2)nodes[currentNodeID].position)
            {
                StartCoroutine(Pause());
                isMoving = false;
            }
        }
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        rb.position = startPos;
        isMoving = false;

        StopAllCoroutines();
    }
}
