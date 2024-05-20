using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseEState : MoveEState
{
    private float resetCount;

    public override void Initialize()
    {
        base.Initialize();
        resetTime = 7f;
    }

    public override void OnUpdate()
    {
        agent.SetDestination(enemy.ChaseTarget.position);

        if (enemy.VisibleChaseTarget(enemy.chaseTargetDir, enemy.chaseTargetDis))
        {
            if (enemy.chaseTargetDis <= enemy.AttackRange)
            {
                stateMachine.ChangeState(new IdleEState());
                return;
            }
        }
        else if (enemy.chaseTargetDis > enemy.DetectRange || !enemy.VisibleChaseTarget(enemy.chaseTargetDir, enemy.chaseTargetDis))
        {
            resetCount += Time.deltaTime;
            if (resetCount > resetTime)
            {
                enemy.ResetChaseTarget();
                stateMachine.ChangeState(new IdleEState());
            }
        }
    }

    public override void OnEnter()
    {
        base.OnEnter();
        //chaseTarget = _chaseTaret;
    }

    public override void OnExit()
    {
        base.OnExit();
        resetCount = 0;
    }

    public float Save()
    {
        return resetCount;
    }

    public void Load(float _resetCount)
    {
        resetCount = _resetCount;
    }
}

        /*        
        agent.SetDestination(chaseTarget.position);

        //캐싱해서 사용하도록 하자.
        Vector3 targetDir = (chaseTarget.position - enemy.transform.position).normalized;
        float targetDis = (chaseTarget.position - enemy.transform.position).magnitude; 
          
        if (targetDis <= enemy.AttackRange)
        {
            if (VisibleTarget(targetDir,targetDis))
            {
                stateMachine.ChangeState(new AttackEState());
            }
        }
        else if (targetDis > enemy.DetectRange || !VisibleTarget(targetDir,targetDis))
        {
            stateMachine.UpdateElapsedTime();
            if (stateMachine.ElapsedTime > resetTime)
            {
                enemy.ResetChaseTarget();
                chaseTarget = null;
                stateMachine.ChangeState(new MoveEState());
            }
        }
        */
