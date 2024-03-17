using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{
    private PlayerController player;
    private int remainingAttacks;
    private bool queuedAttack;

    public AttackState(PlayerController player)
    {
        this.player = player;
    }

    public virtual void Enter()
    {
        player.ToggleIsAttacking(true);

        queuedAttack = false;
        if (!player.IsGrounded)
        {
            player.Animator.Play("JumpAttack");
            remainingAttacks = 0;
        }
        else
        {
            player.Animator.Play("AttackA");
            remainingAttacks = 1;
        }
    }

    public virtual void Update()
    {
        // fix for mysterious bug (jumpAttack doesn't work at highest point)
        if (player.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Jump")
            return;

        // attack combo
        if (player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            if (queuedAttack)
            {
                player.Animator.Play("AttackB");
                queuedAttack = false;
                return;
            }
            player.fsm.TransitionTo(player.fsm.idleState);
        }
    }

    public virtual void Exit()
    {
        // Set isAttacking to false
        player.ToggleIsAttacking(false);
    }

    public virtual void Trigger(TriggerType trigger)
    {   
        switch (trigger)
        {
            case TriggerType.AttackTrigger:
                if (remainingAttacks > 0 && queuedAttack == false)
                {
                    queuedAttack = true;
                    remainingAttacks--;
                }
                break;
            case TriggerType.DashTrigger:
                player.fsm.TransitionTo(player.fsm.dashState);
                break;
            case TriggerType.SkillTrigger:
                player.fsm.TransitionTo(player.fsm.skillState);
                break;
        }
    }
}
