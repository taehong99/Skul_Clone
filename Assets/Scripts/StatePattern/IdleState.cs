using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : IState
{
    private PlayerController player;

    public IdleState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        player.Animator.Play("Idle");
    }

    public void Update()
    {
        if (!player.IsGrounded)
        {
            player.fsm.TransitionTo(player.fsm.jumpState);
        }
        if(Mathf.Abs(player.MoveDir.x) > 0.1f)
        {
            player.fsm.TransitionTo(player.fsm.walkState);
        }
    }

    public void Exit()
    {
        // code that runs when we exit the state
    }

    public void Trigger(TriggerType trigger)
    {
        switch (trigger)
        {
            case TriggerType.AttackTrigger:
                player.fsm.TransitionTo(player.fsm.attackState);
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
