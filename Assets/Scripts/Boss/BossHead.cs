using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BossHead : MonoBehaviour
{
    public enum State { Spawn, Idle, Attack }
    private StateMachine<State> stateMachine = new StateMachine<State>();

    [Header("Spawn Values")]
    [SerializeField] float nodSpeed;
    [SerializeField] float nodDuration;
    [SerializeField] float jawOpenGap;
    [SerializeField] float jawOpenSpeed;
    [SerializeField] float screamDuration;
    [SerializeField] float shakeSpeed;
    [SerializeField] float shakeMagnitude;

    [Header("Misc")]
    [SerializeField] private VoidEventChannelSO screamFinished;
    private BossFace face;
    private BossJaw jaw;

    private void Awake()
    {
        face = GetComponentInChildren<BossFace>();
        jaw = GetComponentInChildren<BossJaw>();
    }

    public void Scream()
    {
        StartCoroutine(ScreamRoutine());
    }

    private IEnumerator ScreamRoutine()
    {
        float t = 0;
        while (t < nodDuration)
        {
            transform.Translate(-transform.up * Time.deltaTime * nodSpeed, Space.World);
            t += Time.deltaTime * nodSpeed;
            yield return null;
        }

        headShakeRoutine = StartCoroutine(HeadShakeRoutine());
        yield return(StartCoroutine(JawOpenRoutine()));
        StopCoroutine(headShakeRoutine);

        screamFinished.RaiseEvent();
    }

    Coroutine headShakeRoutine;
    private IEnumerator HeadShakeRoutine()
    {
        float t = 0;
        while(true)
        {
            float offsetX = Mathf.Sin(Time.time * shakeSpeed) * shakeMagnitude;
            float offsetY = Mathf.Cos(Time.time * shakeSpeed) * shakeMagnitude;
            transform.position = new Vector2(transform.position.x + offsetX, transform.position.y + offsetY);

            t += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator JawOpenRoutine()
    {
        float t = 0;
        while (t < 1)
        {
            jaw.transform.Translate(-transform.up * Time.deltaTime * jawOpenGap, Space.World);
            t += Time.deltaTime * jawOpenSpeed;
            yield return null;
        }
        yield return new WaitForSeconds(screamDuration);
        t = 0;
        while (t < 1)
        {
            jaw.transform.Translate(transform.up * Time.deltaTime * jawOpenGap, Space.World);
            t += Time.deltaTime * jawOpenSpeed;
            yield return null;
        }
    }

    // States
    private class BossHeadState : BaseState<State>
    {
        protected BossHead head;
    }

    private class SpawnState : BossHeadState
    {
        public SpawnState(BossHead head)
        {
            this.head = head;
        }

        public override void Enter()
        {

        }
    }
}
