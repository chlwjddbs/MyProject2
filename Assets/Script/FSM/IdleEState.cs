using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleEState : EnemyStates
{
    private Transform chaseTarget;
    private float resetTime;

    public override void Initialize()
    {
        base.Initialize();
        enemyState = EnemyState.Idle;
        resetTime = 2f;
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnUpdate()
    {
        if(enemy.chaseMode)
        {
            if(chaseTarget == null)
            {
                chaseTarget = enemy.Target;
            }

            if (chaseTarget != null)
            {
                float targetDis = (chaseTarget.position - enemy.transform.position).magnitude;

                if (targetDis <= enemy.AttackRange)
                {
                    stateMachine.ChangeState(new AttackEState());
                }
                else
                {
                    stateMachine.ChangeState(new ChaseEState());
                }
            }
        }
        else
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
            else
            {
                if ((enemy.StartPoint - enemy.transform.position).magnitude > 1.0f)
                {
                    stateMachine.UpdateElapsedTime();
                    if (stateMachine.ElapsedTime > resetTime)
                    {
                        enemy.returnHome = true;
                        stateMachine.ChangeState(new MoveEState());
                    }
                }
            }
        }
    }
}
