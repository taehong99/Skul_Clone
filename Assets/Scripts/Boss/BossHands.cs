using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BossHands : MonoBehaviour
{
    const float handSpawnDelay = 1.5f;

    public enum State { Spawn, Idle, Attack }
    private StateMachine<State> stateMachine = new StateMachine<State>();

    private BossHand leftHand;
    private BossHand rightHand;

    private void Awake()
    {
        leftHand = transform.GetChild(0).GetComponent<BossHand>();
        rightHand = transform.GetChild(1).GetComponent<BossHand>();
        stateMachine.AddState(State.Spawn, new SpawnState(this));
    }

    private void Start()
    {
        
    }

    #region Actions
    // Actions called by BossController
    public void Spawn()
    {
        stateMachine.Start(State.Spawn);
    }

    public void Slide()
    {
        Debug.Log("Called");
        leftHand.Slide();
        rightHand.Slide();
    }

    #endregion

    private IEnumerator SpawnRoutine()
    {
        leftHand.Spawn();
        yield return new WaitForSeconds(handSpawnDelay);
        rightHand.Spawn();
    }

    private class BossHandState : BaseState<State>
    {
        protected BossHands hands;
    }

    private class SpawnState : BossHandState
    {
        public SpawnState(BossHands hands)
        {
            this.hands = hands;
        }

        public override void Enter()
        {
            hands.StartCoroutine(hands.SpawnRoutine());
        }
    }
}
