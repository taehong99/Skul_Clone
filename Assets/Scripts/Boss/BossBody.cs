using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossBody : MonoBehaviour
{
    [Header("Rise Values")]
    [SerializeField] float riseTargetY;
    [SerializeField] float riseSpeed;

    [Header("Fall Values")]
    [SerializeField] float fallTargetY;
    [SerializeField] float fallSpeed;

    [Header("Idle Values")]
    [SerializeField] float idleTargetY;
    [SerializeField] float idleSpeed;
    [SerializeField] float idleOffset;

    [Header("Misc")]
    [SerializeField] private VoidEventChannelSO bodySpawned;
    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void Rise()
    {
        StartCoroutine(RiseRoutine());
    }

    public void Fall()
    {
        StartCoroutine(FallRoutine());   
    }

    public void Idle()
    {
        StartCoroutine(IdleRoutine());
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

    private IEnumerator IdleRoutine()
    {
        // Go to Default Position
        float t = 0;
        Vector2 startPos = transform.position;
        Vector2 endPos = startPos;
        endPos.y = idleTargetY;
        while (t < 1)
        {
            transform.position = Vector2.Lerp(startPos, endPos, t);
            t += Time.deltaTime * riseSpeed;
            yield return null;
        }
        
        // Up Down Movement
        t = 0;
        startPos = transform.position;
        endPos = startPos;
        endPos.y = endPos.y + idleOffset;
        while (t < 1)
        {
            transform.position = Vector2.Lerp(startPos, endPos, t);
            t += Time.deltaTime * riseSpeed;
            yield return null;
        }
        t = 0;
        while (t < 1)
        {
            transform.position = Vector2.Lerp(endPos, startPos, t);
            t += Time.deltaTime * riseSpeed;
            yield return null;
        }
    }
}
