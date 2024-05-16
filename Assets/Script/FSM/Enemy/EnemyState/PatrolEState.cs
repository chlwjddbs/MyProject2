using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolEState : MoveEState
{
    private int patrolNum;

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnUpdate()
    {
        ChaseMode();

        if (enemy.Target != null)
        {
            if (enemy.VisibleTarget)
            {
                agent.SetDestination(enemy.Target.position);

                if (enemy.TargetDis <= enemy.AttackRange)
                {
                    stateMachine.ChangeState(new AttackEState());
                }
                else if(enemy.TargetDis <= enemy.ActionRange)
                {
                    stateMachine.ChangeState(new MoveEState());
                }
                else if (enemy.TargetDis > enemy.ActionRange)
                {
                    SetNextDestination();
                }
            }
            else
            {
                SetNextDestination();
            }
        }
        else
        {
            SetNextDestination();
        }
    }

    public void SetNextDestination()
    {
        if((enemy.PatrolPoint[patrolNum].position - enemy.transform.position).magnitude < 1f)
        {
            if (patrolNum == enemy.PatrolPoint.Length - 1)
            {
                patrolNum = 0;
            }
            else
            {
                patrolNum++;
            }
        }
        agent.SetDestination(enemy.PatrolPoint[patrolNum].position);
    }
}
