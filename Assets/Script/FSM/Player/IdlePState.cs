using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdlePState : PlayerStates
{
    public override void Initialize()
    {
        base.Initialize();
        pState = PlayerState.Idle;
        Debug.Log("idle");
    }

    public override void OnUpdate()
    {
        player.LookAtMouse(Input.mousePosition);
        if (Input.GetMouseButton(1))
        {
            player.ChangeState(new MovePState());
        }
    }
}
