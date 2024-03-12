using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Enemy
{
    [Header("Knight-Specific Variables")]
    [SerializeField] float tackleTime;
    [SerializeField] float tackleSpeed;

    private int attackIndex = 0;
    Coroutine attackRoutine;

    protected override void Attack()
    {
        if(attackIndex == 0)
        {
            attackRoutine = StartCoroutine(AttackARoutine());
        }
        else{
            attackRoutine = StartCoroutine(AttackBRoutine());
        }

        attackIndex ^= 1;
    }

    protected override void StopAttack()
    {
        StopCoroutine(attackRoutine);
    }

    private IEnumerator AttackARoutine() // Hammer Attack
    {
        animator.Play("StanceA");
        yield return new WaitForSeconds(stanceDuration);
        animator.Play("AttackA");
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }
        yield return new WaitForSeconds(1);
        isAttacking = false;
    }

    private IEnumerator AttackBRoutine() // Tackle Attack
    {
        animator.Play("StanceB");
        Vector3 targetDir = player.transform.position.x < transform.position.x ? Vector3.left : Vector3.right;
        yield return new WaitForSeconds(stanceDuration);

        animator.Play("AttackB");
        rb2d.velocity = targetDir * tackleSpeed;
        float timer = 0;
        while (timer < tackleTime)
        {
            timer += Time.deltaTime;
            yield return null;
        }
        Debug.Log("Reached");
        rb2d.velocity = Vector2.zero;
        isAttacking = false;
    }

    public void AttackFrame()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, attackRange, playerMask);
        if (collider == null)
            return;
        IDamageable[] damageables = collider.GetComponents<IDamageable>();
        foreach (IDamageable damageable in damageables)
        {
            // attack right
            if (transform.localScale.x == 1)
            {
                if (collider.gameObject.transform.position.x >= transform.position.x)
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
}
