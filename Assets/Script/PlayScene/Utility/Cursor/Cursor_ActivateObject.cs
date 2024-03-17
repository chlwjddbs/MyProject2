using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor_ActivateObject : Interaction
{
    public bool isDraw = false;

    // Start is called before the first frame update

    public override void OnMouseOver()
    {
        DoAction();
    }

    //Interaction ���� : Ÿ�ٰ� �÷��̾��� �Ÿ��� actionDis �̳��̰� ���콺�� Ÿ���� ������ ������
    public override void DoAction()
    {
        if (isDraw)
        {
            if (PlayerController.isUI | PlayerController.isAction)
            {
                DontAction();
                return;
            }
            else
            {
                if (theDistance < actionDis)
                {
                    player.isObject = true;
                    //Cursor.SetCursor(CursorManager.instance.activateObjectCursor, CursorManager.instance.hotSpot, CursorMode.Auto);
                    CursorManager.instance.SetCursur(CursorManager.instance.activateObjectCursor);
                    
                }
                else
                {
                    //Cursor.SetCursor(CursorManager.instance.DisabledObjectCursor, CursorManager.instance.hotSpot, CursorMode.Auto);
                    CursorManager.instance.SetCursur(CursorManager.instance.DisabledObjectCursor);
                }
            }
        }
    }

    //Ÿ�ٿ��� ���콺�� ������ ���
    public override void DontAction()
    {
        player.isObject = false;
        //Cursor.SetCursor(CursorManager.instance.orginCursor, Vector2.zero, CursorMode.Auto);
        CursorManager.instance.ResetCursor();
    }

    private void OnDisable()
    {
        player.isObject = false;
        CursorManager.instance.ResetCursor();
    }
}
