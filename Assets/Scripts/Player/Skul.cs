using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skul : PlayerController
{
    [Header("Skul Swap Effect")]
    [SerializeField] float swapDuration;
    [SerializeField] float swapSpeed;

    [SerializeField] GameObject skullPrefab;
    [SerializeField] float skullCooldown;
    [SerializeField] LayerMask skullMask;
    [SerializeField] RuntimeAnimatorController defaultController;
    [SerializeField] RuntimeAnimatorController headlessController;
    bool canTeleport = false;
    public bool CanTeleport => canTeleport; // TODO: refactor this

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (skullMask.Contains(collision.gameObject.layer))
        {
            PickUpSkull();
        }
    }

    #region Skills
    Coroutine throwSkullRoutine;
    GameObject thrownSkull = null;

    private void ThrowSkull()
    {
        if (cooldownTimer > 0)
            return;
        animator.Play("Skill1");
        throwSkullRoutine = StartCoroutine(ThrowSkullRoutine());
    }

    private IEnumerator ThrowSkullRoutine()
    {
        cooldownTimer = skullCooldown;
        animator.runtimeAnimatorController = headlessController;
        Vector2 direction = (facingDir == FacingDir.Left) ? Vector2.left : Vector2.right;
        thrownSkull = Instantiate(skullPrefab, transform.position, Quaternion.identity);
        thrownSkull.GetComponent<SkulProjectile>().SetDirection(direction);
        while (cooldownTimer > 0)
        {
            if (cooldownTimer < (skullCooldown - 0.5))
            {
                canTeleport = true;
            }
            cooldownTimer -= Time.deltaTime;
            yield return null;
        }
        Destroy(thrownSkull);
        canTeleport = false;
        cooldownTimer = 0;
        animator.runtimeAnimatorController = defaultController;
    }

    private void TeleportToSkull()
    {
        if (canTeleport)
        {
            transform.position = thrownSkull.transform.position;
            PickUpSkull();
        }
    }

    private void PickUpSkull()
    {
        canTeleport = false;
        StopCoroutine(throwSkullRoutine);
        Destroy(thrownSkull);
        cooldownTimer = 0;
        animator.runtimeAnimatorController = defaultController;
    }

    private IEnumerator SwapAttack()
    {
        isSwapping = true;
        playerSM.Trigger(TriggerType.SwapTrigger);
        float time = 0;
        float originalGravity = rb2d.gravityScale;
        rb2d.gravityScale = 0;
        rb2d.velocity = Vector3.zero;
        Vector3 direction = (facingDir == FacingDir.Left) ? Vector2.left : Vector2.right;

        while (time < swapDuration)
        {
            time += Time.deltaTime;
            transform.Translate(direction * swapSpeed * Time.deltaTime);
            yield return null;
        }

        //moveDir = Vector2.zero;
        rb2d.gravityScale = originalGravity;
        isSwapping = false;
    }
    #endregion

}
