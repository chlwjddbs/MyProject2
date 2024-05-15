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
            //Target�� null�̶�� ���� enemy�� DetectRange ������ Target�� ��Ż�� �����̴�.
            //ChaseState������ ��Ż�� ����� ���� �ð� �Ѿ� ���� �ֱ� ������ ��� ���¸� ��ȯ���ش�.
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
