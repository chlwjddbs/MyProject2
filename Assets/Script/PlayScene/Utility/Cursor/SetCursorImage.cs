using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCursorImage : Interaction
{
    public bool isDraw = false;
    public bool overMouse = false;

    public cursorImages enableCursor;
    public cursorImages disableCursor;

    public override void LateUpdate()
    {
        base.LateUpdate();
    }

    /*
    public override void OnMouseOver()
    {
        DoAction();
    }
    */

    public override void MouseOver()
    {
        if (isDraw && overMouse)
        {
            //마우스가 UI를 가르킬 때
            if (player.isUI | player.isAction)
            {
                //마우스가 나갔다고 판정한다.
                DontAction();
                return;
            }
            else
            {
                DoAction();
            }
        }
    }

    //Interaction 참조 : 타겟과 플레이어의 거리가 actionDis 이내이고 마우스가 타겟을 포지션 했을때
    public override void DoAction()
    {
        if (theDistance < actionDis)
        {
            player.isObject = true;
            CursorManager.instance.SetCursurImage(enableCursor);
        }
        else
        {
            CursorManager.instance.SetCursurImage(disableCursor);
        }
    }

    //타겟에서 마우스가 나갔을 경우
    public override void DontAction()
    {
        player.isObject = false;
        CursorManager.instance.ResetCursor();
    }

    private void OnDestroy()
    {
        player.isObject = false;
        CursorManager.instance.ResetCursor();
    }

    private void OnDisable()
    {
        player.isObject = false;
        CursorManager.instance.ResetCursor();
    }
}
