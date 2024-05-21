using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonSkeleton : Enemy_FSM
{

    public override void SetState()
    {
        base.SetState();
        eStateMachine.RegisterEState(new ChaseEState());
        eStateMachine.RegisterEState(new RunawayEState());
    }
}
