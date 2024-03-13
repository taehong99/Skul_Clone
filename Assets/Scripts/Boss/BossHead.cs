using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BossHead : BossBodyPart
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
    [SerializeField] Vector2 idleJawPos;
    [SerializeField] float idleSpeed;
    [SerializeField] float idleOffset;
    [SerializeField] float jawMovementOffset;
    [SerializeField] float jawMovementSpeed;

    [Header("Sweep Attack Values")]
    [SerializeField] float leanOffset;
    [SerializeField] float leanSpeed;

    [Header("Freeze Values")]
    [SerializeField] Vector2 faceFreezeLocalPos;

    [Header("Special Attack Values")]
    [SerializeField] Vector2 duckTargetPos;
    [SerializeField] Vector2 duckJawPos;
    [SerializeField] float duckSpeed;
    [SerializeField] Vector2 riseTargetPos;
    [SerializeField] float riseSpeed;
    

    [Header("Misc")]
    private BossFace face;
    private BossJaw jaw;
    private BossGem gem;
    private BoxCollider2D hurtbox;

    private void Awake()
    {
        face = GetComponentInChildren<BossFace>();
        jaw = GetComponentInChildren<BossJaw>();
        gem = GetComponentInChildren<BossGem>();
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

    #region Spawn
    public IEnumerator SpawnRoutine()
    {
        // head moves a little bit down
        yield return StartCoroutine(LerpWithOffset(transform, new Vector2(0, -nodOffset), nodSpeed));

        // scream
        yield return StartCoroutine(ScreamRoutine());
    }

    public IEnumerator ScreamRoutine()
    {
        Manager.Game.Shaker.ToggleShake();

        // head shakes while boss screams
        headShakeRoutine = StartCoroutine(HeadShakeRoutine());
        yield return (StartCoroutine(ScreamJawOpenRoutine()));
        StopCoroutine(headShakeRoutine);

        Manager.Game.Shaker.ToggleShake();
    }

    Coroutine headShakeRoutine;
    private IEnumerator HeadShakeRoutine()
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

    private IEnumerator ScreamJawOpenRoutine()
    {
        yield return StartCoroutine(JawOpen());

        yield return new WaitForSeconds(screamDuration);

        yield return StartCoroutine(JawClose());
    }
    #endregion

    #region Idle
    public IEnumerator MoveToIdleRoutine()
    {
        Vector2 offset = idleTargetPos - (Vector2)transform.localPosition;
        Vector2 faceOffset = Vector2.zero - (Vector2)face.transform.localPosition;
        Vector2 jawOffset = idleJawPos - (Vector2)jaw.transform.localPosition;
        StartCoroutine(LerpWithOffset(face.transform, faceOffset, nodSpeed));
        StartCoroutine(LerpWithOffset(jaw.transform, jawOffset, nodSpeed));
        yield return StartCoroutine(LerpWithOffset(transform, offset, nodSpeed));
    }

    public IEnumerator IdleRoutine()
    {
        // Move to Default Position
        yield return StartCoroutine(MoveToIdleRoutine());
        //float yOffset = idleTargetPos.y - transform.localPosition.y;
        //yield return StartCoroutine(LerpToDestination(transform, new Vector2(0, yOffset), nodSpeed));
        
        // Up Down Movement
        yield return StartCoroutine(LerpWithOffset(transform, new Vector2(0, idleOffset), idleSpeed));
        yield return StartCoroutine(LerpWithOffset(transform, new Vector2(0, -idleOffset), idleSpeed));
    }

    public void StartJawMovement()
    {
        jawMovementRoutine = StartCoroutine(JawMovementRoutine());
    }

    public void StopJawMovement()
    {
        StopCoroutine(jawMovementRoutine);
    }

    Coroutine jawMovementRoutine;
    public IEnumerator JawMovementRoutine()
    {
        // jaw up and down motion
        while (true)
        {
            yield return StartCoroutine(LerpWithOffset(jaw.transform, new Vector2(0, -jawMovementOffset), jawMovementSpeed));
            yield return StartCoroutine(LerpWithOffset(jaw.transform, new Vector2(0, jawMovementOffset), jawMovementSpeed));
        }
    }
    #endregion

    #region Sweep
    public IEnumerator PrepareSweepRoutine(Side dir)
    {
        // Leaning motion
        if (dir == Side.Left)
        {
            yield return StartCoroutine(LerpWithOffset(transform, new Vector2(-leanOffset, 0), leanSpeed));
        }
        else
        {
            yield return StartCoroutine(LerpWithOffset(transform, new Vector2(leanOffset, 0), leanSpeed));
        }

        Side attackDir = Manager.Game.Player.transform.position.x <= 0 ? Side.Left : Side.Right;
    }
    #endregion

    #region Transition
    public IEnumerator TransitionFreezeRoutine()
    {
        face.transform.localPosition = faceFreezeLocalPos;
        yield return null;
    }
    #endregion

    #region Special Attack

    public IEnumerator Duck()
    {
        StartCoroutine(LerpToDestination(transform, duckTargetPos, duckSpeed));
        StartCoroutine(LerpToDestination(jaw.transform, duckJawPos, duckSpeed));
        headShakeRoutine = StartCoroutine(HeadShakeRoutine());
        yield return null;
    }

    public IEnumerator Charge()
    {
        yield return StartCoroutine(gem.Charge());
    }

    public IEnumerator LiftHead()
    {
        StartCoroutine(LerpToDestination(transform, riseTargetPos, riseSpeed));
        StartCoroutine(JawOpen());
        gem.StartAttack();
        yield return null;
    }

    public void EndSpecialAttack()
    {
        StopCoroutine(headShakeRoutine);
        gem.EndAttack();
    }

    #endregion

    private IEnumerator JawOpen()
    {
        yield return StartCoroutine(LerpWithOffset(jaw.transform, new Vector2(0, -jawOpenGap), jawOpenSpeed));
    }

    private IEnumerator JawClose()
    {
        yield return StartCoroutine(LerpWithOffset(jaw.transform, new Vector2(0, jawOpenGap), jawOpenSpeed));
    }
}
