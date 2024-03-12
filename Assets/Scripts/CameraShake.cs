using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraShake : MonoBehaviour
{
    CinemachineBasicMultiChannelPerlin vcam;
    [SerializeField] float shakeAmplitude;

    private void Start()
    {
        vcam = GetComponent<CinemachineVirtualCamera>().GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    public void Shake(float duration)
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine(duration));
    }

    public void ToggleShake()
    {
        float curAmp = vcam.m_AmplitudeGain;
        if(curAmp == 0)
        {
            vcam.m_AmplitudeGain = shakeAmplitude;
        }
        else
        {
            vcam.m_AmplitudeGain = 0;
        }
    }

    private IEnumerator ShakeRoutine(float duration)
    {
        vcam.m_AmplitudeGain = shakeAmplitude;
        yield return new WaitForSeconds(duration);
        vcam.m_AmplitudeGain = 0;
    }
}
