using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoverBomb : PreviewObject
{
    // ------------------------------
    //
    //   A bomb object which removes other nearby objects within its radius, while also playing an animation
    //
    //   Created: 06/09/2024
    //
    // ------------------------------

    [Space(10)]
    public float RemovalRadius;

    private bool exploded;
    private Animator anim;

    public override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
    }

    public void DestroyNearbyObjects()
    {
        // Check for what objects are crossing into the explosion radius
        Collider2D[] removalCol = Physics2D.OverlapCircleAll(transform.position, RemovalRadius);
        foreach (Collider2D col in removalCol)
        {
            if (col.GetComponent<BaseObject>() != null)
            {
                LevelController.instance.currentLevelObjects.Remove(col.GetComponent<BaseObject>());
                Destroy(col.gameObject);
            }
        }
    }

    private IEnumerator ExplodeCountdown()
    {
        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0)[0].clip.length);

        StopCoroutine(ExplodeCountdown());
        gameObject.SetActive(false);
    }

    public override void Place()
    {
        if (!exploded)
        {
            base.Place();

            exploded = true;
            anim.SetTrigger("Explode");
            StartCoroutine(ExplodeCountdown());
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, RemovalRadius);
    }
}
