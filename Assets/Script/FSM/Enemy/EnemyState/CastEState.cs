using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastEState : EnemyStates
{
    public override void Initialize()
    {
        base.Initialize();
        enemyState = EnemyState.Casting;
    }

    public override void OnUpdate()
    {
        //enemy.CastingAction();
    }

    public override void OnEnter()
    {
        base.OnEnter();
        enemy.CastingStart();
    }

    public override void OnExit()
    {
        base.OnExit();
        enemy.isCasting = false;
    }
}
