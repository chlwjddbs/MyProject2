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
        //스크린에서 월드로 레이를 그림
        Ray ray = cameraView.ScreenPointToRay(mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            //Vector3 mouseDir;
            //히트 지점을 마우스 방향으로 저장
            mouseDir = new Vector3(hit.point.x, transform.position.y, hit.point.z) - transform.position;

            //플레이어의 앞 방향을 마우스 방향으로 지정
            transform.forward = mouseDir;

            //hit된 오브젝트가 true이면 플레이어와의 거리를 표시해준다.
            if (CheckTag(hit.transform.tag))
            {
                checkDis = mouseDir.magnitude;
            }
            else
            {
                //그렇지 않으면 거리 초기화
                checkDis = Mathf.Infinity;
            }
        }
    }

    public void Move(Vector3 mouseDir)
    {
        //마우스 포인트가 UI 일때 이동 금지
        if (isUI)
        {
            //SetState(PlayerState.Idle);
            return;
        }

        /*
        //마우스 우클릭 유지시
        if (Input.GetMouseButton(1))
        {
            //마우스와 플레이어의 거리가 runRange미만이고 달리지 않고 있을때
            //달리는 도중에는 걷기로 모션 전환 불가
            if (mouseDir.magnitude < runRange && !isRun)
            {
                //걷는다.
                SetState(PlayerState.Walk);
            }

            //마우스와 플레이어의 거리가 runRange이상 일때 달린다.
            else
            {
                SetState(PlayerState.Run);
            }
        }
        */

        //우클릭을 끝낼시 그자리에서 멈춘다.
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
