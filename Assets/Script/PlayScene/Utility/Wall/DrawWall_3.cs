using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawWall_3 : DrawWalls
{
    /*
    public float eraseTime;
    private float countDonw;

    //0�̸� ������, 1�̸� ������
    private float wallMaterialNum;
    public GameObject Drawoverwall;
    public GameObject MiniMap;
    private bool isDrawMiniMap = false;
    */

    //public List<Material> eraseMaterials;

    //private int test;
    //public bool isErase;
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
        //if(!isErase)
        //if (isDraw) : isDraw ������ ���� �ʾƵ� �þ߿� ������ ������ isHide�� ������ ���� �ʾ� ��� �����ϴ�.
        {
            //���׸����� ���� ���ϸ� ������� ���׸��� instance���Ǿ� �񱳰� ���� �ʱ� ������ wallMaterialNum�� ������ ���� ���׸����� üũ���ش�.
            if (isHide && wallMaterialNum == 0)
            {
                wallMaterialNum = 1;
                Wall.material = wallMaterials[1]; //������ ��Ʈ���� ����
            }
            else if (!isHide && wallMaterialNum == 1)
            {
                wallMaterialNum = 0;
                Wall.material = wallMaterials[0]; //������ ��Ʈ���� ����
            }
        }
    }

    public void DrawWall()
    {
        //���� �þ߿��� ����� �ٽ� �������� ���� �������� ���� ���¶�� eraseTime�� ���� �����־�� �Ѵ�.
        if(isDraw && countDonw < eraseTime)
        {
            countDonw = eraseTime;
            UnderWall.enabled = true;
            //isErase = false;
        }
        if (isDraw && Wall.enabled == false)
        {
            countDonw = eraseTime;
            //isErase = false;
            Wall.enabled = true;
            UnderWall.enabled = true;
        }               
        else if(!isDraw && Wall.enabled == true)
        {
            EraseWall();
        }
    }

    //�þ߿� ������� �������� �ʰ� �ܻ��� �����ش�.
    public void EraseWall()
    {
        countDonw -= Time.deltaTime;
        if (countDonw <= 0)
        {
            UnderWall.enabled = false;
            isDraw = false;
            Wall.enabled = false;
        }

        /*
        if(UnderWall.enabled == true)
        {
            UnderWall.enabled = false;
        }
        */

        /*
       isErase = true;
        countDonw -= Time.deltaTime;

        //Debug.Log(test);
        if ((countDonw > 0))
        {
            if (countDonw <= eraseTime - (eraseTime * ((float)(test) / (float)(eraseMaterials.Count))))
            {
                Wall.material = eraseMaterials[test];
                test++;
                UnderWall.enabled = false;
            }
        }
        else
        {
            isDraw = false;
            Wall.enabled = false;
            
            countDonw = eraseTime;
            test = 0;
        }
        */
    }

    public void DrawMiniMap()
    {
        if(isDraw && !isDrawMiniMap)
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
