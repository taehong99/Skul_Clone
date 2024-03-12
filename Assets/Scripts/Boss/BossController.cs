using Cinemachine;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public enum Side { Left, Right }
public class BossController : MonoBehaviour, IDamageable
{
    public enum State { Spawn, Phase1, Transition, Phase2, Dead }
    private StateMachine<State> stateMachine = new StateMachine<State>();

    [Header("Boss Values")]
    [SerializeField] int startingHP;
    [SerializeField] float phase1SweepCD;
    [SerializeField] float phase1SlamCD;
    [SerializeField] float phase2SweepCD;
    [SerializeField] float phase2SlamCD;

    private int hp;
    public int HP { get { return hp; } private set { hp = value; OnHPChanged?.Invoke(value); } }
    public UnityAction<int> OnHPChanged;
    public UnityAction OnFirstDeath;
    public UnityAction OnSecondDeath;
    private bool immune;
    private bool hasDied;

    BossBody body;
    BossHead head;
    BossHands hands;

    private void Awake()
    {
        hp = startingHP;
        body = GetComponentInChildren<BossBody>();
        head = GetComponentInChildren<BossHead>();
        hands = GetComponentInChildren<BossHands>();
    }

    private void Start()
    {
        stateMachine.AddState(State.Spawn, new SpawnState(this));
        stateMachine.AddState(State.Phase1, new Phase1State(this));
        stateMachine.AddState(State.Transition, new TransitionState(this));
        stateMachine.AddState(State.Phase2, new Phase2State(this));
        stateMachine.AddState(State.Dead, new DeadState(this));
        stateMachine.Start(State.Spawn);
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;
        if (HP <= 0)
        {
            if (!hasDied)
            {
                Manager.Events.voidEventDic["whiteFlash"].RaiseEvent();
                stateMachine.ChangeState(State.Transition);
                OnFirstDeath?.Invoke();
                hasDied = true;
            }
            else
            {
                stateMachine.ChangeState(State.Dead);
                OnSecondDeath?.Invoke();
            }
        }
    }

    private class BossState : BaseState<State>
    {
        protected BossController boss;
    }

    private class SpawnState : BossState
    {
        public SpawnState(BossController boss)
        {
            this.boss = boss;
        }

        public override void Enter()
        {
            boss.head.SetHurtBox(false);
            spawnRoutine = boss.StartCoroutine(SpawnRoutine());
        }

        Coroutine spawnRoutine;
        private IEnumerator SpawnRoutine() // Spawn Animation
        {
            yield return boss.StartCoroutine(boss.hands.SpawnRoutine());
            yield return new WaitForSeconds(0.5f);
            yield return boss.StartCoroutine(boss.body.RiseRoutine());
            boss.StartCoroutine(boss.body.FallRoutine());
            boss.StartCoroutine(boss.hands.SlideRoutine());
            yield return boss.StartCoroutine(boss.head.SpawnRoutine());

            ChangeState(State.Phase1);
        }

        public override void Exit()
        {
            boss.StopCoroutine(spawnRoutine);
        }
    }

    private class Phase1State : BossState
    {
        public Phase1State(BossController boss)
        {
            this.boss = boss;
        }

        public override void Enter()
        {
            boss.head.SetHurtBox(true);
            phase1Routine = boss.StartCoroutine(Phase1Routine());
        }

        Coroutine phase1Routine;
        private IEnumerator Phase1Routine()
        {
            boss.StartCoroutine(boss.head.IdleRoutine());
            boss.StartCoroutine(boss.hands.IdleRoutine());
            for (int i = 0; i < 3; i++) // TODO: set to while(true) until phase1 hp == 0
            {
                yield return boss.StartCoroutine(boss.body.IdleRoutine());
                yield return new WaitForSeconds(1f);
                //yield return boss.StartCoroutine(StartAttack(Random.Range(0, 2))); // TODO: Set to random.range
                yield return boss.StartCoroutine(StartAttack(0));
            }
        }

        private IEnumerator StartAttack(int randomAttack)
        {
            if (randomAttack == 0) // Sweep
            {
                yield return boss.StartCoroutine(boss.hands.PrepareSweepRoutine());
                for (int i = 0; i < 2; i++)
                {
                    Side side = Manager.Game.Player.transform.position.x <= 0 ? Side.Left : Side.Right;
                    boss.StartCoroutine(boss.body.PrepareSweepRoutine(side));
                    yield return boss.StartCoroutine(boss.head.PrepareSweepRoutine(side));
                    boss.StartCoroutine(boss.body.SweepRoutine(side));
                    boss.StartCoroutine(boss.head.MoveToIdleRoutine());
                    boss.StartCoroutine(boss.head.ScreamRoutine());
                    yield return boss.StartCoroutine(boss.hands.SweepRoutine(side));
                    yield return boss.StartCoroutine(boss.body.MoveToIdleRoutine());
                    yield return new WaitForSeconds(boss.phase1SweepCD);
                }
                yield return boss.StartCoroutine(boss.hands.BackToIdleRoutine(0));
            }
            else // Slam
            {
                for (int i = 0; i < 2; i++)
                {
                    //body rise + shake + hands rise
                    boss.StartCoroutine(boss.body.SlamRiseRoutine());
                    yield return boss.StartCoroutine(boss.hands.PrepareSlamRoutine());

                    yield return new WaitForSeconds(boss.phase1SlamCD);

                    //body dip + hand slam player x2
                    Side side = Manager.Game.Player.transform.position.x <= 0 ? Side.Left : Side.Right;
                    boss.StartCoroutine(boss.body.SlamDipRoutine());
                    yield return boss.StartCoroutine(boss.hands.SlamRoutine(side));
                }

                yield return boss.StartCoroutine(boss.hands.PrepareSlamRoutine());
                yield return new WaitForSeconds(0.5f);
                yield return boss.StartCoroutine(boss.hands.BackToIdleRoutine(1));
            }
        }

