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
    }

    public override void OnExit(PlayerController player)
    {
        // Idle ���¿��� ��� �� ó���� ����
    }
}
