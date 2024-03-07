using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeSpawner : MonoBehaviour
{
    public enum SmokeType { Dash, Jump }
    [SerializeField] Animator dashSmokeAnimator;
    [SerializeField] Animator jumpSmokeAnimator;
    [SerializeField] Transform dashSmokeSpawnPoint;
    [SerializeField] Transform jumpSmokeSpawnPoint;

    public void SpawnSmoke(SmokeType type)
    {
        switch (type)
        {
            case SmokeType.Jump:
                jumpSmokeAnimator.transform.position = jumpSmokeSpawnPoint.position;
                jumpSmokeAnimator.Play("DoubleJumpEffect");
                break;
            case SmokeType.Dash:
                dashSmokeAnimator.transform.position = dashSmokeSpawnPoint.position;
                dashSmokeAnimator.Play("DashEffect");
                break;
        }
    }
}
