using UnityEngine;

public class MoveState : FSMState
{
    // �ȴ� ������ ��

    public MoveState(FSMBase fsm) : base(fsm) { }

    public override void OnEnter(PlayerController player)
    {
        // Move ���·� ���� �� �ʱ�ȭ �ڵ�
        player.Animator.SetBool("Walk", true);
    }

    public override void OnUpdate(PlayerController player)
    {
        if (InputManager.Instance.JumpInput)
        {
            player.Jump();
        }

        // �̵� ���� ����
        player.Move();
        player.Rotate();

        if (InputManager.Instance.MoveInput.magnitude < 0.1f)
        {
            player.SetState("Idle");
        }

    
    }

    public override void OnExit(PlayerController player)
    {
        // Move ���¿��� ��� �� ó���� ����
        player.Animator.SetBool("Walk", false);
    }
}
