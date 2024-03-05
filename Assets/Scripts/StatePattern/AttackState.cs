using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{
    private PlayerController player;
    private int remainingAttacks;
    private bool queuedAttackB;

    public AttackState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        // Set isAttacking to true
        player.ToggleIsAttacking(true);

        queuedAttackB = false;

        // code that runs when we first enter the state
        if (!player.IsGrounded)
        {
            player.Animator.Play("SkulJumpAttack");
            remainingAttacks = 0;
        }
        else
        {
            player.Animator.Play("SkulAttackA");
            remainingAttacks = 1;
        }
    }

    public void Update()
    {
        // fix for mysterious bug (jumpAttack doesn't work at highest point)
        if (player.Animator.GetCurrentAnimatorClipInfo(0)[0].clip.name == "SkulJump")
            return;

        // if attack animation has completed
        if (player.Animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            if (queuedAttackB)
            {
                player.Animator.Play("SkulAttackB");
                queuedAttackB = false;
                return;
            }
            player.fsm.TransitionTo(player.fsm.idleState);
        }
    }

    public void Exit()
    {
        // Set isAttacking to false
        player.ToggleIsAttacking(false);
    }

    public void Trigger(TriggerType trigger)
    {
        switch (trigger)
        {
            case TriggerType.AttackTrigger:
                if (remainingAttacks > 0)
                {
                    queuedAttackB = true;
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
