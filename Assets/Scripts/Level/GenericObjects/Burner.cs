using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Burner : BaseObject
{
    // ------------------------------
    //
    //   A flamethrower object that emits flames on a set timer
    //
    //   Created: 30/07/2024
    //
    // ------------------------------

    // Public variables
    [Space(10)]
    public float MaxFlameTime;
    public float MaxDelayTime;

    // Private variables
    private GameObject flame;

    private void Awake()
    {
        flame = transform.GetChild(0).gameObject;
        flame.SetActive(false);
    }

    public override void OnActivate()
    {
        base.OnActivate();
        StartCoroutine(BurnerLoop());
    }

    private IEnumerator BurnerLoop()
    {
        flame.SetActive(false);
        yield return new WaitForSeconds(MaxDelayTime);

        flame.SetActive(true);
        yield return new WaitForSeconds(MaxFlameTime);

        StopCoroutine(BurnerLoop());
        StartCoroutine(BurnerLoop());
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        flame.SetActive(false);
        StopAllCoroutines();
    }
}
