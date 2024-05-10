using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleEState : EnemyStates
{
    public override void Initialize()
    {
        eAnim = enemy.CallEnemyAnime();
        enemyState = EnemyState.Idle;
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnUpdate()
    {
        if (enemy.VisibleTarget)
        {
            if (enemy.TargetDis <= enemy.AttackRange)
            {
                stateMachine.ChangeState(new AttackEState());
            }
            else if (enemy.TargetDis <= enemy.DetectRange)
            {
                stateMachine.ChangeState(new MoveEState());
            }
        }
    }
}
