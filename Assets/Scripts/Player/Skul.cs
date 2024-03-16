using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skul : PlayerController
{
    [Header("Skul Skills")]
    [SerializeField] GameObject skullPrefab;
    [SerializeField] LayerMask skullMask;
    [SerializeField] RuntimeAnimatorController defaultController;
    [SerializeField] RuntimeAnimatorController headlessController;

    [Header("Skul Swap Effect")]
    [SerializeField] float swapDuration;
    [SerializeField] float swapSpeed;
    
    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        base.OnTriggerEnter2D(collision);

        if (skullMask.Contains(collision.gameObject.layer))
        {
            PickUpProjectile();
        }
    }

    #region Skills
    protected override void UseSkill1()
    {
        ThrowSkull();
    }

    protected override void UseSkill2()
    {
        TeleportToSkull();
    }

    Coroutine throwSkullRoutine;
    GameObject thrownSkull = null;
    private void ThrowSkull()
    {
        if (skill1CooldownTimer > 0)
            return;

        animator.Play("Skill1");
        throwSkullRoutine = StartCoroutine(ThrowSkullRoutine());
    }

    private IEnumerator ThrowSkullRoutine()
    {
        skill1CooldownTimer = data.skill1Cooldown;
        animator.runtimeAnimatorController = headlessController;
        Vector2 direction = (facingDir == FacingDir.Left) ? Vector2.left : Vector2.right;
        thrownSkull = Instantiate(skullPrefab, transform.position, Quaternion.identity);
        thrownSkull.GetComponent<SkulProjectile>().SetDirection(direction);
        while (skill1CooldownTimer > 0)
        {
            skill1CooldownTimer -= Time.deltaTime;
            yield return null;
        }
        Destroy(thrownSkull);
        skill1CooldownTimer = 0;
        animator.runtimeAnimatorController = defaultController;
    }

    private void TeleportToSkull()
    {
        if (skill1CooldownTimer == 0 || skill2CooldownTimer > 0)
            return;

        StartCoroutine(TeleportRoutine());
    }

    private IEnumerator TeleportRoutine()
    {
        skill2CooldownTimer = data.skill2Cooldown;
        transform.position = thrownSkull.transform.position;
        PickUpProjectile();

        while(skill2CooldownTimer > 0)
        {
            skill2CooldownTimer -= Time.deltaTime;
            yield return null;
        }
        skill2CooldownTimer = 0;
    }

    private void PickUpProjectile()
    {
        StopCoroutine(throwSkullRoutine);
        Destroy(thrownSkull);
        skill1CooldownTimer = 0;
        animator.runtimeAnimatorController = defaultController;
    }

    #endregion

    #region Swap

    protected override void SwapEffect()
    {
        StartCoroutine(SwapAttack());
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
