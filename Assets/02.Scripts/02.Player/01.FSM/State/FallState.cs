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
        player.SetGravity();

        if (player.GetIsGround())
        {
            player.SetState("JumpDown");
        }

        if (InputManager.Instance.IsPlaceMode)
        {
            player.Animator.SetBool("PlaceMode", true);
            if (InputManager.Instance.PlaceInput)
            {
                player.SetState("Place");
            }
        }
        else
        {
            player.Animator.SetBool("PlaceMode", false);
        }
        player.ObjectBuilder.HandleBuildingInput(InputManager.Instance.IsPlaceMode, InputManager.Instance.PlaceInput);

    }

    public override void OnExit(PlayerController player)
    {

    }
}
