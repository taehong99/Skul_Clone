using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHand : BossBodyPart
{
    public enum Type { Left, Right };

    [Header("Rise Values")]
    [SerializeField] Vector2 hidePosition;
    [SerializeField] Vector2 idlePosition;
    [SerializeField] float targetY;
    [SerializeField] float riseSpeed;
    [SerializeField] float grabShakeDuration;

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

    [Header("Slam Values")]
    [SerializeField] Vector2 slamStartPos;
    [SerializeField] float slamSpeed;
    [SerializeField] float slamColliderRadius;
    [SerializeField] float slamShakeDuration;

    [Header("Dead Values")]
    [SerializeField] Vector2 deadPosition;

    [Header("Misc")]
    [SerializeField] Type type;
    [SerializeField] int damage;
    [SerializeField] Vector3 originalRotation;
    RuntimeAnimatorController originalController;
    [SerializeField] RuntimeAnimatorController phase2Controller;
    Animator animator;
    public Animator Animator => animator;
    SpriteRenderer spriter;
    BoxCollider2D hitbox;

    Coroutine shakeRoutine;
    Coroutine riseRoutine;

    private void Start()
    {
        animator = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        hitbox = GetComponent<BoxCollider2D>();
        originalController = animator.runtimeAnimatorController;
        hitbox.enabled = false;
        Manager.Events.voidEventDic["phase2Started"].OnEventRaised += TransformP2;
        Manager.Events.voidEventDic["bossDefeated"].OnEventRaised += TransformDead;
    }

    public void StopCoroutines()
    {
        StopAllCoroutines();
        hitbox.enabled = false;
        Manager.Game.Shaker.StopShake();
    }

    public void TransformP2()
    {
        animator.runtimeAnimatorController = phase2Controller;
    }

    public void TransformDead()
    {
        animator.runtimeAnimatorController = originalController;
        animator.Play("Rest");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        IDamageable[] damageables = collision.transform.GetComponents<IDamageable>();
        foreach(IDamageable damageable in damageables)
        {
            damageable.TakeDamage(damage);
        }
    }

    public IEnumerator SpawnRiseRoutine()
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
        Manager.Game.Shaker.Shake(grabShakeDuration);
    }

    public IEnumerator SpawnSlideRoutine()
    {
        float t = 0;
        while (t < 1)
        {
            transform.Translate(-transform.right * Time.deltaTime, Space.World);
            t += Time.deltaTime * 2;
            yield return null;
        }
    }

    public IEnumerator IdleRoutine()
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
    }

    public IEnumerator BackToIdleRoutine(int i)
    {
        if(i == 0) // sweep version
        {
            animator.Play("Phase1Idle");
            spriter.sortingLayerID = SortingLayer.NameToID("Background");
            transform.localPosition = hidePosition;
            yield return StartCoroutine(LerpToDestination(transform, idlePosition, riseSpeed));
            spriter.sortingLayerID = SortingLayer.NameToID("Default");
        }
        else // slam version
        {
            animator.Play("Phase1Idle");
            yield return StartCoroutine(LerpToDestination(transform, idlePosition, riseSpeed));
        }
    }

    // Hand Sweep
    public IEnumerator LeaveScreenRoutine()
    {
        animator.Play("Phase1Sweep");
        Vector2 targetPos = transform.position + -transform.right * distanceToEdge;
        while (Vector2.Distance(transform.position, targetPos) > 0.01)
        {
            float step = leaveSpeed * Time.deltaTime;
            
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
            yield return null;
        }
        if(type == Type.Right)
        {
            Side attackDir = Manager.Game.Player.transform.position.x <= 0 ? Side.Left : Side.Right;
        }
    }
    public IEnumerator SweepRoutine()
    {
        hitbox.enabled = true;

        Vector2 origin = transform.position;
        Vector2 targetPos = transform.position + transform.right * sweepDistance;
        while (Vector2.Distance(transform.position, targetPos) > 0.01)
        {
            float step = sweepSpeed * Time.deltaTime;
            transform.position = Vector3.MoveTowards(transform.position, targetPos, step);
            yield return null;
        }
        transform.position = origin;
        hitbox.enabled = false;
    }

    // Hand Slam
    public IEnumerator PrepareSlamRoutine()
    {
        if(type == Type.Left)
            animator.Play("Phase1LeftSlam");
        else
            animator.Play("Phase1RightSlam");

        transform.rotation = Quaternion.Euler(originalRotation);
        yield return StartCoroutine(LerpToDestination(transform, slamStartPos, riseSpeed)); 
    }
    public IEnumerator SlamRoutine(Vector2 targetPos)
    {
        // Transition slam
        if (targetPos == Vector2.zero)
        {
            Vector2 destination = transform.position;
            destination.y = -1;
            yield return StartCoroutine(LerpToDestination(transform, destination, slamSpeed));
            yield break;
        }

        yield return StartCoroutine(LerpToDestination(transform, targetPos, slamSpeed));
        Manager.Game.Shaker.Shake(slamShakeDuration);

        Collider2D collider = Physics2D.OverlapCircle(transform.position, slamColliderRadius, Manager.Game.Player.Mask);
        IDamageable[] damageables = collider.transform.GetComponents<IDamageable>();
        foreach(IDamageable damageable in damageables)
        {
            damageable.TakeDamage(damage);
        }
    }

    // Hand Phase Transition
    public void TransitionFreezeRoutine()
    {
        animator.Play("Phase1Sweep");
        transform.localPosition = idlePosition;
        transform.localRotation = Quaternion.Euler(0, transform.localRotation.y * 180, -45);
    }

    // Phase 2 Special Attack
    public IEnumerator GrabFloor()
    {
        yield return StartCoroutine(LerpToDestination(transform, new Vector2(transform.position.x, targetY), riseSpeed));
        animator.Play("Rest");
        yield return null;
    }

    // Hand Phase Transition
    public void TransitionDeadRoutine()
    {
        animator.Play("Rest");
        transform.localPosition = deadPosition;
        transform.localRotation = Quaternion.Euler(0, transform.localRotation.y * 180, 0);
    }
}
