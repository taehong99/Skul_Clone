using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Destroyer : PlayerController
{
    const int whirlwindMaxAttacks = 7;
    const int ambushMaxAttacks = 5;
    const int ambushJumpForce = 20;

    [Header("Destroyer Attack")]
    private bool attackStarted;
    private bool attackQueued;
    private int attackCount;

    [Header("Swap Effect")] // Slows time for everything except himself
    [SerializeField] float slowAmount;
    [SerializeField] float slowDuration;

    Coroutine whirlwindRoutine;

    #region Skills
    protected override void UseSkill1()
    {
        if (skill1CooldownTimer > 0)
            return;

        // First time
        if (!attackStarted)
        {
            attackStarted = true;
            attackQueued = true;
            StartCoroutine(WhirlwindRoutine());
        }
        // Consecutive times
        else
        {
            Debug.Log("entered");
            attackQueued = true;
        }
    }

    private IEnumerator WhirlwindRoutine()
    {
        attackCount = 0;
        while (attackQueued)
        {
            if (attackCount == whirlwindMaxAttacks)
            {
                break;
            }
            //attackCount++;
            attackQueued = false;
            yield return StartCoroutine(Whirlwind());
            yield return null;
        }
        Debug.Log($"Finished at attackQueued:{attackQueued}, attackCount:{attackCount}");
        attackStarted = false;
        StartCoroutine(WhirlwindCooldown());
    }

    private IEnumerator Whirlwind()
    {
        animator.Play("Skill1");
        while (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Skill1")
            yield return null;

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }
        animator.Play("Idle");
        attackCount++;
    }

    private IEnumerator WhirlwindCooldown()
    {
        skill1CooldownTimer = data.skill1Cooldown;
        while (skill1CooldownTimer > 0)
        {
            skill1CooldownTimer -= Time.deltaTime;
            yield return null;
        }
        skill1CooldownTimer = 0;
    }

    protected override void UseSkill2()
    {
        if (skill2CooldownTimer > 0)
            return;

        playerSM.TransitionTo(playerSM.skillState);

        // First time
        if (!attackStarted)
        {
            attackStarted = true;
            attackQueued = true;
            StartCoroutine(SpinningAmbushRoutine());
        }
        // Consecutive times
        else
        {
            Debug.Log("entered");
            attackQueued = true;
        }
    }

    private IEnumerator SpinningAmbushRoutine()
    {
        isFlying = true;
        attackCount = 0;
        while (attackQueued)
        {
            if (attackCount == ambushMaxAttacks)
            {
                break;
            }
            //attackCount++;
            attackQueued = false;
            yield return StartCoroutine(SpinningAmbush());
            yield return null;
        }
        Debug.Log($"Finished at attackQueued:{attackQueued}, attackCount:{attackCount}");
        attackStarted = false;
        playerSM.TransitionTo(playerSM.idleState);
        StartCoroutine(SpinningAmbushCooldown());
        isFlying = false;
    }

    private IEnumerator SpinningAmbush()
    {
        //Jump Part
        animator.Play("Skill2Jump");
        rb2d.velocity = Vector2.up * ambushJumpForce;
        while (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name != "Skill2Jump")
            yield return null;

        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }
        rb2d.velocity = Vector2.zero;
        animator.StopPlayback();

        //Dive Part
        animator.Play("Skill2Dive");
        while (animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Skill2Jump")
            yield return null;
        Vector2 direction = ((facingDir == FacingDir.Left) ? Vector2.left : Vector2.right) + Vector2.down * 2;
        rb2d.velocity = direction * ambushJumpForce;
        
        while (animator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f)
        {
            yield return null;
        }
        rb2d.velocity = Vector2.zero;

        //End
        //animator.Play("Idle");
        attackCount++;
    }

    private IEnumerator SpinningAmbushCooldown()
    {
        skill2CooldownTimer = data.skill1Cooldown;
        while (skill2CooldownTimer > 0)
        {
            skill2CooldownTimer -= Time.deltaTime;
            yield return null;
        }
        skill2CooldownTimer = 0;
    }

    #endregion

    public void PlaySlashSound(int i)
    {
        switch (i)
        {
            case 0:
                Manager.Sound.PlaySFX(Manager.Sound.Data.heavySlash1);
                break;
            case 1:
                Manager.Sound.PlaySFX(Manager.Sound.Data.heavySlash2);
                break;
            case 2:
                Manager.Sound.PlaySFX(Manager.Sound.Data.heavySlashDown);
                break;
            case 3:
                Manager.Sound.PlaySFX(Manager.Sound.Data.heavySlashUp1);
                break;
            case 4:
                Manager.Sound.PlaySFX(Manager.Sound.Data.heavySlashUp2);
                break;
            case 5:
                Manager.Sound.PlaySFX(Manager.Sound.Data.heavySwoosh);
                break;
        }
    }
}
