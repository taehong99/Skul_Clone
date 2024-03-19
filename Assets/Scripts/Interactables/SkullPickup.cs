using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkullPickup : MonoBehaviour, IInteractable
{
    [SerializeField] PlayerData data;
    [SerializeField] float amp;
    [SerializeField] float freq;
    private Coroutine bounceRoutine;
    private Transform sprite;
    private Canvas canvas;

    private void OnEnable()
    {
        sprite = transform.GetChild(0);
        canvas = GetComponentInChildren<Canvas>(true);
        bounceRoutine = StartCoroutine(SpawnRoutine());
    }

    private void OnDisable()
    {
        StopCoroutine(bounceRoutine);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        canvas.gameObject.SetActive(true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        canvas.gameObject.SetActive(false);
    }

    private IEnumerator SpawnRoutine()
    {
        float time = 0;
        float duration = 0.2f;
        float height = 2f;

        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + Vector3.up * height;
        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;

        startPos = transform.position;
        endPos = startPos - Vector3.up * height;
        time = 0f;
        while (time < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        transform.position = endPos;

        StartCoroutine(Bounce());
    }

    private IEnumerator Bounce()
    {
        Vector2 startPos = sprite.position;
        while (true)
        {
            sprite.position = new Vector3(startPos.x, startPos.y + Mathf.Sin(Time.time * freq) * amp, 0);
            yield return null;
        }
    }

    public void Interact()
    {
        Manager.Events.dataEventDic["skullPickedUp"].RaiseEvent(data);
        Destroy(gameObject);
    }
}
