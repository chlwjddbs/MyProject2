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
    private PlayerSight player;
    private Vector3 sightOffset = new Vector3(0,2.5f,0);
    public Transform checkWall;

    private Vector3 playerDir;
    private float playerDis;

    private float time;


    void Start()
    {
        player = GameObject.Find("ThePlayer").GetComponentInChildren<PlayerSight>();
        //eraseTime = 0.1f;
        UnderWall.enabled = false;
        isDraw = false;
        Wall.enabled = false;
        wallMaterialNum = 1;
        Wall.material = wallMaterials[1];
        Drawoverwall.SetActive(false);

        if (MiniMap != null)
        {
            MiniMap.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (isDraw)
        {
            CheckDrawWall();
        }
        else
        {
            EraseWall();
        }
    }

    public override void DrawWall()
    {
        isDraw = true;
        countDonw = eraseTime;

        if (Wall.enabled == false)
        {   
            Wall.enabled = true;
            UnderWall.enabled = true;
        }

        DrawMiniMap();
    }

    //�þ߿� ������� �������� �ʰ� �ܻ��� �����ش�.
    public void EraseWall()
    {
        if (Wall.enabled)
        {
            countDonw -= Time.deltaTime;

            if (countDonw <= 0)
            {
                //player.visibleWalls.Remove(this);
                UnderWall.enabled = false;
                Wall.enabled = false;
                Drawoverwall.SetActive(false);
            }
        }
    }

    public void CheckDrawWall()
    {
        playerDir = (player.transform.position + sightOffset - transform.position).normalized;
        playerDis = (player.transform.position + sightOffset - transform.position).magnitude;
        
        RaycastHit[] hits = Physics.RaycastAll(transform.position, playerDir, playerDis, 1 << 13);
        
        if (hits.Length == 0)
        {
            if (wallMaterialNum == 1)
            {
                wallMaterialNum = 0;
                Wall.material = wallMaterials[0]; //������ ��Ʈ���� ����
                
            }

            if (!Drawoverwall.activeSelf)
            {
                Drawoverwall.SetActive(true);
            }
        }
        else
        {
            if (FindMyCheckWall(hits))
            {
                if (wallMaterialNum == 0)
                {
                    wallMaterialNum = 1;
                    Wall.material = wallMaterials[1]; //������ ��Ʈ���� ����
                }

                if (Drawoverwall.activeSelf)
                {
                    Drawoverwall.SetActive(false);
                }
            }
            else
            {
                if (wallMaterialNum == 1)
                {
                    wallMaterialNum = 0;
                    Wall.material = wallMaterials[0]; //������ ��Ʈ���� ����
                }

                if (!Drawoverwall.activeSelf)
                {
                    Drawoverwall.SetActive(true);
                }
            }
        }
    }

    public bool FindMyCheckWall(RaycastHit[] _hits)
    {
        for (int i = 0; i < _hits.Length; i++)
        {
            if(_hits[i].transform == checkWall)
            {
                return true;
            }
        }
        return false;
    }
}

/*
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
*/