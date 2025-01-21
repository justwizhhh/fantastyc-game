using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIObjectSlot : MonoBehaviour
{
    // ------------------------------
    //
    //   The slot areas that objects are positioned in for Editor Mode
    //
    //   Created: 30/07/2024
    //
    // ------------------------------

    // Public variables
    public float PosRandomness;

    // Private variables
    private Vector2 originalPos;

    // Component references
    private RectTransform rt;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        EventManager.SwitchToEditing += EventManager_SwitchToEditing;
    }

    private void Start()
    {
        originalPos = rt.anchoredPosition;
    }

    private void EventManager_SwitchToEditing()
    {
        rt.anchoredPosition = originalPos + new Vector2(Random.Range(-PosRandomness, PosRandomness), Random.Range(-PosRandomness, PosRandomness));
    }

    private void OnDisable()
    {
        EventManager.SwitchToEditing -= EventManager_SwitchToEditing;
    }
}
