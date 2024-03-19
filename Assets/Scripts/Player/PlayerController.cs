using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using UnityEngine.UIElements;
using Unity.VisualScripting;

public class PlayerController : MonoBehaviour, IDamageable
{
    const float swapCooldown = 5f;

    public enum FacingDir
    {
        Right,
        Left
    }

    [Header("Player State Machine")]
    protected StateMachine playerSM;
    public StateMachine fsm => playerSM; // Properties for the states to access
    public PlayerData Data => data;
    //public PlayerData SubSkullData => subSkullData;
    public Rigidbody2D Rb2d => rb2d;
    public Animator Animator => animator;
    public Vector2 MoveDir => moveDir;
    public bool IsGrounded => isGrounded;
    public bool IsDashing => isDashing;
    public bool IsSwapping => isSwapping;
    public void ToggleIsAttacking(bool b)
    {
        isAttacking = b;
    }

    [Header("Player Data")]
    [SerializeField] protected PlayerData data;
    [SerializeField] protected PlayerControllerDataSO controllerData;

    [Header("Player Move")]
    Vector2 moveDir;
    public FacingDir facingDir;

    [Header("Player Jump")]
    int remainingJumps;
    Vector2 jumpVec;
    float coyoteTimeCounter;
    bool isGrounded;

    [Header("Player Dash")]
    bool isDashing;

    [Header("Player Attack")]
    public bool isAttacking;

    [Header("Player Skills")]
    protected float skill1CooldownTimer = 0f;
    protected float skill2CooldownTimer = 0f;
    public float Skill1CooldownRatio => skill1CooldownTimer / data.skill1Cooldown;
    public float Skill2CooldownRatio => skill2CooldownTimer / data.skill2Cooldown;
    protected bool isFlying;

    [Header("Player Swap")]
    protected PlayerData subSkullData;
    protected bool isSwapping;
    protected float swapCooldownTimer = 0f;
    public float SwapCooldownRatio => swapCooldownTimer / swapCooldown;

    [Header("Effects")]
    SmokeSpawner smokeSpawner;
    PooledObject playerHitEffectPrefab;

    public LayerMask Mask => controllerData.playerMask;

    [Header("Misc")]
    protected Rigidbody2D rb2d;
    protected Animator animator;
    Collider2D[] colliders = new Collider2D[15];
    Collider2D playerCollider;
    Collider2D platformCollider;


    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        smokeSpawner = GetComponentInChildren<SmokeSpawner>();
        playerCollider = GetComponent<BoxCollider2D>();
        playerHitEffectPrefab = Manager.Resource.Load<PooledObject>("Prefabs/PlayerHitEffect");
        dashesLeft = controllerData.dashCount;

        // Cache jump vector once to prevent repetitive math operations
        jumpVec = Vector2.up * Physics2D.gravity.y * controllerData.fallMultiplier * Time.fixedDeltaTime;

