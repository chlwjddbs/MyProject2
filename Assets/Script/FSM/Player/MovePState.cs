using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePState : PlayerStates
{
    private float runRange;
    private bool isRun;
    private PlayerState pRunState;

    public override void Initialize()
    {
        base.Initialize();
        pState = PlayerState.Walk;
        pRunState = PlayerState.Run;
        runRange = 5f;
        Debug.Log("move");

    }

    public override void OnUpdate()
    {
        player.LookAtMouse(Input.mousePosition);
        Move();
        
        if (Input.GetMouseButtonUp(1))
        {
            player.ChangeState(new IdlePState());
        }
    }

    private void Move()
    {
        //player.mousePoint.normalized == player.transform.forward
        //LookAtMouse에서 마우스 방향을 player의 forward로 설정해줬음.
        if (player.mousePos.magnitude < runRange && !isRun)
        {
            if(pAnim.GetInteger("pState") == (int)pRunState)
            {
                ChangePAnime(pState);
            }
            player.transform.Translate(player.body.forward * (player.MoveSpeed / 2) * Time.deltaTime, Space.World);
        }
        else
        {
            isRun = true;
            if (pAnim.GetInteger("pState") == (int)pState)
            {
                ChangePAnime(pRunState);
            }
            player.transform.Translate(player.body.forward * player.MoveSpeed * Time.deltaTime, Space.World);
        }
    }

    public override void OnExit()
    {
        isRun = false;
    }
}
