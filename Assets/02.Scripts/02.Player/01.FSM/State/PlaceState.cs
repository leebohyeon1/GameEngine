using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceState : FSMState
{
    // �ǹ� ��ġ�� �� 

    public PlaceState(FSMBase fsm) : base(fsm) { }

    public override void OnEnter(PlayerController player) 
    {
        player.Animator.SetTrigger("Place");
        player.SetZeroVelocity();
    }
    public override void OnUpdate(PlayerController player)
    {
        base.OnUpdate(player);
    }

    public override void OnFixedUpdate(PlayerController player)
    {
    
    }

    public override void OnExit(PlayerController player) 
    { 

    }
}
