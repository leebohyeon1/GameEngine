using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallState : FSMState
{
    // �������� ������ ��

    public FallState(FSMBase fsm) : base(fsm) { }

    public override void OnEnter(PlayerController player)
    {
        if (!player.Animator.GetCurrentAnimatorStateInfo(0).IsName("Fall"))
        {
            player.Animator.SetTrigger("Fall");
        }
    }

    public override void OnUpdate(PlayerController player)
    {
        base.OnUpdate(player);
        
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
