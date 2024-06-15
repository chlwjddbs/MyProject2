using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawDoorWall : DrawWalls
{
    public DrawArchWall archWall;
    public bool isDrawDoor { get { return archWall.isDraw; } }

    void Start()
    {
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
        if (isDrawDoor)
        {
            DrawDoor();
            CheckDrawWall();
        }
        else
        {
            EraseWall();
        }
    }


    public void DrawDoor()
    {
        countDonw = eraseTime;

        if (Wall.enabled == false)
        {
            Wall.enabled = true;
            UnderWall.enabled = true;
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
                //player.visibleWalls.Remove(this);
                UnderWall.enabled = false;
                Wall.enabled = false;
                Drawoverwall.SetActive(false);
            }
        }
    }

    public override void DrawMiniMap()
    {
        if (MiniMap == null)
        {
            return;
        }

        if (isDrawDoor && !isDrawMiniMap)
        {
            isDrawMiniMap = true;
            MiniMap.SetActive(true);
        }
    }

    public void CheckDrawWall()
    {
        if (archWall.ChechDoorwallMaterialNum() == 1)
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
