using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPlayerRunState : IPlayerState
{
    public void Enter(Player player)
    {
        player.PlayerAnimator.SetBool("Run", true);
    }
    public void UpdateState(Player player)
    {
        if (InputManager.Instance.MoveInput.magnitude <= 0)
        {
            player.ChangeState(new IPlayerIdleState());
        }

        if (!InputManager.Instance.RunInput)
        {
            player.ChangeState(new IPlayerWalkState());
        }

        if(InputManager.Instance.SlideInput)
        {
            player.ChangeState(new IPlayerSlideState());
        }
    }
    public void FixedUpdateState(Player player)
    {
        player.Move();
    }

    public void Exit(Player player)
    {
        if(!InputManager.Instance.SlideInput)
        {
            player.PlayerAnimator.SetBool("Run", false);
        }

    }
}
