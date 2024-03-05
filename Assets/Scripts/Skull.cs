using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skull : MonoBehaviour
{
    public enum Direction { Left, Right }

    [SerializeField] float rotateSpeed;
    [SerializeField] float flySpeed;
    [SerializeField] float bounceForce;

    Vector2 dir;
    Rigidbody2D rb2d;
    Collider2D collider;
    bool isFlying = true;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        collider = GetComponent<CircleCollider2D>();
        collider.enabled = false;
    }

    private void Start()
    {
        rb2d.velocity = dir * flySpeed;
    }

    private void FixedUpdate()
    {
        if (!isFlying)
            return;

        rb2d.rotation = rb2d.rotation + rotateSpeed;
    }

    public void SetDirection(Vector2 direction)
    {
        dir = direction;
    }

    int collisionCount = 1;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collisionCount == 0)
            return;
        rb2d.gravityScale = 1f;
        Vector2 normal = collision.GetContact(0).normal;
        Vector2 direction = normal.x < 0 ? new Vector2(-0.5f, 1) : new Vector2(0.5f, 1);
        rb2d.AddForce(direction * bounceForce, ForceMode2D.Impulse);
        isFlying = false;
        collisionCount--;
        collider.enabled = true;
    }
}
