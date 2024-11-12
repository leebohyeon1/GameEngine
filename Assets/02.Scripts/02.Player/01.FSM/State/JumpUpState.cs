using System.Collections;
using System.Collections.Generic;
using Unity.XR.OpenVR;
using UnityEngine;

public class JumpUpState : FSMState
{
    // 점프 시작 상태

    public JumpUpState(FSMBase fsm) : base(fsm) { }

    public override void OnEnter(PlayerController player)
    {
        player.Animator.SetTrigger("Jump");
    }

    public override void OnUpdate(PlayerController player)
    {

    }

    public override void OnExit(PlayerController player)
    {
        
    }

}
