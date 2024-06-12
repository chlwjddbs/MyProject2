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
        //PlayerAnimecontrol에서 켜준다.
        //player.AttackCollider.enabled = true; 
    }

    public override void OnExit()
    {
        player.AttackCollider.enabled = false;
        player.AttackedTargets.Clear();
    }
}
