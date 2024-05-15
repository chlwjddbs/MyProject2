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

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnUpdate()
    {
        if (enemy.chaseMode)
        {
            if (enemy.Target != null)
            {
                if (enemy.TargetDis <= enemy.AttackRange)
                {
                    stateMachine.ChangeState(new AttackEState());
                }
                else
                {
                    stateMachine.ChangeState(new ChaseEState());
                }
            }
            //Target이 null이라는 것은 enemy의 DetectRange 밖으로 Target이 이탈한 상태이다.
            //ChaseState에서는 이탈한 대상을 일정 시간 쫓아 갈수 있기 때문에 즉시 상태를 변환해준다.
            else
            {
                stateMachine.ChangeState(new ChaseEState());
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
                else if (enemy.TargetDis <= enemy.ActionRange)
                {
                    stateMachine.ChangeState(new MoveEState());
                }
                else
                {
                    ReturnHome();
                }
            }
            else
            {
                ReturnHome();
            }
        }
    }

    public void ReturnHome()
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
