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
        bounceRoutine = StartCoroutine(Bounce());
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
