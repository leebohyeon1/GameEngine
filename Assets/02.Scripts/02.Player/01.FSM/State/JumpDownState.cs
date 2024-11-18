using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpDownState : FSMState
{
    // ���� �� 
    public JumpDownState(FSMBase fSM) : base(fSM) { }

    public override void OnEnter(PlayerController player)
    {
        if (!player.Animator.GetCurrentAnimatorStateInfo(0).IsName("Landing"))
        {
            player.Animator.SetTrigger("Landing");
        }

        player.ActionRecorder.RecordLanding(player.transform.position, player.transform.rotation);

        player.SetZeroVelocity();

        player.Animator.SetBool("PlaceMode", false);
        player.ObjectBuilder.HandleBuildingInput(false,false);
    }

    public override void OnUpdate(PlayerController player)
    {
        base.OnUpdate(player);
    }

    public override void OnExit(PlayerController player)
    {

    }
}
