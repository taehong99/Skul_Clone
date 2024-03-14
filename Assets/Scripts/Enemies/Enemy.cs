using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour, IDamageable
{
    public enum State { Idle, Patrol, Chase, Stance, Attack, Dead }
    protected StateMachine<State> stateMachine = new StateMachine<State>();

    [Header("Data")]
    [SerializeField] protected EnemyData data;

    [Header("Patrol")]
    private bool playerInRange;

    [Header("Attack")]
    [SerializeField] protected float stanceDuration;
    protected bool isAttacking;

    [Header("Death")]
    private PooledObject deathEffectPrefab;

    [Header("Misc")]
    private int hp;
    public int HP { get { return hp; } private set { hp = value; OnHPChanged?.Invoke(value); } }
    public event UnityAction<int> OnHPChanged;
    public event UnityAction OnFlipped;
    
    private bool isPatrolling;

    protected PlayerController player;
    protected Animator animator;
    protected Rigidbody2D rb2d;
    private LedgeChecker ledgeChecker;
    private PooledObject enemyHitEffectPrefab;

    private void Awake()
    {
        HP = data.baseHP;
        animator = GetComponent<Animator>();
        rb2d = GetComponent<Rigidbody2D>();
        ledgeChecker = GetComponentInChildren<LedgeChecker>();
        enemyHitEffectPrefab = Manager.Resource.Load<PooledObject>("Prefabs/EnemyHitEffect");
        deathEffectPrefab = Manager.Resource.Load<PooledObject>("Prefabs/EnemyEffect");

        GameObject playerChecker = new GameObject("PlayerChecker");
        playerChecker.transform.SetParent(transform);
        playerChecker.transform.localPosition = Vector3.zero;
        CircleCollider2D collider = playerChecker.AddComponent<CircleCollider2D>();
        collider.isTrigger = true;
        collider.radius = data.playerCheckRange;
    }

    protected virtual void Start()
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
        if (player.mask.Contains(collision.gameObject.layer))
        {
            playerInRange = true;
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (player.mask.Contains(collision.gameObject.layer))
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
        Manager.Pool.GetPool(enemyHitEffectPrefab, transform.position, Quaternion.identity);

        HP -= damage;
        if (HP <= 0)
        {
            stateMachine.ChangeState(State.Dead);
            return;
        }
        
        KnockBack();
    }

    private int animIndex = 0;
    public void KnockBack()
    {
        if (data.knockbackForce == 0)
            return;
        
        Vector2 knockDirection = Manager.Game.Player.facingDir == PlayerController.FacingDir.Left ? Vector2.left : Vector2.right;
        rb2d.AddForce(knockDirection * data.knockbackForce, ForceMode2D.Impulse);
        stateMachine.ChangeState(State.Idle);
        animator.Play($"Hit{animIndex}");
        animIndex ^= 1;
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
        timer = Random.Range(data.patrolTime - 1, data.patrolTime + 1.5f);
        Vector2 firstDir = Random.Range(0, 2) == 0 ? Vector2.right : Vector2.left;
        Vector2 secondDir = -firstDir;
        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            rb2d.velocity = firstDir * data.moveSpeed;
            //transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            yield return null;
        }
        rb2d.velocity = Vector2.zero;

        if (!flipped)
        {
            FlipOnDemand();
        }

        timer = Random.Range(data.patrolTime - 1, data.patrolTime + 1.5f);
        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            rb2d.velocity = secondDir * data.moveSpeed;
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
        GetComponent<Rigidbody2D>().excludeLayers = player.mask;
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, data.playerCheckRange);
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
            timer = enemy.data.patienceTimer;
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
            enemy.rb2d.velocity = moveDir * enemy.data.moveSpeed; 
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
            if(distToPlayer < (enemy.data.attackRange * enemy.data.attackRange))
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
            DeathEffect();
            Manager.Events.voidEventDic["enemyKilled"].RaiseEvent();
        }

        private void DeathEffect()
        {
            GameObject effect = Manager.Pool.GetPool(enemy.deathEffectPrefab, enemy.transform.position, Quaternion.identity).gameObject;
            enemy.transform.SetParent(effect.transform);
            effect.GetComponent<Animator>().Play("EnemyDeath");
            Destroy(enemy.gameObject);
        }
    }
    #endregion
}


