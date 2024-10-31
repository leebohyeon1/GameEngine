using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPlayerWalkState :IPlayerState
{
    public void Enter(Player player)
    {
        player.PlayerAnimator.SetBool("Walk", true);
    }
    public void UpdateState(Player player)
    {
        if (InputManager.Instance.MoveInput.magnitude <= 0)
        {
            player.ChangeState(new IPlayerIdleState());
        }

        if(InputManager.Instance.RunInput)
        {
            player.ChangeState(new IPlayerRunState());
        }
    }
    public void FixedUpdateState(Player player)
    {
        player.Move();
    }

    public void Exit(Player player)
    {
        if (!InputManager.Instance.RunInput)
        {
            player.PlayerAnimator.SetBool("Walk", false);
        }
    }
}

