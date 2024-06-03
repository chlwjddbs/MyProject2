using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPState : PlayerStates
{
    public override void Initialize()
    {
        base.Initialize();
        pState = PlayerState.Action;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        player.isAction = true;
    }

    public override void OnExit()
    {
        base.OnExit();
        player.isAction = false;
    }
}
