using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestMovement : MonoBehaviour
{
    [SerializeField] float moveSpeed;
    [SerializeField] float acceleration;
    [SerializeField] float deceleration;
    [SerializeField] float velPower;

    Vector2 moveDir;
    Rigidbody2D rb2d;
    Animator animator;
    SpriteRenderer spriter;

    private void Awake()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriter = GetComponent<SpriteRenderer>();
    }

    private void FixedUpdate()
    {
        float targetSpeed = moveDir.x * moveSpeed;
        float speedDiff = targetSpeed - rb2d.velocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;

        float movement = Mathf.Pow(Mathf.Abs(speedDiff) * accelRate, velPower) * Mathf.Sign(speedDiff);

        rb2d.AddForce(movement * Vector2.right);
    }

    private void OnMove(InputValue value)
    {
        moveDir = value.Get<Vector2>();
        animator.SetFloat("xSpeed", Mathf.Abs(moveDir.x));
        if (moveDir.x < 0)
            spriter.flipX = true;
        else if (moveDir.x > 0)
            spriter.flipX = false;
    }
}