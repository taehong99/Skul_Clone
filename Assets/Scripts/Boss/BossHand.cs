using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHand : MonoBehaviour
{
    public enum Type { Left, Right };

    [Header("Event Channels")]
    [SerializeField] private VoidEventChannelSO handSpawned;

    [Header("Rise Values")]
    [SerializeField] float targetY;
    [SerializeField] float riseSpeed;

    [Header("Shake Values")]
    [SerializeField] float shakeAmount;
    [SerializeField] float shakeSpeed;
    [SerializeField] float shakeMagnitude;
    [SerializeField] float shakeInterval;

    [Header("Misc")]
    [SerializeField] Type type;
    Animator animator;
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
        //shakeRoutine = StartCoroutine(ShakeRoutine());
        //riseRoutine = StartCoroutine(RiseRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void Spawn()
    {
        riseRoutine = StartCoroutine(RiseRoutine());
    }
    
    public void Slide()
    {
        StartCoroutine(SlideRoutine());
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
}
