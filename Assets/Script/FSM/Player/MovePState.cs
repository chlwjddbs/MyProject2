using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovePState : PlayerStates
{
    public override void OnUpdate()
    {
        //���콺 ��Ŭ�� ������
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
