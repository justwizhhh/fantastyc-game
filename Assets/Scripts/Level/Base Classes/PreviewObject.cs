using System.Collections.Generic;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.UI;

public class PreviewObject : MonoBehaviour
{
    // ------------------------------
    //
    //   Base object for animation-only editor objects
    //   The rest of the object logic only gets instantiated once Gameplay Mode is initiated
    //
    //   Created: 31/05/2024
    //
    // ------------------------------

    // Public variables
    [Range(0, 180)]
    public float MaxRotation;
    [Range(0.0f, 1.0f)]
    public float Rarity = 1;
    public BaseObject[] SourceObjects;
    public Sprite[] PreviewSprites;
    public Color SpriteInvalidColour;

    [Space(10)]
    public bool IsInsideForeground;
    public bool IsInsidePlacementSpace;

    [HideInInspector] public bool Selected;
    [HideInInspector] public int Variation = 0;

    [HideInInspector] public RectTransform rt;
    private Collider2D col;
    private SpriteRenderer sr;
    private Image image;

    public virtual void Awake()
    {
        rt = GetComponent<RectTransform>();
        col = GetComponent<Collider2D>();
        sr = GetComponent<SpriteRenderer>();
        image = GetComponent<Image>();
    }

    private void Update()
    {
        // Constantly check for ground/placement-space collisions, so the object transparency can change accordingly
        ContactFilter2D contactFilter = new ContactFilter2D().NoFilter();
        List<Collider2D> colCheck = new List<Collider2D>();

        Physics2D.OverlapCollider(col, contactFilter, colCheck);

        IsInsideForeground = colCheck.Find(col => col.gameObject.layer == LayerMask.NameToLayer("Foreground"));
        IsInsidePlacementSpace = colCheck.Find(col => col.GetComponent<ObjectPlacementSpace>());

        sr.color = (IsInsideForeground || !IsInsidePlacementSpace) ? SpriteInvalidColour : Color.white;
    }

    public void Select()
    {
        Selected = true;
    }

    public void SwitchToSprite()
    {
        sr.enabled = true;
        image.enabled = false;
    }

    public void Rotate(int dir)
    {
        if (MaxRotation != 0)
        {
            transform.Rotate(new Vector3(0, 0, MaxRotation * dir));
        }
    }

    public void Tweak()
    {
        if (SourceObjects.Length > 1)
        {
            Variation++;

            sr.sprite = PreviewSprites[Variation];
        }
    }

    // Special behaviour can be defined by child classes, when the object gets placed
    public virtual void Place()
    {
        LevelController.instance.placedPrevObjects.Add(this);
    }
}
