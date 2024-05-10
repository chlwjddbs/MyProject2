using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEState : EnemyStates
{
    public override void Initialize()
    {
        eAnim = enemy.CallEnemyAnime();
        enemyState = EnemyState.Attack;
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnUpdate()
    {
        enemy.LookRotate();
    }

    public override void OnExit()
    {
        
    }
}
