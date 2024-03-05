using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwapState : IState
{
    private PlayerController player;

    public SwapState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        player.Animator.Play("SkulSwap");
    }

    public void Update()
    {
        if (!player.IsSwapping)
        {
            player.fsm.TransitionTo(player.fsm.idleState);
        }
    }

    public void Exit()
    {
        // code that runs when we exit the state
    }
}
