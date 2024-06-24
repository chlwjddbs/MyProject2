using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Interaction �� ����ؼ� ��������� ��ũ��Ʈ �������� ���� MonoBehaviour�� ����
public class DrawOutline : MonoBehaviour
{
    //�ƿ����� ���׸���
    public Material[] outlineMaterial;

    //�������� ���׸���
    public Material[] orignMaterial;

    //public bool isDraw = false;

    private MeshRenderer outline;

    //private SetCursorImage cursor;
    // Start is called before the first frame update
    void Start()
    {
        outline = GetComponent<MeshRenderer>();
        //cursor = transform.GetComponent<SetCursorImage>();
    }

    public void DrawOutLine()
    {
        outline.materials = outlineMaterial;
    }

    //Ÿ�ٿ��� ���콺�� ������ ���
    public void DrawOrign()
    {
        //�׷��� ������ �������� ���
        outline.materials = orignMaterial;

    }
}


/*
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

 public void DoAction()
    {
        outline.materials = outlineMaterial;
    }

    //Ÿ�ٿ��� ���콺�� ������ ���
    public void DontAction()
    {
        //�׷��� ������ �������� ���
        outline.materials = orignMaterial;

    }
*/