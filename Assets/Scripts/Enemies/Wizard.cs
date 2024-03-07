using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : Enemy
{
    [Header("Wizard-Specific Variables")]
    [SerializeField] GameObject fireballPrefab;
    [SerializeField] Transform fireballSpawnPoint;
    [SerializeField] float fireInterval;
    
    Coroutine attackRoutine;

    protected override void Attack()
    {
        attackRoutine = StartCoroutine(AttackRoutine());
    }

    protected override void StopAttack()
    {
        StopCoroutine(attackRoutine);
    }

    private IEnumerator AttackRoutine()
    {
        animator.Play("Attack");
        for(int i = 0; i < 3; i++)
        {
            Instantiate(fireballPrefab, fireballSpawnPoint);
            yield return new WaitForSeconds(fireInterval);
        }
        isAttacking = false;
    }
}
