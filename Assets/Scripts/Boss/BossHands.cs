using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BossHands : MonoBehaviour
{
    const float handSpawnDelay = 1.5f;

    public BossHand leftHand;
    public BossHand rightHand;

    private void Awake()
    {
        leftHand = transform.GetChild(0).GetComponent<BossHand>();
        rightHand = transform.GetChild(1).GetComponent<BossHand>();
    }

    public void StopCoroutines()
    {
        StopAllCoroutines();
        leftHand.StopCoroutines();
        rightHand.StopCoroutines();
    }

    // Hands Spawn
    public IEnumerator SpawnRoutine()
    {
        StartCoroutine(leftHand.SpawnRiseRoutine());
        yield return new WaitForSeconds(handSpawnDelay);
        yield return StartCoroutine(rightHand.SpawnRiseRoutine());
    }
    public IEnumerator SlideRoutine()
    {
        StartCoroutine(leftHand.SpawnSlideRoutine());
        yield return StartCoroutine(rightHand.SpawnSlideRoutine());
    }
    public IEnumerator IdleRoutine()
    {
        StartCoroutine(leftHand.IdleRoutine());
        yield return StartCoroutine(rightHand.IdleRoutine());
    }
    public IEnumerator BackToIdleRoutine(int i)
    {
        StartCoroutine(leftHand.BackToIdleRoutine(i));
        yield return StartCoroutine(rightHand.BackToIdleRoutine(i));
    }

    // Hands Sweep
    public IEnumerator PrepareSweepRoutine()
    {
        StartCoroutine(leftHand.LeaveScreenRoutine());
        yield return StartCoroutine(rightHand.LeaveScreenRoutine());
    }
    public IEnumerator SweepRoutine(Side side)
    {
        if (side == Side.Left)
        {
            yield return StartCoroutine(leftHand.SweepRoutine());
        }
        else
        {
            yield return StartCoroutine(rightHand.SweepRoutine());
        }
    }

    // Hands Slam
    public IEnumerator PrepareSlamRoutine()
    {
        StartCoroutine(leftHand.PrepareSlamRoutine());
        yield return StartCoroutine(rightHand.PrepareSlamRoutine());
    }
    public IEnumerator SlamRoutine(Side side)
    {
        Vector2 targetPos = Manager.Game.Player.transform.position;
        targetPos.y = -1;

        if (side == Side.Left)
        {
            yield return StartCoroutine(leftHand.SlamRoutine(targetPos));
        }
        else
        {
            yield return StartCoroutine(rightHand.SlamRoutine(targetPos));
        }
    }

    // Hands Phase Transition
    public void TransitionFreezeRoutine()
    {
        leftHand.TransitionFreezeRoutine();
        rightHand.TransitionFreezeRoutine();
    }

    public IEnumerator TransitionSlamRoutine()
    {
        StartCoroutine(leftHand.SlamRoutine(Vector2.zero));
        yield return StartCoroutine(rightHand.SlamRoutine(Vector2.zero));
    }

    // Hands Phase 2 Special Attack
    public IEnumerator GrabFloor()
    {
        StartCoroutine(leftHand.GrabFloor());
        yield return StartCoroutine(rightHand.GrabFloor());
    }

    // Hands Death Transition
    public void TransitionDeadRoutine()
    {
        leftHand.TransitionDeadRoutine();
        rightHand.TransitionDeadRoutine();
    }
}