        playerSM = new StateMachine(this);
    }

    private void Start()
    {
        playerSM.Initialize(playerSM.idleState);
    }

    private void Update()
    {
        if (isGrounded)
        {
            coyoteTimeCounter = controllerData.coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        playerSM.Update();
    }

    private void FixedUpdate()
    {
        if(isDashing)
        {
            return;
        }

        Move();
        JumpFall();
    }

    #region Collision

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(rb2d.velocity.y > 0)
        {
            return;
        }
        if (controllerData.groundMask.Contains(collision.gameObject.layer))
        {
            isGrounded = true;
            remainingJumps = controllerData.jumpCount;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (controllerData.groundMask.Contains(collision.gameObject.layer))
        {
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Platform down jump
        if (collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            platformCollider = collision.collider;
        }
        //if(collision.gameObject.layer == LayerMask.NameToLayer("Ground"))
        //{
        //    fsm.TransitionTo(fsm.idleState);
        //}
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Platform"))
        {
            platformCollider = null;
        }
    }

    #endregion

    #region Movement
    private void Move()
    {
        if (isFlying)
            return;
        Vector2 newVel = rb2d.velocity;
        newVel.x = moveDir.x * controllerData.moveSpeed;

        if ((isAttacking && isGrounded) || isSwapping) // prevent movement while attacking
        {
            newVel.x = 0;
        }

        rb2d.velocity = newVel;
    }
    private void Flip()
    {
        if (isSwapping)
            return;

        Vector3 newScale = new Vector3(1, 1, 1);
        if (moveDir.x < 0)
        {
            newScale.x = -1;
            transform.localScale = newScale;
            facingDir = FacingDir.Left;
        }
        else if (moveDir.x > 0)
        {
            transform.localScale = newScale;
            facingDir = FacingDir.Right;
        }
    }
    #endregion

    #region Jump

    // BASIC JUMP
    private void Jump()
    {
        if (moveDir.y == -1 && platformCollider != null)
        {
            StartCoroutine(TemporaryIgnoreCollision());
            return;
        }

        if (coyoteTimeCounter <= 0 && remainingJumps == 0)
            return;
        if (remainingJumps == 1) // double jump smoke effect
            smokeSpawner.SpawnSmoke(SmokeSpawner.SmokeType.Jump);

        remainingJumps--;
        coyoteTimeCounter = 0;
        rb2d.velocity = new Vector2(rb2d.velocity.x, controllerData.jumpPower);
    }

    // TODO: STUDY THIS https://www.youtube.com/watch?v=7KiK0Aqtmzc&t=518s
    void JumpFall()
    {
        if (rb2d.velocity.y < 0)
        {
            rb2d.velocity += jumpVec;
        }
    }

    // Platform down jump
    IEnumerator TemporaryIgnoreCollision()
    {
        Collider2D coll = platformCollider;
        Physics2D.IgnoreCollision(playerCollider, coll, true);
        yield return new WaitForSeconds(0.5f);
        Physics2D.IgnoreCollision(playerCollider, coll, false);
    }

    #endregion

    #region Dash
    // TODO: Implement double dash
    int dashesLeft;
    private IEnumerator Dash()
    {
        if (isDashing || dashesLeft == 0)
            yield break;

        // dash start
        if (dashCooldownRoutine != null)
            StopCoroutine(dashCooldownRoutine); // reset cooldown routine
        dashCooldownRoutine = StartCoroutine(DashCooldownRoutine());
        smokeSpawner.SpawnSmoke(SmokeSpawner.SmokeType.Dash);
        playerSM.Trigger(TriggerType.DashTrigger);
        isDashing = true;

        // dash physics
        float originalGravity = rb2d.gravityScale;
        rb2d.gravityScale = 0;
        rb2d.velocity = ((facingDir == FacingDir.Left) ? Vector2.left : Vector2.right) * controllerData.dashPower;
        yield return new WaitForSeconds(controllerData.dashDuration);
        rb2d.velocity = new Vector3(0, rb2d.velocity.y, 0); // prevent sliding
        rb2d.gravityScale = originalGravity;

        // dash end
        isDashing = false;
        dashesLeft--;
    }

    Coroutine dashCooldownRoutine;
    private IEnumerator DashCooldownRoutine()
    {
        yield return new WaitForSeconds(controllerData.dashCooldown);
        dashesLeft = controllerData.dashCount;
    }

    #endregion

    #region Attack
    private void HandleAttackInput()
    {
        playerSM.Trigger(TriggerType.AttackTrigger);
    }
    public void Attack()
    {
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, data.attackRange, colliders, controllerData.hittableMask);
        for (int i = 0; i < count; i++)
        {
            IDamageable[] damageables = colliders[i].GetComponents<IDamageable>();
            foreach (IDamageable damageable in damageables)
            {
                int multipliedDamage = Mathf.CeilToInt(controllerData.baseDamage * data.damageMultiplier);
                int randomizedDamage = Mathf.CeilToInt(multipliedDamage * Random.Range(0.9f, 1.1f));
                damageable.TakeDamage(randomizedDamage);
            }
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, data.attackRange);
    }

    public void TakeDamage(int damage)
    {
        if (isDashing)
            return;

        Manager.Pool.GetPool(playerHitEffectPrefab, transform.position, Quaternion.identity);
        Manager.Game.PlayerTakeDamage(damage);
    }
    #endregion

    #region Skills

    protected virtual void UseSkill1() { }
    protected virtual void UseSkill2() { }
    
    private void Swap() // Request swap to GameManager
    {
        if (Manager.Game.SubSkullData == null)
            return;

        Manager.Game.SwapSkull();
    }

    public virtual void SwapEffect() { }

    #endregion

    #region Interact

    private void Interact()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, controllerData.interactRange, controllerData.interactableMask);
        if (collider == null || collider.GetComponent<IInteractable>() == null)
            return;

        collider.GetComponent<IInteractable>().Interact();
    }

    #endregion

    #region Inputs
    private void OnMove(InputValue value)
    {
        moveDir = value.Get<Vector2>();
        Flip();
    }
    private void OnJump()
    {
        if (isSwapping)
            return;
        Jump();
    }
    private void OnDash()
    {
        StartCoroutine(Dash());
    }
    private void OnSwap()
    {
        if (isSwapping)
            return;
        Swap();
    }
    private void OnAttack()
    {
        HandleAttackInput();
    }
    private void OnSkill1()
    {
        UseSkill1();
    }
    private void OnSkill2()
    {
        UseSkill2();
    }
    private void OnInteract()
    {
        Interact();
    }

    #endregion
}
