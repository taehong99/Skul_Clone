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

    [SerializeField] private VoidEventChannelSO sweepReady;

    private void Awake()
    {
        leftHand = transform.GetChild(0).GetComponent<BossHand>();
        rightHand = transform.GetChild(1).GetComponent<BossHand>();
    }

    #region Actions
    // Actions called by BossController
    public void Spawn()
    {
        StartCoroutine(SpawnRoutine());
    }

    public void Slide()
    {
        leftHand.Slide();
        rightHand.Slide();
    }

    public void Idle()
    {
        leftHand.Idle();
        rightHand.Idle();
    }

    public void SweepAttack()
    {
        PrepareSweep();
        if (Manager.Game.Player.transform.position.x <= 0)
        {
            sweepReady.OnEventRaised += leftHand.Sweep;
        }
        else
        {
            sweepReady.OnEventRaised += rightHand.Sweep;
        }
    }

    public void PrepareSweep()
    {
        leftHand.LeaveScreen();
        rightHand.LeaveScreen();
    }
    #endregion

    private IEnumerator SpawnRoutine()
    {
        leftHand.Spawn();
        yield return new WaitForSeconds(handSpawnDelay);
        rightHand.Spawn();
    }
}
