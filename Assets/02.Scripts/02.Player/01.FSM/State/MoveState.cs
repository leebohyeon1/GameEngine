using UnityEngine;

public class MoveState : FSMState
{
    // 걷는 상태일 때

    public MoveState(FSMBase fsm) : base(fsm) { }

    public override void OnEnter(PlayerController player)
    {
        // Move 상태로 진입 시 초기화 코드
        player.Animator.SetBool("Walk", true);
    }

    public override void OnUpdate(PlayerController player)
    {
        base.OnUpdate(player);

        if (InputManager.Instance.JumpInput)
        {
            player.Jump();
        }

        // 이동 로직 구현
        player.Move();
        player.Rotate();

        if (InputManager.Instance.MoveInput.magnitude < 0.1f)
        {
            player.SetState("Idle");
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
        // Move 상태에서 벗어날 때 처리할 로직
        player.Animator.SetBool("Walk", false);
    }
}
