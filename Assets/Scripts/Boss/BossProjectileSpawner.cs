using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectileSpawner : MonoBehaviour
{
    [SerializeField] GameObject spawningEffect;
    [SerializeField] GameObject projectile;
    [SerializeField] float yPosition;
    [SerializeField] float minXPosition;
    [SerializeField] float maxXPosition;
    Coroutine spawnRoutine;


    public void StartSpawning()
    {
        spawnRoutine = StartCoroutine(SpawnRoutine());
    }

    public void StopSpawning()
    {
        StopCoroutine(spawnRoutine);
    }

    public IEnumerator SpawnRoutine()
    {
        while (true)
        {
            // instantiate spawning effect
            float offset = Random.Range(minXPosition, maxXPosition + 1);
            float xPosition = Manager.Game.Player.transform.position.x + offset;
            yPosition += Random.Range(-1f, 1f);
            Vector2 spawnPos = new Vector2(xPosition, yPosition);
            Instantiate(spawningEffect, spawnPos, Quaternion.identity);

            yield return new WaitForSeconds(0.5f);

            // instantiate projectile
            Instantiate(projectile, spawnPos, Quaternion.identity);
        }
    }
}
