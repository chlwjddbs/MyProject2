using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseEState : MoveEState
{
    private Transform chaseTarget;

    public override void OnUpdate()
    {
        agent.SetDestination(chaseTarget.position);

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
    }

    public override void OnEnter()
    {
        base.OnEnter();
        resetTime = 7f;
        //chaseTarget = _chaseTaret;
    }

    public void SetTarget(Transform _chaseTaret)
    {       
        chaseTarget = _chaseTaret;   
    }

    public bool VisibleTarget(Vector3 _dir, float _dis)
    {
        if (Physics.Raycast(enemy.transform.position + enemy.searchPlayer.SightOffset, _dir, _dis, 1 << 12))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    //OnEixt는 변경 사항이 없어서 작성하지 않음.

}