        public override void Exit()
        {
            boss.StopAllCoroutines();
        }
    }

    private class TransitionState : BossState
    {
        public TransitionState(BossController boss)
        {
            this.boss = boss;
        }
        public override void Enter()
        {
            boss.head.SetHurtBox(false);
            transitionRoutine = boss.StartCoroutine(TransitionRoutine());
        }

        Coroutine transitionRoutine;
        private IEnumerator TransitionRoutine()
        {
            // Freeze for a bit
            boss.StartCoroutine(boss.body.TransitionFreezeRoutine());
            boss.StartCoroutine(boss.hands.TransitionFreezeRoutine());
            boss.StartCoroutine(boss.head.TransitionFreezeRoutine());

            yield return new WaitForSeconds(1f);

            // Smash ground and become purple
            boss.StartCoroutine(boss.body.SlamRiseRoutine());
            boss.StartCoroutine(boss.head.MoveToIdleRoutine());
            yield return boss.StartCoroutine(boss.hands.PrepareSlamRoutine());
            yield return new WaitForSeconds(0.5f);
            boss.StartCoroutine(boss.body.SlamDipRoutine());
            yield return boss.StartCoroutine(boss.hands.TransitionSlamRoutine());
            Manager.Events.voidEventDic["whiteFlash"].RaiseEvent();
            Manager.Events.voidEventDic["phase2Started"].RaiseEvent();
            yield return boss.StartCoroutine(boss.head.ScreamRoutine());
            yield return new WaitForSeconds(1);
            ChangeState(State.Phase2);
        }

        public override void Exit()
        {
            boss.StopCoroutine(transitionRoutine);
            
        }
    }

    private class Phase2State : BossState
    {
        public Phase2State(BossController boss)
        {
            this.boss = boss;
        }

        public override void Enter()
        {
            boss.head.SetHurtBox(true);
            boss.HP = boss.startingHP;
            boss.StartCoroutine(Phase2Routine());
        }
        private IEnumerator Phase2Routine()
        {
            boss.StartCoroutine(boss.head.IdleRoutine());
            boss.StartCoroutine(boss.hands.IdleRoutine());
            for (int i = 0; i < 3; i++) // TODO: set to while(true) until phase1 hp == 0
            {
                yield return boss.StartCoroutine(boss.body.IdleRoutine());
                yield return new WaitForSeconds(1f);
                //yield return boss.StartCoroutine(StartAttack(Random.Range(0, 2))); // TODO: Set to random.range
                yield return boss.StartCoroutine(StartAttack(1));
            }
        }

        private IEnumerator StartAttack(int randomAttack)
        {
            if (randomAttack == 0) // Sweep
            {
                yield return boss.StartCoroutine(boss.hands.PrepareSweepRoutine());
                for (int i = 0; i < 2; i++)
                {
                    Side side = Manager.Game.Player.transform.position.x <= 0 ? Side.Left : Side.Right;
                    boss.StartCoroutine(boss.body.PrepareSweepRoutine(side));
                    yield return boss.StartCoroutine(boss.head.PrepareSweepRoutine(side));
                    boss.StartCoroutine(boss.body.SweepRoutine(side));
                    boss.StartCoroutine(boss.head.MoveToIdleRoutine());
                    boss.StartCoroutine(boss.head.ScreamRoutine());
                    yield return boss.StartCoroutine(boss.hands.SweepRoutine(side));
                    yield return boss.StartCoroutine(boss.body.MoveToIdleRoutine());
                    yield return new WaitForSeconds(boss.phase2SweepCD);
                }
                yield return boss.StartCoroutine(boss.hands.BackToIdleRoutine(0));
            }
            else // Slam
            {
                for (int i = 0; i < 2; i++)
                {
                    //body rise + shake + hands rise
                    boss.StartCoroutine(boss.body.SlamRiseRoutine());
                    yield return boss.StartCoroutine(boss.hands.PrepareSlamRoutine());

                    yield return new WaitForSeconds(boss.phase2SlamCD);

                    //body dip + hand slam player x2
                    Side side = Manager.Game.Player.transform.position.x <= 0 ? Side.Left : Side.Right;
                    boss.StartCoroutine(boss.body.SlamDipRoutine());
                    yield return boss.StartCoroutine(boss.hands.SlamRoutine(side));
                }

                yield return boss.StartCoroutine(boss.hands.PrepareSlamRoutine());
                yield return new WaitForSeconds(0.5f);
                yield return boss.StartCoroutine(boss.hands.BackToIdleRoutine(1));
            }
        }
    }
    private class DeadState : BossState
    {
        public DeadState(BossController boss)
        {
            this.boss = boss;
        }
        public override void Enter()
        {
            boss.head.SetHurtBox(false);
        }
    }
}
