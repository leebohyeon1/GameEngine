using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IPlayerSlideState : IPlayerState
{
    public void Enter(Player player)
    {
        player.PlayerAnimator.SetTrigger("Sliding");
        player.StartCoroutine(player.Slide());
    }
   
    public void UpdateState(Player player)
    {

    }

    public void FixedUpdateState(Player player)
    {

    }

    public void Exit(Player player)
    {
        
    }

 
}
