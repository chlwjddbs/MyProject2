using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPState : PlayerStates
{
    public override void Initialize()
    {
        base.Initialize();
        pState = PlayerState.Jump;
    }
    public override void OnUpdate()
    {
        player.transform.Translate(player.body.forward * player.MoveSpeed * Time.deltaTime, Space.World);
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
