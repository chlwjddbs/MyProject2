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
                //enemy�� ���϶��� �����Ѵ�.
                if (enemy.VisibleTarget)
                {
                    if (enemy.TargetDis <= enemy.AttackRange)
                    {
                        stateMachine.ChangeState(new AttackEState());
                        return;
                    }
                }

                stateMachine.ChangeState(new ChaseEState());

            }
            //Target�� null�̶�� ���� enemy�� DetectRange ������ Target�� ��Ż�� �����̴�.
            //ChaseState������ ��Ż�� ����� ���� �ð� �Ѿ� ���� �ֱ� ������ ��� ���¸� ��ȯ���ش�.
            else
            {
                stateMachine.ChangeState(new ChaseEState());
            }

            return;
        }


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
    }

    public void SetMoveMode()
    {
        if (enemy.patrolUnit)
        {
            stateMachine.UpdateElapsedTime();
            if (stateMachine.ElapsedTime > resetTime)
            {
                stateMachine.ChangeState(new PatrolEState());
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
