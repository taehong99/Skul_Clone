using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour, IDamageable
{
    public int health;

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
        animator.SetTrigger($"hit{Random.Range(0, 2)}");
    }

    public void Die() {
        animator.SetTrigger("dead");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
    }
}
