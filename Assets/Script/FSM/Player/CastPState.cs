using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastPState : PlayerStates
{
    public override void Initialize()
    {
        base.Initialize();
        pState = PlayerState.Casting;
    }

    public override void OnUpdate()
    {
        if (!player.isAction)
        {
            player.LookAtMouse(Input.mousePosition);
        }
    }

    public override void OnEnter()
    {
        base.OnEnter();
        player.isCasting = true;
    }

    public override void OnExit()
    {
        base.OnExit();
        player.isCasting = false;
        player.isAction = false;
    }
}
