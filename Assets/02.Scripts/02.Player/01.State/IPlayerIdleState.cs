using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPlayerIdleState : IPlayerState
{
    public void Enter(Player player)
    {
        player.PlayerAnimator.SetBool("Walk", false);
        player.PlayerAnimator.SetBool("Run", false);
    }
    public void UpdateState(Player player)
    {
       
    }
    public void FixedUpdateState(Player player)
    {
        if(InputManager.Instance.MoveInput.magnitude > 0)
        {
            player.ChangeState(new IPlayerWalkState());
        }
    }

    public void Exit(Player player)
    {
  
    }

}
