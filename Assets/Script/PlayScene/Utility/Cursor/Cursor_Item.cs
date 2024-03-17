using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cursor_Item : Interaction
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
            //���콺�� UI�� ����ų ��
            if (PlayerController.isUI | PlayerController.isAction)
            {
                //���콺�� �����ٰ� �����Ѵ�.
                DontAction();
                return;
            }
            else
            {
                if (theDistance < actionDis)
                {
                    //Cursor.SetCursor(CursorManager.instance.itemOverCursor_Enable, CursorManager.instance.hotSpot, CursorMode.Auto);
                    //Cursor.SetCursor(CursorManager.instance.itemOverCursor_Enable, CursorManager.instance.hotSpot, CursorMode.Auto);
                    CursorManager.instance.SetCursur(CursorManager.instance.itemOverCursor_Enable);
                }
                else
                {
                    CursorManager.instance.SetCursur(CursorManager.instance.itemOverCursor_Disable);
                }
            }
        }
    }

    //Ÿ�ٿ��� ���콺�� ������ ���
    public override void DontAction()
    {
        CursorManager.instance.ResetCursor();
    }
}
