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

    public override void OnUpdate()
    {
        ChaseMode();
        #region ReturnHome On
        if (enemy.returnHome)
        {
            if (enemy.VisibleTarget)
            {
                if (enemy.TargetDis <= enemy.AttackRange)
                {
                    stateMachine.ChangeState(new AttackEState());
                }
                else if (enemy.TargetDis <= enemy.ActionRange)
                {
                    agent.SetDestination(enemy.Target.position);
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

            return;
        }
        #endregion

        if (enemy.Target != null)
        {
            if (enemy.VisibleTarget)
            {
                if (enemy.TargetDis > enemy.AttackRange && enemy.TargetDis <= enemy.ActionRange)
                {
                    agent.SetDestination(enemy.Target.position);
                    return;
                }
            }
        }

        stateMachine.ChangeState(new IdleEState());
    }

    public void ChaseMode()
    {
        if (enemy.chaseMode)
        {
            stateMachine.ChangeState(new ChaseEState());
            return;
        }
    }
    public void ReturnHome()
    {
        //원래 자리로 돌아간다.
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
    public void ReSetPath()
    {
        agent.ResetPath();
        agent.velocity = Vector3.zero;
    }

    public override void OnExit()
    {
        //움직임을 멈출때 즉시 제자리에 멈추고 미끄러짐을 방지함
        ReSetPath();
    }
}

/*
        if (enemy.Target != null)
        {
            if (enemy.VisibleTarget)
            {
                agent.SetDestination(enemy.Target.position);

                if (enemy.TargetDis <= enemy.AttackRange)
                {
                    stateMachine.ChangeState(new AttackEState());
                }
                else if (enemy.TargetDis > enemy.ActionRange)
                {
                    stateMachine.ChangeState(new IdleEState());
                }
            }
            else
            {
                stateMachine.ChangeState(new IdleEState());
            }
        }
        else
        {
            stateMachine.ChangeState(new IdleEState());
        }
        */