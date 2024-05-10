using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavAgentController : MonoBehaviour
{
    private NavMeshAgent agent;

    [SerializeField]
    private Vector3 worldPosition;

    public LayerMask mask;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        //���콺 Ŭ��
        if(Input.GetMouseButtonDown(0))
        {
            if(RayToWorldPosition() == true)
            {
                //agent�� Ŭ�� ����(worldPosition)���� �̵�
                agent.SetDestination(worldPosition);
            }
        }        
    }    

    bool RayToWorldPosition()
    {
        worldPosition = Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit))
        {
            Debug.Log("mask click");
            worldPosition = hit.point;
            return true;
        }

        return false;
    }
}
