using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdlePState : PlayerStates
{
    public override void Initialize()
    {
        base.Initialize();
        pState = PlayerState.Idle;
    }

    public override void OnUpdate()
    {
        player.LookAtMouse(Input.mousePosition);

        player.Attack();

        if (player.CheckBehavior())
        {
            if (Input.GetMouseButton(1))
            {
                player.ChangeState(new MovePState());
            }

            if (Input.GetKeyDown(KeyCode.Space))
            {
                player.ChangeState(new JumpPState());
            }
        }
    }
}
