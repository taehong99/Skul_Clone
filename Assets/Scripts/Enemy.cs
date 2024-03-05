using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public int health;
    [SerializeField] float knockbackForce;

    Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if(health <= 0)
        {
            Die();
        }
        KnockBack();
        animator.SetTrigger($"hit{Random.Range(0, 2)}");
    }

    public void KnockBack()
    {
        Vector2 knockDirection = Manager.Game.Player.facingDir == PlayerController.FacingDir.Left ? Vector2.left : Vector2.right;
        transform.Translate(Vector2.Lerp(Vector2.zero, knockDirection, knockbackForce));
    }

    public void Die() {
        animator.SetTrigger("dead");
    }
}
