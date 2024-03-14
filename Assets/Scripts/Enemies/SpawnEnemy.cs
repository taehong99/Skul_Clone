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

    // spawns when parent spawner gets activated
    private void OnEnable()
    {
        GetComponentInParent<EnemySpawner>().OnSpawnEnemies += Spawn;
    }

    public void Spawn()
    {
        StartCoroutine(SpawnRoutine());
        Manager.Events.voidEventDic["enemySpawned"].RaiseEvent();
    }

    private IEnumerator SpawnRoutine()
    {
        GameObject effect = Manager.Pool.GetPool(spawnEffectPrefab, transform.position, Quaternion.identity).gameObject;
        effect.GetComponent<Animator>().Play("EnemyAppear");
        while (effect.GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime <= 0.8f)
        {
            yield return null;
        }
        Instantiate(enemyPrefab, transform.position, transform.rotation);
    }
}
