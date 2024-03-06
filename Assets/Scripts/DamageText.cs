using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DamageText : MonoBehaviour
{
    TMP_Text tmpText;
    [SerializeField] float riseAmt;
    [SerializeField] float fallAmt;
    [SerializeField] float speed;
    Vector3 initialPos;

    private void Awake()
    {
        tmpText = GetComponent<TMP_Text>();
        initialPos = transform.position;
    }

    private void Start()
    {
        StartCoroutine(Bounce());
    }

    public void SetText(int input)
    {
        tmpText.text = input.ToString();
    }

    IEnumerator Bounce()
    {
        float timer = 0f;

        Vector3 targetPeakPos = initialPos + new Vector3(0, riseAmt, 0);
        // rise
        while (timer < speed)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / speed);
            transform.position = Vector3.Lerp(initialPos, targetPeakPos, t);
            yield return null;
        }
        // fall
        timer = 0f;
        Vector3 targetEndPos = targetPeakPos + new Vector3(0, -fallAmt, 0);
        while (timer < speed)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / speed);
            transform.position = Vector3.Lerp(targetPeakPos, targetEndPos, t);
            yield return null;
        }

        Destroy(gameObject);
    }
}
