using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour
{
    private void OnEnable()
    {
        StartCoroutine(hitFrame());
    }

    private IEnumerator hitFrame() {
        yield return new WaitForSeconds(1);
        gameObject.SetActive(false);
    }
}
