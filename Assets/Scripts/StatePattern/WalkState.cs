using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkState : IState
{
    private PlayerController player;

    public WalkState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        player.Animator.Play("Walk");
    }

    public void Update()
    {
        if (!player.IsGrounded)
        {
            player.fsm.TransitionTo(player.fsm.jumpState);
        }
        if (Mathf.Abs(player.MoveDir.x) == 0)
        {
            player.fsm.TransitionTo(player.fsm.idleState);
        }
    }

    public void Exit()
    {
        
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
