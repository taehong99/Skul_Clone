using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class FlashWhenHit : MonoBehaviour, IDamageable
{
    SpriteRenderer spriter;
    Material whiteMat;
    Material defaultMat;

    private void Awake()
    {
        spriter = GetComponent<SpriteRenderer>();
        whiteMat = Resources.Load<Material>("Materials/WhiteMat");
        defaultMat = spriter.material;
    }

    public void TakeDamage(int damage)
    {
        spriter.material = defaultMat;
        StartCoroutine(FlashWhite());
    }

    IEnumerator FlashWhite()
    {
        spriter.material = whiteMat;
        yield return new WaitForSeconds(0.1f);
        spriter.material = defaultMat;
    }
}
