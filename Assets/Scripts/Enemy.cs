using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Enemy : MonoBehaviour, IDamageable
{
    [SerializeField] int baseHP;
    private int hp;
    public int HP { get { return hp; } private set { hp = value; OnHPChanged?.Invoke(value); } }
    public event UnityAction<int> OnHPChanged;

    [SerializeField] float knockbackForce;

    Animator animator;

    private void Awake()
    {
        HP = baseHP;
        animator = GetComponent<Animator>();
    }

    public void TakeDamage(int damage)
    {
        HP -= damage;
        if(HP <= 0)
        {
            StartCoroutine(Die());
        }
        KnockBack();
        animator.SetTrigger($"hit{Random.Range(0, 2)}");
    }

    public void KnockBack()
    {
        Vector2 knockDirection = Manager.Game.Player.facingDir == PlayerController.FacingDir.Left ? Vector2.left : Vector2.right;
        transform.Translate(Vector2.Lerp(Vector2.zero, knockDirection, knockbackForce));
    }

    IEnumerator Die() {
        animator.SetTrigger("dead");
        while(animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }
        Destroy(gameObject);
    }
}
