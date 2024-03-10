using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBody : MonoBehaviour
{
    public enum State { Spawn, Idle, Attack }
    private StateMachine<State> stateMachine = new StateMachine<State>();

    [Header("Rise Values")]
    [SerializeField] float riseTargetY;
    [SerializeField] float riseSpeed;

    [Header("Fall Values")]
    [SerializeField] float fallTargetY;
    [SerializeField] float fallSpeed;

    [Header("Misc")]
    [SerializeField] private VoidEventChannelSO bodySpawned;
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        stateMachine.AddState(State.Spawn, new SpawnState(this));
    }

    private void Start()
    {
        
    }

    public void Rise()
    {
        StartCoroutine(RiseRoutine());
    }

    public void Fall()
    {
        StartCoroutine(FallRoutine());   
    }

    private IEnumerator RiseRoutine()
    {
        float t = 0;
        Vector2 startPos = transform.position;
        Vector2 endPos = transform.position;
        endPos.y = riseTargetY;
        while (t < 1)
        {
            transform.position = Vector2.Lerp(startPos, endPos, t);
            t += Time.deltaTime * riseSpeed;
            yield return null;
        }
        bodySpawned.RaiseEvent();
    }

    private IEnumerator FallRoutine()
    {
        float t = 0;
        Vector2 startPos = transform.position;
        Vector2 endPos = transform.position;
        endPos.y = fallTargetY;
        while (t < 1)
        {
            transform.position = Vector2.Lerp(startPos, endPos, t);
            t += Time.deltaTime * fallSpeed;
            yield return null;
        }
    }

    private class BossBodyState : BaseState<State>
    {
        protected BossBody body;
    }

    private class SpawnState : BossBodyState
    {
        public SpawnState(BossBody body)
        {
            this.body = body;
        }

        public override void Enter()
        {
        }
    }
}
