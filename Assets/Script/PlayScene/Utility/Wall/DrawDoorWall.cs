using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawDoorWall : DrawWalls
{
    public DrawArchWall archWall;
    public bool isDrawDoor;
    public bool isHideDoor;

    void Start()
    {
        if (MiniMap != null)
        {
            MiniMap.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        ChangeWall();
        DrawWall();
        DrawOverWall();
        if (MiniMap != null)
        {
            DrawMiniMap();
        }

        CheckDraw();
        CheckHide();
    }

    public void ChangeWall()
    {
        {
            //���׸����� ���� ���ϸ� ������� ���׸��� instance���Ǿ� �񱳰� ���� �ʱ� ������ wallMaterialNum�� ������ ���� ���׸����� üũ���ش�.
            if (isHideDoor && wallMaterialNum == 0)
            {
                wallMaterialNum = 1;
                Wall.material = wallMaterials[1]; //������ ��Ʈ���� ����
            }
            else if (!isHideDoor && wallMaterialNum == 1)
            {
                wallMaterialNum = 0;
                Wall.material = wallMaterials[0]; //������ ��Ʈ���� ����
            }
        }
    }

    public void DrawWall()
    {
        if (isDrawDoor && countDonw < eraseTime)
        {
            countDonw = eraseTime;
            UnderWall.enabled = true;
        }
        if (isDrawDoor && Wall.enabled == false)
        {
            countDonw = eraseTime;
            Wall.enabled = true;
            UnderWall.enabled = true;
        }
        else if (!isDrawDoor && Wall.enabled == true)
        {
            EraseWall();
        }
    }

    public void EraseWall()
    {
        countDonw -= Time.deltaTime;
        if (countDonw <= 0)
        {
            UnderWall.enabled = false;
            Wall.enabled = false;
        }
    }

    public void DrawMiniMap()
    {
        if (isDrawDoor && !isDrawMiniMap)
        {
            isDrawMiniMap = true;
            MiniMap.SetActive(true);
        }
    }

    public void DrawOverWall()
    {
        if (Drawoverwall != null)
        {
            if (isHideDoor && Drawoverwall.activeSelf == true)
            {
                Drawoverwall.SetActive(false);
            }
            else if (!isHideDoor && Drawoverwall.activeSelf == false)
            {
                Drawoverwall.SetActive(true);
            }
        }
    }

    public void CheckDraw()
    {
        if (!isDrawDoor && archWall.isDraw)
        {
            isDrawDoor = true;
        }
        else if(isDrawDoor && !archWall.isDraw)
        {
            isDrawDoor = false;
        }
    }

    public void CheckHide()
    {
        if (!isHideDoor && archWall.isHide)
        {
            isHideDoor = true;
        }
        else if (isHideDoor && !archWall.isHide)
        {
            isHideDoor = false;
        }
    }
}
