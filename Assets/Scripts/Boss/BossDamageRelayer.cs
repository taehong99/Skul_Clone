using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossDamageRelayer : MonoBehaviour, IDamageable
{
    public void TakeDamage(int damage)
    {
        GetComponentInParent<BossController>().TakeDamage(damage);
    }
}
