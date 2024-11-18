using System.Collections;
using System.Collections.Generic;
using Unity.XR.OpenVR;
using UnityEngine;

public class JumpUpState : FSMState
{
    // ���� ���� ����

    public JumpUpState(FSMBase fsm) : base(fsm) { }

    public override void OnEnter(PlayerController player)
    {
        if (!player.Animator.GetCurrentAnimatorStateInfo(0).IsName("Jump"))
        { 
            player.Animator.SetTrigger("Jump");
        }


        player.ActionRecorder.RecordJump(player.transform.position, player.transform.rotation);
    }

    public override void OnUpdate(PlayerController player)
    {
        base.OnUpdate(player);

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
