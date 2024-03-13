using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBody : BossBodyPart
{
    [Header("Rise Values")]
    [SerializeField] float riseTargetY;
    [SerializeField] float riseSpeed;

    [Header("Fall Values")]
    [SerializeField] float fallTargetY;
    [SerializeField] float fallSpeed;

    [Header("Idle Values")]
    [SerializeField] Vector2 idleTargetPos;
    [SerializeField] float idleSpeed;
    [SerializeField] float idleOffset;

    [Header("Sweep Attack Values")]
    [SerializeField] float leanOffset;
    [SerializeField] float leanSpeed;
    [SerializeField] float tiltAngle;

    [Header("Slam Attack Values")]
    [SerializeField] float slamRiseTargetY;
    [SerializeField] float slamRiseSpeed;
    [SerializeField] float slamDipSpeed;
    [SerializeField] float shakeSpeed;
    [SerializeField] float shakeMagnitude;

    [Header("Dead Values")]
    [SerializeField] float faintSpeed;

    [Header("Misc")]
    [SerializeField] Sprite phase2Sprite;
    [SerializeField] Sprite deadSprite;
    Animator animator;
    SpriteRenderer spriter;
    BossPlatforms platforms;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
        
    }

    private void Start()
    {
        platforms = GetComponentInChildren<BossPlatforms>();
        platforms.SetPlatforms(false);
        Manager.Events.voidEventDic["phase2Started"].OnEventRaised += TransformP2;
        Manager.Events.voidEventDic["bossDefeated"].OnEventRaised += TransformDead;
    }

    public void TransformP2()
    {
        spriter.sprite = phase2Sprite;
    }

    public void TransformDead()
    {
        spriter.sprite = deadSprite;
    }

    // Body Spawn
    public IEnumerator RiseRoutine()
    {
        // Rise on Spawn
        yield return StartCoroutine(LerpToDestination(transform, new Vector2(transform.position.x, riseTargetY), riseSpeed));
    }

    public IEnumerator FallRoutine()
    {
        // Fall back to default position on Spawn
        yield return StartCoroutine(LerpToDestination(transform, new Vector2(transform.position.x, fallTargetY), fallSpeed));
    }

    public IEnumerator IdleRoutine()
    {
        // Activate platforms
        platforms.SetPlatforms(true);

        // Go to Default Position
        yield return StartCoroutine(LerpToDestination(transform, idleTargetPos, riseSpeed));

        // Up Down Movement
        yield return StartCoroutine(LerpToDestination(transform, new Vector2(transform.position.x, transform.position.y + idleOffset), idleSpeed));
        yield return StartCoroutine(LerpToDestination(transform, new Vector2(transform.position.x, transform.position.y - idleOffset), idleSpeed));
    }

    // Body Sweep
    public IEnumerator PrepareSweepRoutine(Side dir)
    {
        // Deactivate platforms
        platforms.SetPlatforms(false);

        // Leaning motion
        if (dir == Side.Left)
        {
            yield return StartCoroutine(LerpToDestination(transform, new Vector2(transform.position.x - leanOffset, transform.position.y), leanSpeed));
        }
        else
        {
            yield return StartCoroutine(LerpToDestination(transform, new Vector2(transform.position.x + leanOffset, transform.position.y), leanSpeed));
        }
    }
    public IEnumerator SweepRoutine(Side dir)
    {
        // Motion during sweep
        if (dir == Side.Left)
        {
            Vector2 targetPos = transform.position;
            targetPos.x += leanOffset * 2;
            Quaternion targetRot = Quaternion.Euler(0, 0, -tiltAngle);
            yield return StartCoroutine(TiltToDestination(transform, targetPos, targetRot, leanSpeed));
        }
        else
        {
            Vector2 targetPos = transform.position;
            targetPos.x -= leanOffset * 2;
            Quaternion targetRot = Quaternion.Euler(0, 0, tiltAngle);
            yield return StartCoroutine(TiltToDestination(transform, targetPos, targetRot, leanSpeed));
        }
    }
    public IEnumerator MoveToIdleRoutine()
    {
        Vector2 targetPos = idleTargetPos;
        Quaternion targetRot = Quaternion.identity;
        yield return StartCoroutine(TiltToDestination(transform, targetPos, targetRot, leanSpeed));

        // Activate platforms
        platforms.SetPlatforms(true);
    }

    // Body Slam
    public IEnumerator SlamRiseRoutine()
    {
        // Deactivate platforms
        platforms.SetPlatforms(false);

        bodyShakeRoutine = StartCoroutine(BodyShakeRoutine());
        yield return StartCoroutine(LerpToDestination(transform, new Vector2(transform.position.x, slamRiseTargetY), slamRiseSpeed));
        StopCoroutine(bodyShakeRoutine);
    }
    public IEnumerator SlamDipRoutine()
    {
        yield return StartCoroutine(LerpToDestination(transform, idleTargetPos, slamDipSpeed));
    }
    Coroutine bodyShakeRoutine;
    public IEnumerator BodyShakeRoutine()
    {
        float t = 0;
        while (true)
        {
            float offsetX = Mathf.Sin(Time.time * shakeSpeed) * shakeMagnitude;
            float offsetY = Mathf.Cos(Time.time * shakeSpeed) * shakeMagnitude;
            transform.position = new Vector2(transform.position.x + offsetX, transform.position.y + offsetY);

            t += Time.deltaTime;
            yield return null;
        }
    }

    // Body Phase Transition
    public void TransitionFreezeRoutine()
    {
        transform.position = idleTargetPos;
    }

    // Body Dead Transition
    public IEnumerator FallDown()
    {
        Vector2 targetPos = new Vector2(-1, -1);
        Quaternion targetRot = Quaternion.Euler(0, 0, 35);
        yield return StartCoroutine(TiltToDestination(transform, targetPos, targetRot, faintSpeed));
    }
}
