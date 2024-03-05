using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public enum TriggerType { AttackTrigger, DashTrigger }
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

    // event to notify other objects of the state change
    //public event Action<IState> stateChanged;

    // State Machine Constructor
    public StateMachine(PlayerController player)
    {
        // create an instance for each state and pass in PlayerController
        this.walkState = new WalkState(player);
        this.jumpState = new JumpState(player);
        this.idleState = new IdleState(player);
        this.attackState = new AttackState(player);
        this.dashState = new DashState(player);
    }

    // set the starting state
    public void Initialize(IState state)
    {
        curState = state;
        state.Enter();

        // notify other objects that state has changed
        //stateChanged?.Invoke(state);
    }

    // exit this state and enter another
    public void TransitionTo(IState nextState)
    {
        curState.Exit();
        curState = nextState;
        nextState.Enter();

        // notify other objects that state has changed
        //stateChanged?.Invoke(nextState);
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
