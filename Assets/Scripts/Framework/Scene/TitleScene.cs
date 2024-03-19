using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class TitleScene : BaseScene
{
    [SerializeField] Image textImage;
    private float minAlpha = 0.1f; 
    private float maxAlpha = 1f; 
    private float pulseSpeed = 1f;
    Coroutine pulseRoutine;
    bool loadStarted;

    private void OnEnable()
    {
        pulseRoutine = StartCoroutine(TextPulseRoutine());
        Manager.UI.GetComponent<PlayerInput>().enabled = false;
    }

    private void OnDisable()
    {
        StopCoroutine(pulseRoutine);
        Manager.UI.GetComponent<PlayerInput>().enabled = true;
    }

    private void Update()
    {
        if (loadStarted)
            return;

        if (Input.anyKeyDown)
        {
            Manager.Scene.LoadScene("1.CastleScene");
            loadStarted = true;
        }
    }

    private IEnumerator TextPulseRoutine()
    {
        while (true)
        {
            float t = Mathf.PingPong(Time.time * pulseSpeed, 1f);
            float alpha = Mathf.Lerp(minAlpha, maxAlpha, t);

            Color newColor = textImage.color;
            newColor.a = alpha;
            textImage.color = newColor;

            yield return null;
        }
    }

    public override IEnumerator LoadingRoutine()
    {
        yield return null;
    }
}
