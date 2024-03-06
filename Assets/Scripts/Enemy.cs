using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour, IDamageable
{
    public enum State { Idle, Patrol, Chase, Stance, Attack, Dead }
    private StateMachine<State> stateMachine = new StateMachine<State>();

    [Header("Stats")]
    [SerializeField] int baseHP;
    [SerializeField] float moveSpeed;
    [SerializeField] float knockbackForce;

    [Header("Patrol")]
    [SerializeField] float patienceTimer; // how long before going from idle to patrol
    [SerializeField] float patrolTime; // patrol time for each direction

    [Header("Chase")]
    [SerializeField] LayerMask playerMask;

    [Header("Attack")]
    [SerializeField] float attackRange;
    [SerializeField] float stanceDuration;
    [SerializeField] float damage;

    [Header("Misc")]
    private int hp;
    public int HP { get { return hp; } private set { hp = value; OnHPChanged?.Invoke(value); } }
    public event UnityAction<int> OnHPChanged;
    private int animIndex = 0;
    private bool isPatrolling;

    private PlayerController player;
    private Animator animator;

    private void Awake()
    {
        HP = baseHP;
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        player = Manager.Game.Player;

        stateMachine.AddState(State.Idle, new IdleState(this));
        stateMachine.AddState(State.Patrol, new PatrolState(this));
        stateMachine.AddState(State.Chase, new ChaseState(this));
        stateMachine.AddState(State.Stance, new StanceState(this));
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
            stateMachine.ChangeState(State.Chase);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (playerMask.Contains(collision.gameObject.layer))
        {
            stateMachine.ChangeState(State.Idle);
        }
    }

    #region Methods
    private void Flip()
    {
        //
    }

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
        Vector2 knockDirection = Manager.Game.Player.facingDir == PlayerController.FacingDir.Left ? Vector2.left : Vector2.right;
        transform.Translate(Vector2.Lerp(Vector2.zero, knockDirection, knockbackForce));
    }

    public void StartPatrol()
    {
        StartCoroutine(Patrol());
    }
    private IEnumerator Patrol()
    {
        isPatrolling = true;
        float timer = patrolTime;

        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
            yield return null;
        }
        timer = patrolTime;
        while (timer >= 0)
        {
            timer -= Time.deltaTime;
            transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
            yield return null;
        }
        stateMachine.ChangeState(State.Idle);
        isPatrolling = false;
    }

    private void Die() {
        GetComponent<Rigidbody2D>().excludeLayers = playerMask;
        Destroy(gameObject);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
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
            enemy.animator.Play("Attack");
            enemy.StartPatrol();
        }

        public override void Transition()
        {
            if (!enemy.isPatrolling)
            {
                ChangeState(State.Idle);
            }
        }
    }

    private class ChaseState : EnemyState
    {
        public ChaseState(Enemy enemy)
        {
            this.enemy = enemy;
        }

        public override void Update()
        {
            // move towards player
            Vector2 dir = enemy.player.transform.position.x < enemy.transform.position.x ? Vector2.left : Vector2.right;
            enemy.transform.Translate(dir * enemy.moveSpeed * Time.deltaTime);
        }

        public override void Transition()
        {
            // if player in range, attack
            float distToPlayer = (enemy.player.transform.position - enemy.transform.position).sqrMagnitude;
            if(distToPlayer < (enemy.attackRange * enemy.attackRange))
            {
                ChangeState(State.Stance);
            }
        }
    }

    private class StanceState : EnemyState
    {
        float time;

        public StanceState(Enemy enemy)
        {
            this.enemy = enemy;
        }

        public override void Enter()
        {
            time = 0;
            enemy.animator.Play("Stance");
        }

        public override void Update() {
            time += Time.deltaTime;
        }

        public override void Transition()
        {
            if(time >= enemy.stanceDuration)
            {
                ChangeState(State.Attack);
            }
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
            enemy.animator.Play("Attack");
        }

        public override void Transition()
        {
            if(enemy.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
            {
                ChangeState(State.Chase);
            }
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


