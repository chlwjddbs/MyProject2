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
    

    void Start()
    {
        player = GameObject.Find("ThePlayer").GetComponentInChildren<PlayerSight>();

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
            countDonw = eraseTime;
            CheckDrawWall();
            DrawWall();
        }
        else
        {
            EraseWall();
        }
    }

    private void LateUpdate()
    {
        //EraseWall();
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
        if (Wall.enabled)
        {
            countDonw -= Time.deltaTime;
            if (countDonw <= 0)
            {
                player.visibleWalls.Remove(transform);
                UnderWall.enabled = false;
                isDraw = false;
                Wall.enabled = false;
                Drawoverwall.SetActive(false);              
            }
        }
    }

    //���� �þ߿� ������ �̴ϸʿ� ǥ�� �����ش�.
    public override void DrawMiniMap()
    {
        if (MiniMap == null)
        {
            return;
        }

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

    public void CheckDrawWall()
    {
        Vector3 dir = (player.transform.position + sightOffset - transform.position).normalized;
        float dis = (player.transform.position + sightOffset - transform.position).magnitude;
        
        RaycastHit[] hits = Physics.RaycastAll(transform.position, dir, dis, 1 << 13);

        if (transform.name == "GreenWall135")
        {
            Debug.DrawRay(transform.position, dir * dis, Color.red, 1f);

            for (int i = 0; i < hits.Length; i++)
            {
                Debug.Log(hits[i].transform.GetComponentInParent<DrawWalls>().name);
            }
        }

        if (hits.Length == 0)
        {
            if (wallMaterialNum == 1)
            {
                wallMaterialNum = 0;
                Wall.material = wallMaterials[0]; //������ ��Ʈ���� ����
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
            }
            else
            {
                if (wallMaterialNum == 1)
                {
                    wallMaterialNum = 0;
                    Wall.material = wallMaterials[0]; //������ ��Ʈ���� ����
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
*/