using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class SmokeSpawner : MonoBehaviour
{
    public enum SmokeType { Dash, Jump }
    [SerializeField] Transform dashSmokeSpawnPoint;
    [SerializeField] Transform jumpSmokeSpawnPoint;
    private PooledObject dashEffectPrefab;
    private PooledObject jumpEffectPrefab;

    private void Awake()
    {
        dashEffectPrefab = Manager.Resource.Load<PooledObject>("Prefabs/DashSmoke1");
        jumpEffectPrefab = Manager.Resource.Load<PooledObject>("Prefabs/JumpSmoke");
    }

    public void SpawnSmoke(SmokeType type)
    {
        switch (type)
        {
            case SmokeType.Jump:
                Manager.Pool.GetPool(jumpEffectPrefab, jumpSmokeSpawnPoint.position, Quaternion.identity);
                break;
            case SmokeType.Dash:
                Quaternion rotation = Manager.Game.Player.transform.localScale.x == 1 ? Quaternion.identity : Quaternion.Euler(new Vector3(0, 180, 0));
                Manager.Pool.GetPool(dashEffectPrefab, dashSmokeSpawnPoint.position, rotation);
                break;
        }
    }
}
