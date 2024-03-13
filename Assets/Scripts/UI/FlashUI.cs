using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlashUI : BaseUI
{
    [SerializeField] float flashSpeed;
    [SerializeField] float flashBuffer;
    Image image;

    private void Start()
    {
        image = GetUI<Image>("WhiteFlash");
        Manager.Events.voidEventDic["whiteFlash"].OnEventRaised += Flash;
    }

    public void Flash()
    {
        StartCoroutine(FlashRoutine());
    }

    private IEnumerator FlashRoutine()
    {
        float t = 0;
        while (t <= 1)
        {
            SetAlpha(t);
            t += Time.deltaTime * flashSpeed;
            yield return null;
        }
        yield return new WaitForSeconds(flashBuffer);
        while (t >= 0)
        {
            SetAlpha(t);
            t -= Time.deltaTime * flashSpeed;
            yield return null;
        }
        SetAlpha(0);
    }

    private void SetAlpha(float alpha)
    {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }
}
