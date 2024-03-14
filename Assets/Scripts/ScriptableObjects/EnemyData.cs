using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Data/Enemy", fileName = "EnemyData")]
public class EnemyData : ScriptableObject
{
    [Header("Stats")]
    public int baseHP;
    public float moveSpeed;
    public int damage;
    public float knockbackForce;

    [Header("Patrol")]
    public float patienceTimer;
    public float patrolTime;

    [Header("Detect Player")]
    public float playerCheckRange;
    public float attackRange;
}
