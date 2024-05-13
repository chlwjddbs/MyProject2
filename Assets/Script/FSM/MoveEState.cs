using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveEState : EnemyStates
{
    protected NavMeshAgent agent;
    protected float resetTime = 3f;

    public override void Initialize()
    {
        base.Initialize();
        enemyState = EnemyState.Walk;
        agent = enemy.GetComponent<NavMeshAgent>();
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnUpdate()
    {
        if (enemy.Target)
        {
            agent.SetDestination(enemy.Target.position);

            if (enemy.TargetDis <= enemy.AttackRange)
            {
                stateMachine.ChangeState(new AttackEState());
            }
            else if (enemy.TargetDis > enemy.ReactionRange)
            {
                stateMachine.ChangeState(new IdleEState());
            }
        }
        else
        {
            if (enemy.returnHome)
            {
                //���� �ڸ��� ���ư���.
                if ((enemy.StartPoint - enemy.transform.position).magnitude > 1.0f)
                {
                    agent.SetDestination(enemy.StartPoint);
                }
                else
                {
                    enemy.returnHome = false;
                    stateMachine.ChangeState(new IdleEState());
                }
            }
            else
            {
                agent.ResetPath();
                agent.velocity = Vector3.zero;
                stateMachine.ChangeState(new IdleEState());
            }
        }
    }

    public override void OnExit()
    {
        //�������� ���⶧ ��� ���ڸ��� ���߰� �̲������� ������
        agent.ResetPath();
        agent.velocity = Vector3.zero;
    }
}
