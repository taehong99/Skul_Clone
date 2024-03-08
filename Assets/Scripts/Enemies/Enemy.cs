using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour, IDamageable
{
    public enum State { Idle, Patrol, Chase, Stance, Attack, Dead }
    protected StateMachine<State> stateMachine = new StateMachine<State>();

    [Header("Stats")]
    [SerializeField] protected int baseHP;
    [SerializeField] protected float moveSpeed;
    [SerializeField] protected float knockbackForce;
    [SerializeField] protected float playerCheckRange;

    [Header("Patrol")]
    [SerializeField] float patienceTimer = 3; // how long before going from idle to patrol
    [SerializeField] float patrolTime = 2; // patrol time for each direction
    bool playerInRange;

    [Header("Chase")]
    [SerializeField] LayerMask playerMask;

    [Header("Attack")]
    [SerializeField] protected float attackRange;
    [SerializeField] protected float stanceDuration;
    [SerializeField] protected float damage;
    protected bool isAttacking;

    [Header("Misc")]
    private int hp;
    public int HP { get { return hp; } private set { hp = value; OnHPChanged?.Invoke(value); } }
    public event UnityAction<int> OnHPChanged;
    public event UnityAction OnFlipped;
    private int animIndex = 0;
    private bool isPatrolling;

    protected PlayerController player;
    protected Animator animator;
    protected Rigidbody2D rb2d;
    private SpriteRenderer spriter;
    private LedgeChecker ledgeChecker;

    private void Awake()
    {
        HP = baseHP;
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        spriter = GetComponent<SpriteRenderer>();
        ledgeChecker = GetComponentInChildren<LedgeChecker>();

        GameObject playerChecker = new GameObject("PlayerChecker");
        playerChecker.transform.SetParent(transform);
        playerChecker.transform.localPosition = Vector3.zero;
        CircleCollider2D collider = playerChecker.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = playerCheckRange;
    }

    private void Start()
    {
        player = Manager.Game.Player;

        stateMachine.AddState(State.Idle, new IdleState(this));
        stateMachine.AddState(State.Patrol, new PatrolState(this));
        stateMachine.AddState(State.Chase, new ChaseState(this));
        stateMachine.AddState(State.Attack, new AttackState(this));
        stateMachine.AddState(State.Dead, new DeadState(this));

        stateMachine.Start(State.Idle);
    }

    private void Update()
    {
        stateMachine.Update();
        Flip();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (playerMask.Contains(collision.gameObject.layer))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (playerMask.Contains(collision.gameObject.layer))
        {
            playerInRange = false;
        }
    }

    #region Methods
    #region Flips
    private void Flip()
    {
        Vector3 newScale = transform.localScale;

        if (rb2d.velocity.x < -0.1f)
        {
            newScale.x = -1;
        }
        else if (rb2d.velocity.x > 0.1f)
        {
            newScale.x = 1;
        }

        if(newScale.x != transform.localScale.x)
        {
            OnFlipped?.Invoke();
        }

        transform.localScale = newScale;
    }

    private void FlipOnDemand()
    {
        Vector2 newScale = transform.localScale;

        newScale.x = -newScale.x;

        transform.localScale = newScale;
    }

    private void FacePlayer()
    {
        Vector3 newScale = transform.localScale;

        if (player.transform.position.x > transform.position.x) // player is to the right
        {
            newScale.x = 1;
        }
        else // player is to the left
        {
            newScale.x = -1;
        }

        if (newScale.x != transform.localScale.x)
        {
            OnFlipped?.Invoke();
        }

        transform.localScale = newScale;
    }

    #endregion

    public void TakeDamage(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            stateMachine.ChangeState(State.Dead);
            return;
        }
        animator.Play($"Hit{animIndex}");
        animIndex ^= 1;
        KnockBack();
    }

    public void KnockBack()
    {
        if (knockbackForce == 0)
            return;
        Vector2 knockDirection = Manager.Game.Player.facingDir == PlayerController.FacingDir.Left ? Vector2.left : Vector2.right;
        rb2d.AddForce(knockDirection * knockbackForce, ForceMode2D.Impulse);
    }

    Coroutine patrolRoutine;
    private void StartPatrol()
    {
        patrolRoutine = StartCoroutine(Patrol());
    }
    private void StopPatrol()
    {
        StopCoroutine(patrolRoutine);
    }
    float timer;
    bool flipped;
    protected virtual IEnumerator Patrol()
    {
        isPatrolling = true;
        timer = patrolTime;

        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            rb2d.velocity = Vector2.right * moveSpeed;
            //transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            yield return null;
        }
        rb2d.velocity = Vector2.zero;

        if (!flipped)
        {
            FlipOnDemand();
        }

        timer = patrolTime;
        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            rb2d.velocity = Vector2.left * moveSpeed;
            //transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            yield return null;
        }
        
        stateMachine.ChangeState(State.Idle);
        isPatrolling = false;
    }

    //protected void CheckForPlayer() OverlapCircle Version
    //{
    //    Collider2D collider = Physics2D.OverlapCircle(transform.position, playerCheckRange, playerMask);
    //    Debug.Log(collider.gameObject.name);
    //    if(collider != null)
    //    {
    //        StopCoroutine(patrolRoutine);
    //        stateMachine.ChangeState(State.Chase);
    //    }
    //}

    protected virtual void Attack()
    {
        Debug.Log("Attack!");
    }

    protected virtual void StopAttack()
    {
        Debug.Log("Attack disrupted!");
    }

    private void Die() {
        GetComponent<Rigidbody2D>().excludeLayers = playerMask;
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        //Gizmos.DrawWireSphere(transform.position, playerCheckRange);
    }
    #endregion

    #region States
    private class EnemyState : BaseState<State>
    {
        protected Enemy enemy;
    }

    private class IdleState : EnemyState
    {
        private float timer;

        public IdleState(Enemy enemy)
        {
            this.enemy = enemy;
        }

        public override void Enter()
        {
            Debug.Log("Entered Idle");
            enemy.rb2d.velocity = Vector2.zero;
            enemy.animator.Play("Idle");
            timer = enemy.patienceTimer;
        }

        public override void Update()
        {
            timer -= Time.deltaTime;
        }

        public override void Transition()
        {
            if (timer <= 0)
            {
                ChangeState(State.Patrol);
            }
        }
    }

    private class PatrolState : EnemyState
    {
        public PatrolState(Enemy enemy)
        {
            this.enemy = enemy;
        }

        public override void Enter()
        {
            Debug.Log("Entered Patrol");
            enemy.ledgeChecker.OnReachedEndOfLedge += ReachedEndOfLedge;
            enemy.animator.Play("Walk");
            enemy.StartPatrol();
        }

        public override void Update()
        {
            
        }

        public override void Transition()
        {
            if (enemy.playerInRange)
            {
                ChangeState(State.Chase);
            }
            else if (!enemy.isPatrolling)
            {
                ChangeState(State.Idle);
            }
        }

        public override void Exit()
        {
            enemy.ledgeChecker.OnReachedEndOfLedge -= ReachedEndOfLedge;
            enemy.StopPatrol();
        }

        private void ReachedEndOfLedge()
        {
            enemy.timer = 0;
            enemy.flipped = true;
        }
    }

    private class ChaseState : EnemyState
    {
        public ChaseState(Enemy enemy)
        {
            this.enemy = enemy;
        }

        public override void Enter()
        {
            Debug.Log("Entered Chase");
            enemy.ledgeChecker.OnReachedEndOfLedge += ReachedEndOfLedge;
            enemy.animator.Play("Walk");
        }

        public override void Update()
        {
            // move towards player
            Vector2 moveDir = enemy.player.transform.position.x < enemy.transform.position.x ? Vector2.left : Vector2.right;
            enemy.rb2d.velocity = moveDir * enemy.moveSpeed; 
            //enemy.transform.Translate(dir * enemy.moveSpeed * Time.deltaTime);
        }

        public override void Transition()
        {
            // if player outside of watch range, idle
            if (!enemy.playerInRange) {
                ChangeState(State.Idle);
            }
            // if player in attack range, attack
            float distToPlayer = (enemy.player.transform.position - enemy.transform.position).sqrMagnitude;
            if(distToPlayer < (enemy.attackRange * enemy.attackRange))
            {
                enemy.rb2d.velocity = Vector2.zero;
                ChangeState(State.Attack);
            }
        }

        public override void Exit()
        {
            enemy.ledgeChecker.OnReachedEndOfLedge -= ReachedEndOfLedge;
            enemy.StopPatrol();
        }

        private void ReachedEndOfLedge()
        {
            ChangeState(State.Idle);
        }
    }

    private class AttackState : EnemyState
    {
        public AttackState(Enemy enemy)
        {
            this.enemy = enemy;
        }

        public override void Enter()
        {
            Debug.Log("Entered Attack");
            enemy.isAttacking = true;
            enemy.FacePlayer();
            enemy.Attack();
        }

        public override void Transition()
        {
            if (!enemy.isAttacking)
            {
                ChangeState(State.Idle);
            }
        }

        public override void Exit()
        {
            enemy.StopAttack();
        }
    }

    private class DeadState : EnemyState
    {
        public DeadState(Enemy enemy)
        {
            this.enemy = enemy;
        }

        public override void Enter()
        {
            Debug.Log("Entered Dead");
            enemy.animator.Play("EnemyDeath");
        }

        public override void Transition()
        {
            Debug.Log(enemy.animator.GetCurrentAnimatorClipInfo(0)[0].clip.name);
            if(enemy.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                enemy.Die();
            }
        }
    }
    #endregion
}


