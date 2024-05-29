using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePState : PlayerStates
{
    public override void OnUpdate()
    {
        //마우스 우클릭 유지시
        if (Input.GetMouseButton(1))
        {
            player.LookAtMouse(Input.mousePosition);
            Move();
            
        }
    }

    private void Move()
    {

    }
}
