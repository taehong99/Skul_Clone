using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    [SerializeField] LayerMask hittableMask;
    [SerializeField] float moveSpeed;
    [SerializeField] float shootDelay;
    private Rigidbody2D rb2d;
    private Coroutine coroutine;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
    }

    private void OnEnable()
    {
        coroutine = StartCoroutine(MoveTowardsTarget());
    }

    private void OnDisable()
    {
        StopCoroutine(coroutine);
    }

    private IEnumerator MoveTowardsTarget()
    {
        yield return new WaitForSeconds(shootDelay);
        Vector2 targetDir = (Manager.Game.Player.transform.position - transform.position).normalized * moveSpeed;
        rb2d.velocity = targetDir;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (hittableMask.Contains(collision.gameObject.layer))
        {
            Destroy(gameObject);
        }
    }
}
