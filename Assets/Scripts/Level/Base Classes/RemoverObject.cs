using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoverObject : PreviewObject
{
    // ------------------------------
    //
    //   An object which removes other nearby objects within its radius
    //
    //   Created: 15/06/2024
    //
    // ------------------------------

    public float RemovalRadius;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, RemovalRadius);
    }

    public void DestroyNearbyObjects()
    {
        // Check for what objects are crossing into the explosion radius
        Collider2D[] removalCol = Physics2D.OverlapCircleAll(transform.position, RemovalRadius);
        foreach (Collider2D col in removalCol)
        {
            if (col.GetComponent<BaseObject>() != null)
            {
                Destroy(col.gameObject);
            }
        }
    }

    public override void Place()
    {
        DestroyNearbyObjects();
        Destroy(gameObject);
    }
}
