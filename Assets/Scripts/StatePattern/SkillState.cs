using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillState : IState
{
    private PlayerController player;

    public SkillState(PlayerController player)
    {
        this.player = player;
    }

    //Do nothing
    public void Enter()
    {
        
    }

    public void Update()
    {
        
    }

    public void Exit()
    {
        // code that runs when we exit the state
    }
}
