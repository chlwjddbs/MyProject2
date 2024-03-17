using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawWall : MonoBehaviour
{
    //���°� �ٲ� (�θ�)
    public GameObject Wall;
    private MeshRenderer wallMesh;


    //������, ������ �� ��Ʈ���� 
    public List<Material> wallMaterials;
    // Start is called before the first frame update

    public bool isHide = false;
    public bool isPassHide = false;
    
    void Start()
    {
        wallMesh = Wall.GetComponent<MeshRenderer>();
        
    }

    // Update is called once per frame
    void Update()
    {
        ChangeWall();
       
    }

    public void ChangeWall()
    {
        if (isHide & !isPassHide)
        {
            wallMesh.material = wallMaterials[1]; //������ ��Ʈ���� ����
        }
        else
        {
            wallMesh.material = wallMaterials[0]; //������ ��Ʈ���� ����
        }
    }

   

    /*
    private void OnTriggerEnter(Collider other)
    {
        changeState.material = wallMaterials[1];
    }

    private void OnTriggerExit(Collider other)
    {
        changeState.material = wallMaterials[0];
    }
    */
}
