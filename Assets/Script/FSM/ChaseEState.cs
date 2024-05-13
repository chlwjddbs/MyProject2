using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseEState : MoveEState
{
    private Transform chaseTarget;

    public override void OnUpdate()
    {
        if (chaseTarget == null)
        {
            chaseTarget = enemy.Target;
        }

        if (chaseTarget != null)
        {        
            agent.SetDestination(chaseTarget.position);

            float targetDis = (chaseTarget.position - enemy.transform.position).magnitude;

            if (enemy.TargetDis <= enemy.AttackRange)
            {
                stateMachine.ChangeState(new AttackEState());
            }
            else if (enemy.TargetDis > enemy.ReactionRange)
            {
                stateMachine.UpdateElapsedTime();
                if(stateMachine.ElapsedTime > resetTime)
                {
                    enemy.chaseMode = false;
                    stateMachine.ChangeState(new MoveEState());
                }
            }
        }
    }

    public override void OnEnter()
    {
        base.OnEnter();
        resetTime = 7f;
    }

    public override void OnExit()
    {
        base.OnExit();
    }
}
