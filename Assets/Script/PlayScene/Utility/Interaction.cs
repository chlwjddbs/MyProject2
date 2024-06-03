using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Interaction : MonoBehaviour
{
    public float theDistance;
    public float actionDis = 5f;

    protected Player player;


    private void OnEnable()
    {
        player = FindObjectOfType<Player>();
    }


    // Update is called once per frame
    public virtual void Update()
    {
        
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            theDistance = player.checkObjectDis;
        }
        else
        {
            theDistance = 100f;
        }
        
        //theDistance = PlayerController.CheckDistance;
    }

    //OnMouseOver�� �ƴ� ������Ʈ���� ó���� ��� �����ۿ� DoAction�� ���� �ȴ�.
    public virtual void OnMouseOver()
    {
        if (theDistance < actionDis)
        {
            DoAction();
        }
    }

    private void OnMouseExit()
    {
        DontAction();
    }

    public virtual void DoAction()
    {

    }

    public virtual void DontAction()
    {
        player.isObject = false;
    }

    
}
