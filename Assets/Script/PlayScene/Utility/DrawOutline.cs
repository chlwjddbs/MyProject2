using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Interaction 을 상속해서 사용헀으나 스크립트 변경으로 인해 MonoBehaviour로 변경
public class DrawOutline : MonoBehaviour
{
    //아웃라인 메테리얼
    public Material[] outlineMaterial;

    //오리지널 메테리얼
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

    //타겟에서 마우스가 나갔을 경우
    public void DrawOrign()
    {
        //그렇지 않으면 오리지널 출력
        outline.materials = orignMaterial;

    }
}


/*
public override void OnMouseOver()
{
    DoAction(); 
}

//Interaction 참조 : 마우스가 타겟을 포지션 했을때
public override void DoAction()
{
    if (cursor.isDraw)
    {
        //아이템이 UI와 겹치거나 플레이어가 액션 중일 때 outline 미표시
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

    //타겟에서 마우스가 나갔을 경우
    public void DontAction()
    {
        //그렇지 않으면 오리지널 출력
        outline.materials = orignMaterial;

    }
*/