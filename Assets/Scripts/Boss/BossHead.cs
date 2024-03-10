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
    [SerializeField] float targetIdleY;
    [SerializeField] float idleSpeed;
    [SerializeField] float idleOffset;
    [SerializeField] float jawMovementOffset;
    [SerializeField] float jawMovementSpeed;

    [Header("Misc")]
    [SerializeField] private VoidEventChannelSO screamFinished;
    [SerializeField] private VoidEventChannelSO idleFinished;
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
    public void Idle()
    {
        StartCoroutine(IdleRoutine());
        jawMovementRoutine = StartCoroutine(JawMovementRoutine());
    }

    private IEnumerator ScreamRoutine()
    {
        // head moves a little bit down
        yield return StartCoroutine(LerpToDestination(transform, new Vector2(0, -nodOffset), nodSpeed));
        
        // head shakes while boss screams
        headShakeRoutine = StartCoroutine(HeadShakeRoutine());
        yield return(StartCoroutine(ScreamJawOpenRoutine()));
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

    private IEnumerator ScreamJawOpenRoutine()
    {
        yield return StartCoroutine(LerpToDestination(jaw.transform, new Vector2(0, -jawOpenGap), jawOpenSpeed));

        yield return new WaitForSeconds(screamDuration);

        yield return StartCoroutine(LerpToDestination(jaw.transform, new Vector2(0, jawOpenGap), jawOpenSpeed));
    }

    private IEnumerator IdleRoutine()
    {
        // Move to Default Position
        float yOffset = targetIdleY - transform.localPosition.y;
        yield return StartCoroutine(LerpToDestination(transform, new Vector2(0, yOffset), nodSpeed));

        // Up Down Movement
        yield return StartCoroutine(LerpToDestination(transform, new Vector2(0, idleOffset), idleSpeed));
        yield return StartCoroutine(LerpToDestination(transform, new Vector2(0, -idleOffset), idleSpeed));

        Debug.Log("reached");
        idleFinished.RaiseEvent();
    }

    Coroutine jawMovementRoutine;
    private IEnumerator JawMovementRoutine()
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
