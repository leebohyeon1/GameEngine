using UnityEngine;

public class IdleState : FSMState
{
    // 가만히 있는 상태일 때

    public IdleState(FSMBase fsm) : base(fsm) { }

    public override void OnEnter(PlayerController player)
    { 
        // Idle 상태로 진입 시 초기화 코드
    }

    public override void OnUpdate(PlayerController player)
    {
        base.OnUpdate(player);
        // Idle 상태일 때 처리할 로직
        player.Rotate();

        if(InputManager.Instance.MoveInput.magnitude > 0 )
        {
            player.SetState("Move");
        }

        if (InputManager.Instance.JumpInput)
        {
            player.Jump();
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


        player.SetGravity();
    }

    public override void OnExit(PlayerController player)
    {
        // Idle 상태에서 벗어날 때 처리할 로직
    }
}
