using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawWall_3 : DrawWalls
{
    /*
    public float eraseTime;
    private float countDonw;

    //0이면 불투명, 1이면 반투명
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
        //벽에 시야에서 벗어났다 다시 들어왔지만 아직 지워지지 않은 상태라면 eraseTime을 리셋 시켜주어야 한다.
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

    //시야에 나간즉시 없애주지 않고 잔상을 남겨준다.
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

    //벽이 시야에 들어오면 미니맵에 표시 시켜준다.
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
                Wall.material = wallMaterials[0]; //불투명 메트리얼 적용
            }
        }
        else
        {
            if (FindMyCheckWall(hits))
            {
                if (wallMaterialNum == 0)
                {
                    wallMaterialNum = 1;
                    Wall.material = wallMaterials[1]; //반투명 메트리얼 적용
                }
            }
            else
            {
                if (wallMaterialNum == 1)
                {
                    wallMaterialNum = 0;
                    Wall.material = wallMaterials[0]; //불투명 메트리얼 적용
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
    //if (isDraw) : isDraw 판정을 하지 않아도 시야에 들어오지 않으면 isHide가 판정이 되지 않아 없어도 무방하다.
    {
        //메테리얼을 직접 비교하면 사용중인 메테리얼에 instance가되어 비교가 되지 않기 때문에 wallMaterialNum을 선언해 현재 메테리얼을 체크해준다.
        if (isHide && wallMaterialNum == 0)
        {
            wallMaterialNum = 1;
            Wall.material = wallMaterials[1]; //반투명 메트리얼 적용
        }
        else if (!isHide && wallMaterialNum == 1)
        {
            wallMaterialNum = 0;
            Wall.material = wallMaterials[0]; //불투명 메트리얼 적용
        }
    }
}
*/