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

    //시야에 나간즉시 없애주지 않고 잔상을 남겨준다.
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
                Wall.material = wallMaterials[0]; //불투명 메트리얼 적용
                
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
                    Wall.material = wallMaterials[1]; //반투명 메트리얼 적용
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
                    Wall.material = wallMaterials[0]; //불투명 메트리얼 적용
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