using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyerAttackState : AttackState
{
    private PlayerController player;
    private int remainingAttacks;
    private bool queuedAttack;

    public DestroyerAttackState(PlayerController player) : base(player)
    {
        this.player = player;
    }

    public override void Enter()
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
            remainingAttacks = 2;
        }
    }

    public override void Update()
    {
        // fix for mysterious bug (jumpAttack doesn't work at highest point)
        if (player.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "Jump")
            return;

        // attack combo
        if (player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            if (queuedAttack)
            {
                if (remainingAttacks == 1)
                {
                    player.Animator.Play("AttackB");
                }
                else
                {
                    player.Animator.Play("AttackC");
                }

                queuedAttack = false;
                return;
            }
            player.fsm.TransitionTo(player.fsm.idleState);
        }
    }

    public override void Exit()
    {
        player.ToggleIsAttacking(false);
    }

    public override void Trigger(TriggerType trigger)
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
            case TriggerType.SwapTrigger:
                player.fsm.TransitionTo(player.fsm.swapState);
                break;
        }
    }
}
