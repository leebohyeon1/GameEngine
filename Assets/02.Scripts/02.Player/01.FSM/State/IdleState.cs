using UnityEngine;

public class IdleState : FSMState
{
    // ������ �ִ� ������ ��

    public IdleState(FSMBase fsm) : base(fsm) { }

    public override void OnEnter(PlayerController player)
    { 
        // Idle ���·� ���� �� �ʱ�ȭ �ڵ�
    }

    public override void OnUpdate(PlayerController player)
    {
        // Idle ������ �� ó���� ����
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
        // Idle ���¿��� ��� �� ó���� ����
    }
}
