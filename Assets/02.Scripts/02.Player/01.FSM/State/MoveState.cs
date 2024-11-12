using UnityEngine;

public class MoveState : FSMState
{
    public MoveState(FSMBase fsm) : base(fsm) { }

    public override void OnEnter(PlayerController player)
    {
        // Move ���·� ���� �� �ʱ�ȭ �ڵ�
        player.Animator.SetBool("Walk", true);
    }

    public override void OnUpdate(PlayerController player)
    {
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
