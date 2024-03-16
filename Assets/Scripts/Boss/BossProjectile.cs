using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{
    [SerializeField] int projectileDamage;
    [SerializeField] float projectileForce;
    [SerializeField] float hitboxRadius;
    Rigidbody2D rb2d;
    Animator animator;

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        Vector2 direction = (new Vector3(Random.Range(-3, 4), -3, 0) - transform.position).normalized;
        rb2d.AddForce(direction * projectileForce, ForceMode2D.Impulse);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, hitboxRadius);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(Explosion());
    }

    private IEnumerator Explosion()
    {
        animator.Play("BossProjectileExplosion");
        DealDamage();
        while(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1)
        {
            yield return null;
        }
        Destroy(this);
    }

    private void DealDamage()
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, hitboxRadius, Manager.Game.Player.Mask);
        if (collider != null)
        {
            IDamageable[] damageables = collider.GetComponents<IDamageable>();
            foreach(var damageable in damageables)
            {
                damageable.TakeDamage(projectileDamage);
            }
        }
    }
}
