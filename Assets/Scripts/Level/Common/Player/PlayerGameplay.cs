using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using static UnityEngine.InputSystem.HID.HID;

public class PlayerGameplay : MonoBehaviour
{
    // ------------------------------
    //
    //   Class for moving the current player around in Gameplay Mode
    //
    //   Created: 14/05/2024
    //
    // ------------------------------

    [Header("Toggleables")]
    public float MoveSpeed;
    public float MoveAccel;
    public float AttackDistance;
    public float AttackTime;
    public float AttackDelay;
    public float RollSpeed;
    public float RollTime;
    public float RollDelayTime;
    public float KnockbackSpeed;
    public float KnockbackTime;
    public float SlipSpeed;
    public float SlipAccel;
    public float CoyoteTime;
    public float GiveUpTime;
    public float FlagTime;
    public float DeathKnockbackSpeed;
    public float DeathKnockbackAccel;

    [Space(10)]
    [Header("Input Values")]
    public Vector2 prevMoveInput = new Vector2(0, 1);
    public Vector2 currentMoveInput = new Vector2(0, 0);
    public bool attackInput;
    public bool rollInput;
    public bool dieInput;
    public bool flagInput;

    // Private variables
    [HideInInspector] public bool isMoving;

    [HideInInspector] public Vector2 velocity;
    [HideInInspector] public float currentAccel;
    [HideInInspector] public int currentFloor;

    [HideInInspector] public bool flagged;

    [HideInInspector] public float currentAttackTimer;
    [HideInInspector] public float currentAttackDelayTimer;
    [HideInInspector] public float currentRollTimer;
    [HideInInspector] public float currentRollDelayTimer;

    [HideInInspector] public float currentKnockbackTimer;
    [HideInInspector] public Vector2 knockbackContact;
    [HideInInspector] public GameObject knockbackSource;

    [HideInInspector] public float currentCoyoteTime;
    [HideInInspector] public float currentGiveUpTime;
    [HideInInspector] public float currentFlagTime;

    // Component references
    [HideInInspector] public PlayerStateMachine stateMachine;
    [HideInInspector] public CircleCollider2D col;
    [HideInInspector] public Rigidbody2D rb;
    [HideInInspector] public SpriteRenderer sr;
    [HideInInspector] public Animator anim;

    private void Awake()
    {
        stateMachine = GetComponent<PlayerStateMachine>();
        col = GetComponent<CircleCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        sr = transform.GetChild(0).GetComponent<SpriteRenderer>();
        anim = transform.GetChild(0).GetComponent<Animator>();
    }

    // Input functions
    public void OnMovement(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            currentMoveInput = context.ReadValue<Vector2>();
        }
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            attackInput = context.ReadValueAsButton();
        }
    }

    public void OnRoll(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            rollInput = context.ReadValueAsButton();
        }
    }

    public void OnDie(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            dieInput = context.ReadValueAsButton();
        }
    }

    public void OnFlag(InputAction.CallbackContext context)
    {
        if (context.performed || context.canceled)
        {
            flagInput = context.ReadValueAsButton();
        }
    }

    private void Update()
    {
        // Specific player control functions
        CheckFloorType();

        UpdateRollDelay();
        UpdateCoyoteTime();
        UpdateGiveUpTimer();
        UpdateFlagTimer();

        UpdateAnimator();

        // Getting crushed by an object (code is a bit dodgy for this one)
        Collider2D[] crushCheck = Physics2D.OverlapCircleAll(rb.position, col.radius);
        foreach (Collider2D hit in crushCheck)
        {
            if (hit.gameObject.GetComponent<PlayerGameplay>() != null
                || hit.gameObject.GetComponent<Rigidbody2D>() == null)
            {
                continue;
            }
            else if (hit.gameObject.layer != LayerMask.NameToLayer("Objects"))
            {
                continue;
            }
            else
            {
                //Debug.Log((hit.ClosestPoint(rb.position) - rb.position).sqrMagnitude);
                if ((hit.ClosestPoint(rb.position) - rb.position).sqrMagnitude == 0)
                {
                    stateMachine.ChangeState(typeof(PlayerDead));
                    break;
                }
            }
        }
    }

    public void SetKnockback(Vector2 pos, GameObject source, Collision2D collision = null)
    {
        knockbackContact = pos;
        knockbackSource = source;

        // Remove any separation the player might have with the colliding object
        if (collision != null)
        {
            rb.position += collision.GetContact(0).normal * collision.GetContact(0).separation;
        }
    }

    public void CheckHarmfulCollision()
    {
        // This gets called by other states if they need to check for harmful collisions like projectiles or other enemies
        Collider2D[] harmfulCols = Physics2D.OverlapCircleAll(rb.position, col.radius);
        foreach(Collider2D col in harmfulCols)
        {
            if (col.gameObject.CompareTag("Harmful"))
            {
                stateMachine.ChangeState(typeof(PlayerDead));
            }
        }
    }

    private void CheckFloorType()
    {
        // Note to self: this might break stuff once i add other objects that the player can walk over
        Collider2D[] floorCols = Physics2D.OverlapPointAll(rb.position);
        if (floorCols.Length <= 1)
        {
            currentFloor = 0;
        }
        else
        {
            foreach (Collider2D floor in floorCols)
            {
                if (floor != col)
                {
                    currentFloor = floor.gameObject.layer;
                    break;
                }
            }
        }
    }

    private void UpdateRollDelay()
    {
        if (stateMachine.currentPlayerState.GetType() != typeof(PlayerRoll))
        {
            if (currentRollDelayTimer > 0)
            {
                currentRollDelayTimer -= Time.deltaTime;
            }
        }
    }

    private void UpdateCoyoteTime()
    {
        if (stateMachine.currentPlayerState.GetType() != typeof(PlayerIdlePit)
            || stateMachine.currentPlayerState.GetType() != typeof(PlayerMovePit))
        {
            if (currentCoyoteTime > 0)
            {
                currentCoyoteTime -= Time.deltaTime;
            }
        }
    }

    private void UpdateGiveUpTimer()
    {
        if (dieInput)
        {
            // TO-DO : Give this a cool UI animation as well
            currentGiveUpTime += Time.deltaTime;
            if (currentGiveUpTime > GiveUpTime)
            {
                stateMachine.ChangeState(typeof(PlayerDead));
            }
        }
        else
        {
            currentGiveUpTime = 0;
        }
    }

    private void UpdateFlagTimer()
    {
        if (flagInput)
        {
            currentFlagTime += Time.deltaTime;
            if (currentFlagTime > GiveUpTime)
            {
                if (!flagged)
                {
                    flagged = true;
                    GameController.instance.UpdateFlaggingPlayers();
                }
            }
        }
        else
        {
            currentFlagTime = 0;
        }
    }

    private void UpdateAnimator()
    {
        anim.SetFloat("DirectionX", prevMoveInput.x);
        anim.SetFloat("DirectionY", prevMoveInput.y);
    }

    private void FixedUpdate()
    {
        rb.velocity = velocity * Time.fixedDeltaTime;
    }
}
