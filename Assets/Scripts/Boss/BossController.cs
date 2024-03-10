using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class BossController : MonoBehaviour
{
    public enum State { Spawn, Phase1, Phase2, Dead }
    private StateMachine<State> stateMachine = new StateMachine<State>();

    [Header("Event Channels")]
    [SerializeField] private VoidEventChannelSO handSpawned;
    [SerializeField] private VoidEventChannelSO bodySpawned;
    [SerializeField] private VoidEventChannelSO screamFinished;
    [SerializeField] private VoidEventChannelSO idleFinished;

    [SerializeField] int hp;

    BossBody body;
    BossHead head;
    BossHands hands;

    private void Awake()
    {
        body = GetComponentInChildren<BossBody>();
        head = GetComponentInChildren<BossHead>();
        hands = GetComponentInChildren<BossHands>();
    }

    private void Start()
    {
        stateMachine.AddState(State.Spawn, new SpawnState(this));
        stateMachine.AddState(State.Phase1, new Phase1State(this));
        stateMachine.Start(State.Spawn);
    }

    private void OnDisable()
    {
        // Unsubscribe
        handSpawned.OnEventRaised = null;
        bodySpawned.OnEventRaised = null;
        screamFinished.OnEventRaised = null;
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
            // Spawn Actions
            boss.handSpawned.OnEventRaised += boss.body.Rise;
            boss.bodySpawned.OnEventRaised += boss.hands.Slide;
            boss.bodySpawned.OnEventRaised += boss.head.Scream;
            boss.bodySpawned.OnEventRaised += boss.body.Fall;
            boss.screamFinished.OnEventRaised += Transition;

            boss.hands.Spawn();
        }

        public override void Transition()
        {
            ChangeState(State.Phase1);
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
            StartIdle();
        }

        private void StartIdle()
        {
            boss.idleFinished.OnEventRaised += StartAttack;
            boss.hands.Idle();
            boss.body.Idle();
            boss.head.Idle();
        }

        private void StartAttack()
        {
            if (boss.hands != null)
            {
                boss.hands.SweepAttack();
            }
        }
    }
}
