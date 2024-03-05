using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FlashWhenHit : MonoBehaviour, IDamageable
{
    SpriteRenderer spriter;
    Material whiteMat;

    private void Awake()
    {
        spriter = GetComponent<SpriteRenderer>();
        whiteMat = Resources.Load<Material>("Materials/WhiteMat");
    }

    public void TakeDamage(int damage)
    {
        Debug.Log("Flashed");
        StartCoroutine(FlashWhite());
    }

    IEnumerator FlashWhite()
    {
        Material defaultMat = spriter.material;
        spriter.material = whiteMat;
        yield return new WaitForSeconds(0.1f);
        spriter.material = defaultMat;
    }
}
