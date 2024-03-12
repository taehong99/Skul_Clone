using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemy : MonoBehaviour
{
    enum EnemyType { Soldier, Knight, Wizard }
    [SerializeField] EnemyType enemyToSpawn;
    private GameObject enemyPrefab;
    private PooledObject spawnEffectPrefab;

    private void Start()
    {
        spawnEffectPrefab = Manager.Resource.Load<PooledObject>("Prefabs/EnemyEffect");
        switch (enemyToSpawn)
        {
            case EnemyType.Soldier:
                enemyPrefab = Manager.Resource.Load<GameObject>("Prefabs/Soldier");
                break;
            case EnemyType.Knight:
                enemyPrefab = Manager.Resource.Load<GameObject>("Prefabs/Knight");
                break;
            case EnemyType.Wizard:
                enemyPrefab = Manager.Resource.Load<GameObject>("Prefabs/Wizard");
                break;
            default:
                break;
        }
    }

    public void Spawn()
    {
        GameObject go = Manager.Pool.GetPool(spawnEffectPrefab, transform.position, Quaternion.identity).gameObject;
        go.GetComponent<Animator>().Play("EnemyAppear");
        Instantiate(enemyPrefab, transform.position, transform.rotation);
    }

    // spawns when parent spawner gets activated
    private void OnEnable()
    {
        GetComponentInParent<EnemySpawner>().OnSpawnEnemies += Spawn;
    }

    private void OnDisable()
    {
        GetComponentInParent<EnemySpawner>().OnSpawnEnemies -= Spawn;
    }
}