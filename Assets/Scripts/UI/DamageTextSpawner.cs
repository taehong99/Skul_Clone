using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class DamageTextSpawner : MonoBehaviour, IDamageable
{
    DamageText damageText;

    private void Awake()
    {
        damageText = Manager.Resource.Load<DamageText>("Prefabs/DamageText");
    }

    public void TakeDamage(int damage)
    {
        Spawn(damage);
    }

    public void Spawn(int damage)
    {
        DamageText text = Instantiate(damageText, transform.position, transform.rotation);
        if (gameObject.CompareTag("Player"))
        {
            text.SetColor(Color.red);
        }
        text.SetText(damage);
    }
}
