using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class GameOverUI : PopUpUI
{
    private float minAlpha = 0.1f;
    private float maxAlpha = 1f;
    private float pulseSpeed = 1f;
    Coroutine pulseRoutine;
    private bool loadStarted;

    private void Update()
    {
        if (Input.anyKeyDown)
        {
            if (loadStarted)
                return;

            Manager.Scene.LoadScene("0.TitleScene");
            Manager.UI.Return();
            loadStarted = true;
        }
    }

    private void OnEnable()
    {
        pulseRoutine = StartCoroutine(TextPulseRoutine());
    }

    private void OnDisable()
    {
        StopCoroutine(pulseRoutine);
    }

    private IEnumerator TextPulseRoutine()
    {
        while (true)
        {
            float t = Mathf.PingPong(Time.time * pulseSpeed, 1f);
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);

            Color newColor = GetUI<Image>("ContinueText").color;
            newColor.a = alpha;
            GetUI<Image>("ContinueText").color = newColor;

            yield return null;
        }
    }
}
