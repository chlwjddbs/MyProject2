using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveEState : EnemyStates
{
    private NavMeshAgent agent;
    public override void Initialize()
    {
        eAnim = enemy.CallEnemyAnime();
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
            stateMachine.ChangeState(new IdleEState());
        }
    }

    public override void OnExit()
    {
        agent.ResetPath();
    }
}
