using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawArchWall : DrawWalls
{
    
    public MeshRenderer archWall;
    public MeshRenderer UnderWall2;

    // Start is called before the first frame update
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
    }

    public void ChangeWall()
    {
        {
            //���׸����� ���� ���ϸ� ������� ���׸��� instance���Ǿ� �񱳰� ���� �ʱ� ������ wallMaterialNum�� ������ ���� ���׸����� üũ���ش�.
            if (isHide && wallMaterialNum == 0)
            {
                wallMaterialNum = 1;
                Wall.material = wallMaterials[1]; //������ ��Ʈ���� ����
                archWall.material = wallMaterials[1];
            }
            else if (!isHide && wallMaterialNum == 1)
            {
                wallMaterialNum = 0;
                Wall.material = wallMaterials[0]; //������ ��Ʈ���� ����
                archWall.material = wallMaterials[0];
            }
        }
    }

    public void DrawWall()
    {
        if (isDraw && countDonw < eraseTime)
        {
            countDonw = eraseTime;
            UnderWall.enabled = true;
            UnderWall2.enabled = true;
        }
        if (isDraw && Wall.enabled == false)
        {
            countDonw = eraseTime;
            Wall.enabled = true;
            archWall.enabled = true;
            UnderWall.enabled = true;
            UnderWall2.enabled = true;
        }
        else if (!isDraw && Wall.enabled == true)
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
            UnderWall2.enabled = false;
            isDraw = false;
            Wall.enabled = false;
            archWall.enabled = false;
        }
    }

    public void DrawMiniMap()
    {
        if (isDraw && !isDrawMiniMap)
        {
            isDrawMiniMap = true;
            MiniMap.SetActive(true);
        }
    }

    public void DrawOverWall()
    {
        if (Drawoverwall != null)
        {
            if (isHide && Drawoverwall.activeSelf == true)
            {
                Drawoverwall.SetActive(false);
            }
            else if (!isHide && Drawoverwall.activeSelf == false)
            {
                Drawoverwall.SetActive(true);
            }
        }
    }
}
