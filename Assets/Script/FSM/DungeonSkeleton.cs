using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonSkeleton : Enemy_FSM
{
    public override void SetData()
    {
        base.SetData();
        eStateMachine.RegisterEState(new ChaseEState());
    }
}
