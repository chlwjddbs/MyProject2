using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPState : PlayerStates
{
    public override void Initialize()
    {
        base.Initialize();
        pState = PlayerState.Attack;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        player.SetDamage();
        player.isAction = true;
        //PlayerAnimecontrol���� ���ش�.
        //player.AttackCollider.enabled = true; 
    }

    public override void OnExit()
    {
        player.ResetAttack();
        player.SetDamage();
    }
}
