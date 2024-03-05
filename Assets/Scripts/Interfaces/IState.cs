using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IState
{
    public void Enter()
    {
        // code that runs when we first enter the state
    }

    public void Update()
    {
        // per-frame logic, include condition to transition to a new state
    }

    public void Exit()
    {
        // code that runs when we exit the state
    }

    public void Trigger(TriggerType trigger)
    {
        // code that runs when a trigger is triggered
    }
}
