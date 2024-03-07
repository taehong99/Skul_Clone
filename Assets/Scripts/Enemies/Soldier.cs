using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Enemy
{
    Coroutine attackRoutine;

    protected override void Attack()
    {
        attackRoutine = StartCoroutine(SoldierAttackRoutine());
    }

    protected override void StopAttack()
    {
        StopCoroutine(attackRoutine);
    }

    private IEnumerator SoldierAttackRoutine()
    {
        animator.Play("Stance");
        yield return new WaitForSeconds(stanceDuration);
        animator.Play("Attack");
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1);
        isAttacking = false;
    }
}
