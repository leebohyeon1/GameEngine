using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : FSMState
{
    // 떨어지는 상태일 때

    public FallState(FSMBase fsm) : base(fsm) { }

    public override void OnEnter(PlayerController player)
    {
        player.Animator.SetTrigger("Fall");
    }

    public override void OnUpdate(PlayerController player)
    {
        if (player.GetIsGround())
        {
            player.SetState("JumpDown");
        }
    }

    public override void OnExit(PlayerController player)
    {

    }
}
