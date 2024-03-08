using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Soldier : Enemy
{
    Coroutine attackRoutine;
    Collider2D hitbox;

    protected override void Start()
    {
        base.Start();
        hitbox = GetComponentsInChildren<BoxCollider2D>()[1];
    }

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

    public void AttackFrame()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, attackRange, playerMask);
        IDamageable[] damageables = collider.GetComponents<IDamageable>();
        foreach (IDamageable damageable in damageables)
        {
            // attack right
            if(transform.localScale.x == 1)
            {
                if(collider.gameObject.transform.position.x >= transform.position.x)
                {
                    damageable.TakeDamage(damage);
                }
            }
            else//attack left
            {
                if (collider.gameObject.transform.position.x <= transform.position.x)
                {
                    damageable.TakeDamage(damage);
                }
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
