using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleEState : EnemyStates
{
    private float resetTime;

    public override void Initialize()
    {
        base.Initialize();
        enemyState = EnemyState.Idle;
        resetTime = 2f;
    }

    public override void OnUpdate()
    {
        if (enemy.chaseMode)
        {
            //enemy가 보일때만 공격한다.
            if (enemy.VisibleChaseTarget(enemy.chaseTargetDir, enemy.chaseTargetDis))
            {
                if (enemy.TargetDis <= enemy.AttackRange)
                {
                    enemy.LookRotate();
                    if (stateMachine.AttackCoolTime > enemy.AttackDelay)
                    {
                        stateMachine.ChangeState(new AttackEState());
                    }
                    return;
                }
            }

            stateMachine.ChangeState(new ChaseEState());

            return;
        }

        if (enemy.Target != null)
        {
            if (enemy.VisibleTarget)
            {
                if (enemy.TargetDis <= enemy.AttackRange)
                {
                    enemy.LookRotate();
                    if (stateMachine.AttackCoolTime >= enemy.AttackDelay)
                    {
                        stateMachine.ChangeState(new AttackEState());
                    }
                    return;
                }
                else if (enemy.TargetDis <= enemy.ActionRange)
                {
                    stateMachine.ChangeState(new MoveEState());
                    return;
                }
            }
        }
        SetMoveMode();
    }

    public void SetMoveMode()
    {
        if (enemy.patrolUnit)
        {
            //stateMachine.UpdateElapsedTime();
            if (stateMachine.ElapsedTime > resetTime)
            {
                stateMachine.ChangeState(new PatrolEState());
            }
        }
        else
        {
            if ((enemy.StartPoint - enemy.transform.position).magnitude > 1.0f)
            {
                //stateMachine.UpdateElapsedTime();
                if (stateMachine.ElapsedTime > resetTime)
                {
                    enemy.returnHome = true;
                    stateMachine.ChangeState(new MoveEState());
                }
            }
        }
    }
}
        /*
        if (enemy.Target != null)
        {
            if (enemy.VisibleTarget)
            {
                if (enemy.TargetDis <= enemy.AttackRange)
                {
                    stateMachine.ChangeState(new AttackEState());
                }
                else if (enemy.TargetDis <= enemy.ActionRange)
                {
                    stateMachine.ChangeState(new MoveEState());
                }
                else
                {
                    SetMoveMode();
                }
            }
            else
            {
                SetMoveMode();
            }
        }
        else
        {
            SetMoveMode();
        }
        */