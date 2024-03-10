using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHand : MonoBehaviour
{
    public enum Type { Left, Right };

    [Header("Event Channels")]
    [SerializeField] private VoidEventChannelSO handSpawned;
    [SerializeField] private VoidEventChannelSO sweepReady;

    [Header("Rise Values")]
    [SerializeField] float targetY;
    [SerializeField] float riseSpeed;

    [Header("Shake Values")]
    [SerializeField] float shakeAmount;
    [SerializeField] float shakeSpeed;
    [SerializeField] float shakeMagnitude;
    [SerializeField] float shakeInterval;

    [Header("Sweep Values")]
    [SerializeField] float distanceToEdge;
    [SerializeField] float leaveSpeed;
    [SerializeField] float sweepDelay;
    [SerializeField] float sweepDistance;
    [SerializeField] float sweepSpeed;

    [Header("Misc")]
    [SerializeField] Type type;
    Animator animator;
    public Animator Animator => animator;
    SpriteRenderer spriter;

    Coroutine shakeRoutine;
    Coroutine riseRoutine;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        if(type == Type.Right)
        {
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
    }

    private void OnEnable()
    {
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void Spawn()
    {
        StopAllCoroutines();
        riseRoutine = StartCoroutine(RiseRoutine());
    }
    
    public void Slide()
    {
        StopAllCoroutines();
        StartCoroutine(SlideRoutine());
    }

    public void Idle()
    {
        StopAllCoroutines();
        StartCoroutine(IdleRoutine());
    }

    public void LeaveScreen()
    {
        StopAllCoroutines();
        leaveScreenRoutine = StartCoroutine(LeaveScreenRoutine());
    }

    public void Sweep()
    {
        if (this == null)
            return;
        StopAllCoroutines();
        StartCoroutine(SweepRoutine());
    }

    private IEnumerator RiseRoutine()
    {
        float t = 0;
        Vector2 startPos = transform.position;
        Vector2 endPos = transform.position;
        endPos.y = targetY;
        while (t < 1)
        {
            transform.position = Vector2.Lerp(startPos, endPos, t);

            // Shaking
            float offsetX = Mathf.Sin(Time.time * shakeSpeed) * shakeMagnitude;
            transform.position = new Vector2(transform.position.x + offsetX, transform.position.y);

            t += Time.deltaTime * riseSpeed;
            yield return null;
        }
        animator.Play("Rest");
        spriter.sortingLayerID = SortingLayer.NameToID("Default");
        
        if(type == Type.Right)
        {
            handSpawned.RaiseEvent();
        }
    }

    private IEnumerator SlideRoutine()
    {
        float t = 0;
        while (t < 1)
        {
            transform.Translate(-transform.right * Time.deltaTime, Space.World);
            t += Time.deltaTime * 2;
            yield return null;
        }
    }

    private IEnumerator IdleRoutine()
    {
        animator.Play("Phase1Idle");

        float t = 0;
        while (t < 1)
        {
            transform.Translate(transform.right * Time.deltaTime, Space.World);
            transform.Translate(transform.up * Time.deltaTime, Space.World);
            t += Time.deltaTime * 2;
            yield return null;
        }
        
        yield return null;
    }

    private IEnumerator Slam()
    {
        yield return null;
    }

    Coroutine leaveScreenRoutine;
    private IEnumerator LeaveScreenRoutine()
    {
        animator.Play("Phase1Sweep");
        Vector2 targetPos = transform.position + -transform.right * distanceToEdge;
        while (Vector2.Distance(transform.position, targetPos) > 0.01)
        {
            float step = leaveSpeed * Time.deltaTime;
            
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
            yield return null;
        }
        sweepReady.RaiseEvent();
    }

    private IEnumerator SweepRoutine()
    {
        Vector2 targetPos = transform.position + transform.right * sweepDistance;
        while (Vector2.Distance(transform.position, targetPos) > 0.01)
        {
            float step = sweepSpeed * Time.deltaTime;
            Debug.Log(step);
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
            yield return null;
        }
    }

    private IEnumerator Respawn()
    {
        yield return null;
    }
    private IEnumerator LerpToDestination(Transform transform, Vector2 offset, float speed)
    {
        float t = 0;
        Vector2 startPos = transform.localPosition;
        Vector2 endPos = -transform.right;
        endPos += offset;
        while (t < 1)
        {
            transform.localPosition = Vector2.Lerp(startPos, endPos, t);
            t += Time.deltaTime * speed;
            yield return null;
        }
    }
}
