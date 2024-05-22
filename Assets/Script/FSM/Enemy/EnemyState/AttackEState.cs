using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEState : EnemyStates
{
    public override void Initialize()
    {
        base.Initialize();
        enemyState = EnemyState.Attack;
    }

    public override void OnEnter()
    {
        if(enemy.AttackCollider != null)
        {
            enemy.AttackCollider.enabled = true;
        }
        base.OnEnter();
    }

    public override void OnUpdate()
    {
        enemy.LookRotate();
    }

    public override void OnExit()
    {
        if (enemy.AttackCollider != null)
        {
            enemy.AttackCollider.enabled = false;
        }
        enemy.AttackedTargets.Clear();
        stateMachine.ResetAttackCoolTime();
    }
}
