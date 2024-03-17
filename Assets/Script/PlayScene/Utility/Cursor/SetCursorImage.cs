using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetCursorImage : Interaction
{
    public bool isDraw = false;
    public cursorImages enableCursor;
    public cursorImages disableCursor;

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
                player.isObject = true;
                if (theDistance < actionDis)
                {
                    CursorManager.instance.SetCursurImage(enableCursor);
                }
                else
                {
                    CursorManager.instance.SetCursurImage(disableCursor);
                }
            }
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
}
