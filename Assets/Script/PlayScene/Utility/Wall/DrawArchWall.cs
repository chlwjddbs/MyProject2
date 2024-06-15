using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawArchWall : DrawWalls
{
    
    public MeshRenderer archWall;
    public MeshRenderer underWall2;

    private Vector3 sightOffset = new Vector3(0, 2.5f, 0);
    private PlayerSight player;

    private Vector3 playerDir;
    private float playerDis;

    public Transform checkWall;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("ThePlayer").GetComponentInChildren<PlayerSight>();
        //eraseTime = 0.1f;
        archWall.enabled = false;
        UnderWall.enabled = false;
        underWall2.enabled = false;
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
            archWall.enabled = true;
            UnderWall.enabled = true;
            underWall2.enabled = true;
        }

        DrawMiniMap();
    }

    public void EraseWall()
    {
        if (Wall.enabled)
        {
            countDonw -= Time.deltaTime;

            if (countDonw <= 0)
            {
                Wall.enabled = false;
                archWall.enabled = false;
                UnderWall.enabled = false;
                underWall2.enabled = false;
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
            if (_hits[i].transform == checkWall)
            {
                return true;
            }
        }
        return false;
    }

    public int ChechDoorwallMaterialNum()
    {
        return wallMaterialNum;
    }
}
/*
 *  public override void DrawWall()
    {
        if (isDraw && countDonw < eraseTime)
        {
            countDonw = eraseTime;
            UnderWall.enabled = true;
        }
        if (isDraw && Wall.enabled == false)
        {
            countDonw = eraseTime;
            Wall.enabled = true;
            archWall.enabled = true;
            UnderWall.enabled = true;
        }
        else if (!isDraw && Wall.enabled == true)
        {
            EraseWall();
        }
    }
*/