using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawOutline : Interaction
{
    //�ƿ����� ���׸���
    public Material[] outlineMaterial;

    //�������� ���׸���
    public Material[] orignMaterial;

    public bool isDraw = false;

    private MeshRenderer outline;

    private SetCursorImage cursor;
    // Start is called before the first frame update
    void Start()
    {
        outline = GetComponent<MeshRenderer>();
        cursor = transform.GetComponent<SetCursorImage>();
    }


    public override void OnMouseOver()
    {
        DoAction();
    }

    //Interaction ���� : ���콺�� Ÿ���� ������ ������
    public override void DoAction()
    {
        if (cursor.isDraw)
        {
            //�������� UI�� ��ġ�ų� �÷��̾ �׼� ���� �� outline ��ǥ��
            if (player.isUI | player.isAction)
            {
                DontAction();
                return;
            }
            else
            {
                outline.materials = outlineMaterial;
            }
        }
    }

    //Ÿ�ٿ��� ���콺�� ������ ���
    public override void DontAction()
    {
        //�׷��� ������ �������� ���
        outline.materials = orignMaterial;

    }
}
