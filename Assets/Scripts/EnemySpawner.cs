using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemySpawner : MonoBehaviour
{
    public UnityAction OnSpawnEnemies;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnSpawnEnemies?.Invoke();
        Destroy(gameObject);
    }
}
