using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

public class ObjectPlacementSpace : MonoBehaviour
{
    // ------------------------------
    //
    //   Class for defining which areas the player can place objects in
    //
    //   Created: 18/06/2024
    //
    // ------------------------------

    public PolygonCollider2D PolygonCol;

    private  SpriteShapeRenderer sr;
    private Animator anim;

    private void Awake()
    {
        PolygonCol = GetComponent<PolygonCollider2D>();
        
        sr = GetComponent<SpriteShapeRenderer>();
        // anim = GetComponent<Animator>();
    }

    private void OnEnable()
    {
        EventManager.SwitchToEditing += EventManager_SwitchToEditing;
        EventManager.SwitchToGameplay += EventManager_SwitchToGameplay;
    }

    private void EventManager_SwitchToEditing()
    {
        sr.enabled = true;
    }

    private void EventManager_SwitchToGameplay()
    {
        sr.enabled = false;
    }

    private void Start()
    {
        sr.enabled = false;
    }
}
