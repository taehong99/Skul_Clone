using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    public UnityAction OnSpawnEnemies;
    private bool spawned;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (spawned)
            return;
        if (collision.CompareTag("Player"))
        {
            spawned = true;
            StartCoroutine(SpawnRoutine());
        }
    }

    private IEnumerator SpawnRoutine()
    {
        OnSpawnEnemies?.Invoke();
        yield return new WaitForSeconds(1f);
        Destroy(gameObject);
    }
}
