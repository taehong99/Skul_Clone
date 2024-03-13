using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossGem : MonoBehaviour
{
    Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    public IEnumerator Charge()
    {
        animator.Play("GemCharging");
        yield return new WaitForSeconds(4.333f);
    }

    public void StartAttack()
    {
        animator.Play("GemShooting");
    }

    public void EndAttack()
    {
        animator.Play("Null");
    }
}
