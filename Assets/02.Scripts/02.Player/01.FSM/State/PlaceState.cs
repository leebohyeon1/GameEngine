using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceState : FSMState
{
    // 건물 설치할 때 

    public PlaceState(FSMBase fsm) : base(fsm) { }

    public override void OnEnter(PlayerController player) 
    {
        player.Animator.SetTrigger("Place");
        player.SetZeroVelocity();
    }
    public override void OnUpdate(PlayerController player)
    {
    }

    public override void OnFixedUpdate(PlayerController player)
    {
    
    }

    public override void OnExit(PlayerController player) 
    { 

    }
}
