using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerStateMachine stateMachine;

    private Camera cameraView;
    private Animator playerAnime;

    public static bool isDeath = false;
    public static bool isAction = false;
    public static bool isCasting = false;
    public static bool isUI = false;

    private Vector3 mouseDir;
    private float checkDis;

    // Start is called before the first frame update
    void Start()
    {
        cameraView = Camera.main;
        playerAnime = GetComponent<Animator>();
        stateMachine = new PlayerStateMachine(this, new IdleState());
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        if (!GameData.instance.isSet)
        {
            return;
        }

        //if (playerStatus.isDeath)
        if (isDeath)
        {
            return;
        }
    }

    public void LookAtMouse(Vector3 mousePos)
    {
        //��ũ������ ����� ���̸� �׸�
        Ray ray = cameraView.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            //Vector3 mouseDir;
            //��Ʈ ������ ���콺 �������� ����
            mouseDir = new Vector3(hit.point.x, transform.position.y, hit.point.z) - transform.position;

            //�÷��̾��� �� ������ ���콺 �������� ����
            transform.forward = mouseDir;

            //hit�� ������Ʈ�� true�̸� �÷��̾���� �Ÿ��� ǥ�����ش�.
            if (CheckTag(hit.transform.tag))
            {
                checkDis = mouseDir.magnitude;
            }
            else
            {
                //�׷��� ������ �Ÿ� �ʱ�ȭ
                checkDis = Mathf.Infinity;
            }
        }
    }

    public void Move(Vector3 mouseDir)
    {
        //���콺 ����Ʈ�� UI �϶� �̵� ����
        if (isUI)
        {
            //SetState(PlayerState.Idle);
            return;
        }

        /*
        //���콺 ��Ŭ�� ������
        if (Input.GetMouseButton(1))
        {
            //���콺�� �÷��̾��� �Ÿ��� runRange�̸��̰� �޸��� �ʰ� ������
            //�޸��� ���߿��� �ȱ�� ��� ��ȯ �Ұ�
            if (mouseDir.magnitude < runRange && !isRun)
            {
                //�ȴ´�.
                SetState(PlayerState.Walk);
            }

            //���콺�� �÷��̾��� �Ÿ��� runRange�̻� �϶� �޸���.
            else
            {
                SetState(PlayerState.Run);
            }
        }
        */

        //��Ŭ���� ������ ���ڸ����� �����.
        if (Input.GetMouseButtonUp(1))
        {
            //SetState(PlayerState.Idle);
        }
    }

    private bool CheckTag(string hitTag)
    {
        if (hitTag == "Item") return true;
        if (hitTag == "Npc") return true;
        if (hitTag == "Object") return true;
        return false;
    }
}
