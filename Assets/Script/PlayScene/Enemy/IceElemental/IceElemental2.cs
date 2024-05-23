using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceElemental2 : Enemy_FSM
{
    protected override void Start()
    {
        SetData();
    }

    public override void SetState()
    {
        base.SetState();
        eStateMachine.RegisterEState(new ChaseEState());
    }

    protected override void Update()
    {
        if (!isDeath)
        {
            eStateMachine.Update(Time.deltaTime);

            if (eStateMachine.AttackCoolTime <= AttackDelay)
            {
                eStateMachine.AttackTimeCount();
            }

            if (chaseMode)
            {
                SearchChaseTarget();
            }
            else
            {
                if(Target != null)
                {
                    chaseMode = true;
                    SetChaseTarget(Target);
                }
            }
        }
    }
}
