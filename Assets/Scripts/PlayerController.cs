using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public enum FacingDir
    {
        Right,
        Left
    }

    [Header("Player State Machine")]
    [SerializeField] StateMachine playerSM;
    public StateMachine fsm => playerSM; // Properties for the states
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

    [Header("Player Move")]
    [SerializeField] float moveSpeed;
    Vector2 moveDir;
    public FacingDir facingDir;

    [Header("Player Jump")]
    [SerializeField] float jumpPower;
    [SerializeField] int jumpCount;
    [SerializeField] float fallMultiplier = 2.5f;
    [SerializeField] float coyoteTime;
    [SerializeField] LayerMask groundLayer;
    int remainingJumps;
    Vector2 jumpVec;
    float coyoteTimeCounter;
    bool isGrounded;

    [Header("Player Dash")]
    [SerializeField] float dashPower;
    [SerializeField] float dashDuration;
    [SerializeField] float dashCooldown;
    public bool canDash;
    bool isDashing;

    [Header("Player Attack")]
    [SerializeField] float attackRange;
    [SerializeField] int attackDamage;
    [SerializeField] LayerMask hittableMask;
    public bool isAttacking;

    [Header("Player Skills")]
    [SerializeField] GameObject skullPrefab;
    [SerializeField] float skullCooldown;
    [SerializeField] LayerMask skullMask;
    [SerializeField] RuntimeAnimatorController defaultController;
    [SerializeField] RuntimeAnimatorController headlessController;
    float cooldownTimer = 0f;
    bool isSwapping;
    
    [Header("Player Swap")]
    [SerializeField] float swapDuration;
    [SerializeField] float swapSpeed;

    [Header("Misc")]
    Rigidbody2D rb2d;
    Animator animator;
    Collider2D[] colliders = new Collider2D[15];


    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        canDash = true;

        // Cache jump vector once to prevent repetitive math operations
        jumpVec = Vector2.up * Physics2D.gravity.y * fallMultiplier * Time.fixedDeltaTime;

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
            coyoteTimeCounter = coyoteTime;
        }
        else
        {
            coyoteTimeCounter -= Time.deltaTime;
        }
        playerSM.Update();
    }

    private void FixedUpdate()
    {
        animator.SetFloat("ySpeed", rb2d.velocity.y);
        if (isDashing)
            return;
        
        Move();
        JumpFall();
    }

    #region Collision

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (groundLayer.Contains(collision.gameObject.layer))
        {
            animator.SetBool("isGrounded", true);
            isGrounded = true;
            remainingJumps = jumpCount;
        }
        if (skullMask.Contains(collision.gameObject.layer))
        {
            PickUpSkull();
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (groundLayer.Contains(collision.gameObject.layer))
        {
            animator.SetBool("isGrounded", false);
            isGrounded = false;
        }
    }
    #endregion

    #region Movement
    private void Move()
    {
        Vector2 newVel = rb2d.velocity;
        newVel.x = moveDir.x * moveSpeed;

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
        if(coyoteTimeCounter <= 0 && remainingJumps == 0)
            return;

        remainingJumps--;
        coyoteTimeCounter = 0;
        rb2d.velocity = new Vector2(rb2d.velocity.x, jumpPower);
    }

    // TODO: STUDY THIS https://www.youtube.com/watch?v=7KiK0Aqtmzc&t=518s
    void JumpFall()
    {
        if (rb2d.velocity.y < 0)
        {
            rb2d.velocity += jumpVec;
        }
    }

    //private void CheckGround() // For Coyote Time
    //{
    //    if(Physics2D.OverlapCircle(groundCheck.position, 0.5f, 0, groundLayer))
    //    {
    //        isGrounded = true;
    //    }
    //}

    #endregion

    #region Dash
    // TODO: Implement double dash
    private IEnumerator Dash()
    {
        canDash = false;
        playerSM.Trigger(TriggerType.DashTrigger);
        isDashing = true;
        float originalGravity = rb2d.gravityScale;
        rb2d.gravityScale = 0;
        rb2d.velocity = ((facingDir == FacingDir.Left) ? Vector2.left : Vector2.right) * dashPower;
        yield return new WaitForSeconds(dashDuration);
        rb2d.velocity = new Vector3(0, rb2d.velocity.y, 0); // prevent sliding
        rb2d.gravityScale = originalGravity;
        isDashing = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;
    }
    #endregion

    #region Attack
    private void HandleAttackInput()
    {
        playerSM.Trigger(TriggerType.AttackTrigger);
    }
    public void Attack()
    {
        int count = Physics2D.OverlapCircleNonAlloc(transform.position, attackRange, colliders, hittableMask);

        for(int i = 0; i < count; i++)
        {
            IDamageable[] damageables = colliders[i].GetComponents<IDamageable>();
            foreach(IDamageable damageable in damageables)
            {
                damageable.TakeDamage(attackDamage);
            }
        }
    }
    #endregion

    #region Skills
    Coroutine throwSkullRoutine;
    GameObject thrownSkull = null;

    private void ThrowSkull()
    {
        if (cooldownTimer > 0)
            return;
        animator.Play("SkulThrow");
        throwSkullRoutine = StartCoroutine(ThrowSkullRoutine());
    }

    private IEnumerator ThrowSkullRoutine()
    {
        cooldownTimer = skullCooldown;
        animator.runtimeAnimatorController = headlessController;
        Vector2 direction = (facingDir == FacingDir.Left) ? Vector2.left : Vector2.right;
        thrownSkull = Instantiate(skullPrefab, transform.position, Quaternion.identity);
        thrownSkull.GetComponent<Skull>().SetDirection(direction);
        while (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
            yield return null;
        }
        Destroy(thrownSkull);
        cooldownTimer = 0;
        animator.runtimeAnimatorController = defaultController;
    }

    private void TeleportToSkull()
    {
        if(thrownSkull != null)
        {
            transform.position = thrownSkull.transform.position;
            PickUpSkull();
        }
    }

    private void PickUpSkull()
    {
        StopCoroutine(throwSkullRoutine);
        Destroy(thrownSkull);
        cooldownTimer = 0;
        animator.runtimeAnimatorController = defaultController;
    }

    private IEnumerator SwapAttack()
    {
        isSwapping = true;
        playerSM.Trigger(TriggerType.SwapTrigger);
        float time = 0;
        float originalGravity = rb2d.gravityScale;
        rb2d.gravityScale = 0;
        rb2d.velocity = Vector3.zero;
        Vector3 direction = (facingDir == FacingDir.Left) ? Vector2.left : Vector2.right;

        while (time < swapDuration)
        {
            time += Time.deltaTime;
            transform.Translate(direction * swapSpeed * Time.deltaTime);
            yield return null;
        }

        //moveDir = Vector2.zero;
        rb2d.gravityScale = originalGravity;
        isSwapping = false;
    }
    #endregion

    #region Inputs
    private void OnMove(InputValue value)
    {
        moveDir = value.Get<Vector2>();
        animator.SetFloat("xSpeed", Mathf.Abs(moveDir.x));
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
        if (!canDash)
            return;
        StartCoroutine(Dash());
    }
    private void OnSwap()
    {
        if (isSwapping)
            return;
        StartCoroutine(SwapAttack());
    }
    private void OnAttack()
    {
        HandleAttackInput();
    }
    private void OnSkill1()
    {
        ThrowSkull();
    }

    private void OnSkill2()
    {
        TeleportToSkull();
    }

    #endregion
}
