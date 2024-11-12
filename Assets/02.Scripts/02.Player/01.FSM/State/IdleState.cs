using UnityEngine;

public class IdleState : FSMState
{
    public IdleState(FSMBase fsm) : base(fsm) { }

    public override void OnEnter(PlayerController player)
    { 
        // Idle 상태로 진입 시 초기화 코드
    }

    public override void OnUpdate(PlayerController player)
    {
        // Idle 상태일 때 처리할 로직
        player.Rotate();

        if(InputManager.Instance.MoveInput.magnitude > 0 )
        {
            player.SetState("Move");
        }
    }

    public override void OnExit(PlayerController player)
    {
        // Idle 상태에서 벗어날 때 처리할 로직
    }
}
