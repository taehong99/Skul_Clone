using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BossHead : MonoBehaviour
{
    [Header("Spawn Values")]
    [SerializeField] float nodSpeed;
    [SerializeField] float nodOffset;
    [SerializeField] float jawOpenGap;
    [SerializeField] float jawOpenSpeed;
    [SerializeField] float screamDuration;
    [SerializeField] float shakeSpeed;
    [SerializeField] float shakeMagnitude;

    [Header("Idle Values")]
    [SerializeField] Vector2 idleTargetPos;
    [SerializeField] float idleSpeed;
    [SerializeField] float idleOffset;
    [SerializeField] float jawMovementOffset;
    [SerializeField] float jawMovementSpeed;

    [Header("Sweep Attack Values")]
    [SerializeField] float leanOffset;
    [SerializeField] float leanSpeed;

    [Header("Misc")]
    private BossFace face;
    private BossJaw jaw;
    private BoxCollider2D hurtbox;

    private void Awake()
    {
        face = GetComponentInChildren<BossFace>();
        jaw = GetComponentInChildren<BossJaw>();
        hurtbox = GetComponent<BoxCollider2D>();
    }

    public void SetHurtBox(bool b)
    {
        if(b == true)
        {
            hurtbox.enabled = true;
        }
        else
        {
            hurtbox.enabled = false;
        }
    }

    public IEnumerator SpawnRoutine()
    {
        // head moves a little bit down
        yield return StartCoroutine(LerpToDestination(transform, new Vector2(0, -nodOffset), nodSpeed));

        // scream
        yield return StartCoroutine(ScreamRoutine());
    }

    public IEnumerator PrepareSweepRoutine(Side dir)
    {
        // Leaning motion
        if (dir == Side.Left)
        {
            yield return StartCoroutine(LerpToDestination(transform, new Vector2(-leanOffset, 0), leanSpeed));
        }
        else
        {
            yield return StartCoroutine(LerpToDestination(transform, new Vector2(leanOffset, 0), leanSpeed));
        }

        Side attackDir = Manager.Game.Player.transform.position.x <= 0 ? Side.Left : Side.Right;
        Manager.Events.dirEventDic["sweepReady"].RaiseEvent(attackDir);
    }

    public IEnumerator ScreamRoutine()
    {
        // head shakes while boss screams
        headShakeRoutine = StartCoroutine(HeadShakeRoutine());
        yield return(StartCoroutine(ScreamJawOpenRoutine()));
        StopCoroutine(headShakeRoutine);
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
    private IEnumerator ScreamJawOpenRoutine()
    {
        yield return StartCoroutine(LerpToDestination(jaw.transform, new Vector2(0, -jawOpenGap), jawOpenSpeed));

        yield return new WaitForSeconds(screamDuration);

        yield return StartCoroutine(LerpToDestination(jaw.transform, new Vector2(0, jawOpenGap), jawOpenSpeed));
    }

    public IEnumerator MoveToIdleRoutine()
    {
        Vector2 offset = idleTargetPos - (Vector2)transform.localPosition;
        yield return StartCoroutine(LerpToDestination(transform, offset, nodSpeed));
    }

    public IEnumerator IdleRoutine()
    {
        // Move to Default Position
        yield return StartCoroutine(MoveToIdleRoutine());
        //float yOffset = idleTargetPos.y - transform.localPosition.y;
        //yield return StartCoroutine(LerpToDestination(transform, new Vector2(0, yOffset), nodSpeed));

        // Up Down Movement
        yield return StartCoroutine(LerpToDestination(transform, new Vector2(0, idleOffset), idleSpeed));
        yield return StartCoroutine(LerpToDestination(transform, new Vector2(0, -idleOffset), idleSpeed));
    }

    Coroutine jawMovementRoutine;
    public IEnumerator JawMovementRoutine()
    {
        // jaw up and down motion
        while (true)
        {
            yield return StartCoroutine(LerpToDestination(jaw.transform, new Vector2(0, -jawMovementOffset), jawMovementSpeed));
            yield return StartCoroutine(LerpToDestination(jaw.transform, new Vector2(0, jawMovementOffset), jawMovementSpeed));
        }
    }

    private IEnumerator LerpToDestination(Transform transform, Vector2 offset, float speed)
    {
        float t = 0;
        Vector2 startPos = transform.localPosition;
        Vector2 endPos = startPos;
        endPos += offset;
        while (t < 1)
        {
            transform.localPosition = Vector2.Lerp(startPos, endPos, t);
            t += Time.deltaTime * speed;
            yield return null;
        }
    }
}
