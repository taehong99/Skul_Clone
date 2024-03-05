using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpState : IState
{
    private PlayerController player;
    private float lastYVel;

    public JumpState(PlayerController player)
    {
        this.player = player;
    }

    public void Enter()
    {
        // play animation depending on y velocity
        if (player.Rb2d.velocity.y > 0.1f)
        {
            player.Animator.Play("SkulJump");
        }
        else if (player.Rb2d.velocity.y < -0.1f)
        {
            player.Animator.Play("SkulFall");
        }
        lastYVel = player.Rb2d.velocity.y;
    }

    public void Update()
    {
        // change animation depending on y velocity
        if(Mathf.Sign(player.Rb2d.velocity.y) != Mathf.Sign(lastYVel))
        {
            if (player.Rb2d.velocity.y > 0.1f)
            {
                player.Animator.Play("SkulJump");
            }
            else
            {
                player.Animator.Play("SkulFall");
            }
        }
        lastYVel = player.Rb2d.velocity.y;
        // TODO: Add FallRepeat 

        // state transition
        if (player.IsGrounded)
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
                player.fsm.TransitionTo(player.fsm.dashState);
                break;
        }
    }
}
