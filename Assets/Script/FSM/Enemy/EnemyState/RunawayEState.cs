using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RunawayEState : EnemyStates
{
    private NavMeshAgent agent;
    private Vector3 runDestination;
    private float resetTime;
    public override void Initialize()
    {
        base.Initialize();
        enemyState = EnemyState.Walk;
        agent = enemy.GetComponent<NavMeshAgent>();
        resetTime = 5f;
    }

    public override void OnEnter()
    {
        base.OnEnter();
    }

    public override void OnUpdate()
    {
        //stateMachine.UpdateElapsedTime();
        if(stateMachine.ElapsedTime >= resetTime) 
        {          
            stateMachine.ChangeState(new IdleEState());
            agent.ResetPath();
            agent.velocity = Vector3.zero;
        }        
    }

    public override void OnExit()
    {
        runDestination = Vector3.zero;
    }

    public void SetDamagedDir(Vector3 _damagedDir)
    {
        if (stateMachine.ElapsedTime < resetTime)
        {
            runDestination = enemy.transform.position + (_damagedDir * enemy.DetectRange);
            agent.SetDestination(runDestination);
        }
    }

    
}
