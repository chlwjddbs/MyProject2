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
            //���콺�� UI�� ����ų ��
            if (player.isUI | player.isAction)
            {
                //���콺�� �����ٰ� �����Ѵ�.
                DontAction();
                return;
            }
            else
            {
                DoAction();
            }
        }
    }

    //Interaction ���� : Ÿ�ٰ� �÷��̾��� �Ÿ��� actionDis �̳��̰� ���콺�� Ÿ���� ������ ������
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

    //Ÿ�ٿ��� ���콺�� ������ ���
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
