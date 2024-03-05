using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : IState
{
    private PlayerController player;

    public DashState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        player.Animator.Play("SkulDash");
    }

    public void Update()
    {
        if (!player.IsDashing)
        {
            player.fsm.TransitionTo(player.fsm.idleState);
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
                break;
            case TriggerType.SwapTrigger:
                player.fsm.TransitionTo(player.fsm.swapState);
                break;
        }
    }
}
