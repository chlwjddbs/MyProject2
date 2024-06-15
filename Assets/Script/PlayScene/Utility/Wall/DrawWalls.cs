using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawWalls : MonoBehaviour
{
    //������, ������ �� ��Ʈ���� 
    public List<Material> wallMaterials;

    public MeshRenderer Wall;
    public MeshRenderer UnderWall;

    //0�̸� ������, 1�̸� ������
    protected int wallMaterialNum;
    public GameObject Drawoverwall;
    public GameObject MiniMap;
    protected bool isDrawMiniMap = false;

    public bool isHide = false;
    public bool isDraw = false;

    public float eraseTime = 0.1f;
    protected float countDonw;

    public bool isSight;

    public virtual void DrawWall() { }
    public virtual void DrawMiniMap() 
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
}
