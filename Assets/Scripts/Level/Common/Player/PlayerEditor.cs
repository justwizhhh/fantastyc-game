using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PlayerEditor : MonoBehaviour
{
    // ------------------------------
    //
    //   Class for moving the current player's cursor around in Editing Mode
    //
    //   Created: 14/05/2024
    //
    // ------------------------------

    [Header("Toggleables")]
    public float UIMoveSpeed;
    public float UIMoveAccel;
    public float SpriteMoveSpeed;
    public float SpriteMoveAccel;

    [Space(10)]
    [Header("Input Values")]
    public Vector2 currentMoveInput;

    // Private variables
    private Vector2 velocity;

    private bool uiMode;
    private PreviewObject selectedObject;

    // Component references
    private RectTransform rt;
    private SpriteRenderer sr;
    private Image image;

    private Camera cam;
    private Canvas canvas;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        sr = GetComponent<SpriteRenderer>();
        image = GetComponent<Image>();

        cam = (Camera)FindFirstObjectByType(typeof(Camera));
        canvas = FindObjectOfType<Canvas>();
    }

    private void Start()
    {
        SwitchToUI();
    }

    // Input functions
    public void OnMovement(InputAction.CallbackContext context)
    {
        currentMoveInput = context.ReadValue<Vector2>();
    }

    public void OnSelect(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            // Either select an object, or place down the currently-selected object
            if (uiMode)
            {
                if (selectedObject == null)
                {
                    SelectObject();
                }
            }
            else
            {
                PlaceObject();
            }
        }
    }

    public void OnTweak(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (selectedObject != null)
            {
                selectedObject.Tweak();
            }
        }
    }

    public void OnRotate(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (selectedObject != null)
            {
                selectedObject.Rotate((int)context.ReadValue<float>());
            }
        }
    }

    private void FixedUpdate()
    {
        // Finally moving the player's cursor
        float screenDependentAccel = UIMoveAccel * Screen.width;
        float screenDependentSpeed = UIMoveSpeed * Screen.width;

        if (currentMoveInput != Vector2.zero)
        {
            velocity += currentMoveInput.normalized * (uiMode ? screenDependentAccel : SpriteMoveAccel) * Time.deltaTime;
            velocity = Vector2.ClampMagnitude(velocity, (uiMode ? screenDependentSpeed : SpriteMoveSpeed));
        }
        else
        {
            velocity = Vector2.zero;
        }

        //velocity = currentMoveInput.normalized * (uiMode ? screenDependentSpeed : SpriteMoveSpeed) * Time.deltaTime;

        transform.position += (Vector3)velocity;

        // Keep the player within screen boundaries
        if (uiMode)
        {
            transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, cam.ViewportToScreenPoint(new Vector3(0, 0, 0)).x, cam.ViewportToScreenPoint(new Vector3(1, 0, 0)).x),
            Mathf.Clamp(transform.position.y, cam.ViewportToScreenPoint(new Vector3(0, 0, 0)).y, cam.ViewportToScreenPoint(new Vector3(0, 1, 0)).y),
            0);
        }
        else
        {
            transform.position = new Vector3(
            Mathf.Clamp(transform.position.x, cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).x, cam.ViewportToWorldPoint(new Vector3(1, 0, 0)).x),
            Mathf.Clamp(transform.position.y, cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).y, cam.ViewportToWorldPoint(new Vector3(0, 1, 0)).y),
            0);
        }
        
    }

    // Check if the player is hovering above a selectable object
    private void SelectObject()
    {
        foreach (PreviewObject obj in LevelController.instance.currentPrevObjects)
        {
            Vector3[] rectPoints = { Vector3.zero, Vector3.zero, Vector3.zero, Vector3.zero };
            obj.rt.GetWorldCorners(rectPoints);
            Rect pointCheck = new Rect(rectPoints[0].x, rectPoints[0].y, rectPoints[2].x - rectPoints[0].x, rectPoints[2].y - rectPoints[0].y);

            if (pointCheck.Contains(rt.position))
            {
                if (!LevelController.instance.selectedPrevObjects.Contains(obj))
                {
                    selectedObject = obj;
                    selectedObject.Select();
                    selectedObject.transform.position = transform.position;
                    selectedObject.transform.SetParent(transform, true);
                    selectedObject.transform.localScale = Vector3.one;

                    LevelController.instance.currentPrevObjects.Remove(selectedObject);
                    LevelController.instance.selectedPrevObjects.Add(selectedObject);

                    break;
                }
            }
        }
    }

    private void PlaceObject()
    {
        if (selectedObject != null)
        {
            // Check if the object is within viable placement space, and also does not collide with any other obstacles
            if (selectedObject.IsInsidePlacementSpace && !selectedObject.IsInsideForeground)
            {
                selectedObject.transform.SetParent(null);
                selectedObject.transform.position = transform.position;
                selectedObject.Place();

                selectedObject = null;

                gameObject.SetActive(false);
            }
        }
    }

    // Put the player, and their currently selected object into world space, and out of the canvas
    public void SwitchToSprite()
    {
        if (uiMode)
        {
            // Change player properties
            uiMode = false;
            sr.enabled = true;
            image.enabled = false;

            Vector3 newPos = Camera.main.ScreenToWorldPoint(rt.position);
            transform.SetParent(null, true);
            transform.position = new Vector3(newPos.x, newPos.y, 0);
            transform.localScale = Vector3.one;

            // Change object properties
            selectedObject.SwitchToSprite();
            LevelController.instance.ClearPreviewObjects();
        }
    }

    public void SwitchToUI()
    {
        if (!uiMode)
        {
            uiMode = true;
            sr.enabled = false;
            image.enabled = true;

            transform.SetParent(canvas.transform, true);
            rt.anchoredPosition = Vector2.zero;
            transform.localScale = Vector3.one;
        }
    }
}
