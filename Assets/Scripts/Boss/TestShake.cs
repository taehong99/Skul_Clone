using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestShake : MonoBehaviour
{
    [Header("Rise Values")]
    [SerializeField] float targetY;
    [SerializeField] float riseSpeed;

    [Header("Shake Values")]
    [SerializeField] float shakeAmount;
    [SerializeField] float shakeSpeed;
    [SerializeField] float shakeMagnitude;
    [SerializeField] float shakeInterval;

    [SerializeField] float spawnDelay;

    Coroutine shakeRoutine;
    Coroutine riseRoutine;

    // Start is called before the first frame update
    private void OnEnable()
    {
        //shakeRoutine = StartCoroutine(ShakeRoutine());
        riseRoutine = StartCoroutine(RiseRoutine());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    private IEnumerator RiseRoutine()
    {
        yield return new WaitForSeconds(spawnDelay);

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
    }

}
