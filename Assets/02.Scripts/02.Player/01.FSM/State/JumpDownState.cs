using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDownState : FSMState
{
    // ÂøÁö ½Ã 
    public JumpDownState(FSMBase fSM) : base(fSM) { }

    public override void OnEnter(PlayerController player)
    {
        player.Animator.SetTrigger("Landing");
        player.SetZeroVelocity();

        player.Animator.SetBool("PlaceMode", false);
        player.ObjectBuilder.HandleBuildingInput(false,false);
    }

    public override void OnUpdate(PlayerController player)
    {

    }

    public override void OnExit(PlayerController player)
    {

    }
}
