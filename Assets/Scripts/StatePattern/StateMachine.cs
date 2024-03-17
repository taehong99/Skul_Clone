using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum TriggerType { AttackTrigger, DashTrigger, SkillTrigger }
[Serializable]
public class StateMachine
{
    public IState curState { get; private set; }

    // References to states
    public IdleState idleState;
    public WalkState walkState;
    public JumpState jumpState;
    public AttackState attackState;
    public DashState dashState;
    public SkillState skillState;

    // State Machine Constructor
    public StateMachine(PlayerController player)
    {
        // create an instance for each state and pass in PlayerController
        this.walkState = new WalkState(player);
        this.jumpState = new JumpState(player);
        this.idleState = new IdleState(player);
        if (player.Data.skullName == "Skul")
            this.attackState = new AttackState(player);
        else if (player.Data.skullName == "Destroyer")
            this.attackState = new DestroyerAttackState(player);
        this.dashState = new DashState(player);
        this.skillState = new SkillState(player);
    }

    public string CurState()
    {
        switch (curState)
        {
            case IdleState:
                return "Idle";
            case WalkState:
                return "Walk";
            case JumpState:
                return "Jump";
            case AttackState:
                return "Attack";
            case DashState:
                return "Dash";
            case SkillState:
                return "Skill";
            default:
                return "None";
        }
    }

    // set the starting state
    public void Initialize(IState state)
    {
        curState = state;
        state.Enter();
    }

    // exit this state and enter another
    public void TransitionTo(IState nextState)
    {
        curState.Exit();
        curState = nextState;
        nextState.Enter();

        Debug.Log($"Entered {nextState.GetType()}");
    }

    // allow the StateMachine to update this state
    public void Update()
    {
        if (curState != null)
        {
            curState.Update();
        }
    }

    public void Trigger(TriggerType trigger)
    {
        if(curState != null)
        {
            curState.Trigger(trigger);
        }
    }
}
