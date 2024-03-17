using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    //���� ī�޶� ��������.
    private Camera cameraView;
    public Vector3 mouseDir;

    //������ ������Ʈ ����
    public GameObject SceneView;

    public Animator playerAnime;

    public PlayerStatus playerStatus;

    //�̵����� �����°�
    public Vector3 posOffset;

    //���ͺ� �������� �÷��̾� �����̼ǿ� ī�޶� ���� �ޱ� �ʱ� ���� �÷��̾ �и��Ͽ� ����.
    public Transform player;

    //�÷��̾�� ���콺�� �Ÿ��� ����� ������ ������ �Ǵ��ϴ� �Ÿ�.
    [HideInInspector]public float runRange = 5f;
    //�÷��̾ �޸��� �ȱ�
    public bool isRun = false;

    //������Ʈ���� �Ÿ�
    public static float CheckDistance = 100f;

    //ĳ���� ����
    public PlayerState pState = PlayerState.Idle;

    //���콺 ����Ʈ�� UI���� ����
    public static bool isUI = false;

    //���콺 ����Ʈ�� ��ȣ �ۿ� ������ ������Ʈ���� ����
    public bool isObject = false;

    //2023.04.05 ���� ���ݰ� ������ ������ ���� ������ �����Ƿ� �׼����� ����. ( + ĳ���� �߰� )
    //public bool isAttack = false;
    //public bool isJump = false;

    public static bool isAction = false;
    public static bool isCasting = false;

    private string ActionState = "ActionState";

    public AnimatorOverrideController myOverrideAnim;

    //��ų ����Ʈ ���� ��ġ
    public Transform effectPos;

    /*
    //Ŭ���� �������� �̵� ���� �׽�Ʈ
    public bool isMove = false;
    private Vector3 test;
    */
    // Start is called before the first frame update
    void Start()
    {
        //���� ī�޶�� Carera.main���� ������ �ִ� ���� ī�޶� ������ �� �ִ�.
        cameraView = Camera.main;
        SceneView.SetActive(false);
        //playerAnime = GetComponentInChildren<Animator>();
        //playerStatus = GetComponent<PlayerStatus>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.timeScale == 0)
        {
            return;
        }

        if (!DataManager.instance.isSet)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log(isUI);
        }
        if (playerStatus.isDeath)
        {
            return;
        }

        //���� ���̰ų� �������϶��� �ٸ� ���� ����
        //if (!isAttack && !isJump)
        if(!isAction)
        {
            Vector3 mousePos = Input.mousePosition;
            LookAtMouse(mousePos);
            Move(mouseDir);
            Attack();
            Jump();
        }


        switch (pState)
        {
            case PlayerState.Idle:
                ResetState();
                break;

            case PlayerState.Walk:
                transform.Translate(mouseDir.normalized * (playerStatus.moveSpeed / 2) * Time.deltaTime, Space.World);
                break;

            case PlayerState.Run:
                //�ȴ� ���߿��� �޸���� ���� ��ȯ�� ���������� �޸��� �߿��� �ȴ� ���·� ��ȯ �Ұ�
                isRun = true;
                transform.Translate(mouseDir.normalized * playerStatus.moveSpeed * Time.deltaTime, Space.World);
                break;

            case PlayerState.Attack:
                //���� �� ���� ���� ����
                Vector3 mousePos_a = Input.mousePosition;
                LookAtMouse(mousePos_a);
                break;

            case PlayerState.Jump:
                //�����߿� ���� ���� �Ұ���
                transform.Translate(mouseDir.normalized * playerStatus.moveSpeed * Time.deltaTime, Space.World);
                break;

            case PlayerState.Action:
                
                break;

            case PlayerState.Casting:
                //ĳ���� �� ���� ���� ����
                if (isCasting)
                {
                    Vector3 mousePos_c = Input.mousePosition;
                    LookAtMouse(mousePos_c);
                }
                break;

            case PlayerState.Death:
                break;
        }
        
    }

    public void ResetState()
    {
        isObject = false;
        //isAttack = false;
        isAction = false;
        isRun = false;
    }

    private void Move(Vector3 mouseDir)
    {
        //���콺 ����Ʈ�� UI �϶� �̵� ����
        if (isUI)
        {
            SetState(PlayerState.Idle);
            return;
        }

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

        //��Ŭ���� ������ ���ڸ����� �����.
        if (Input.GetMouseButtonUp(1))
        {
            SetState(PlayerState.Idle);
        }
    }

    private void Attack()
    {
        //���콺 ����Ʈ�� UI�̰ų� item�϶� ���� ����
        if (isUI | isObject)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            //isAttack = true;
            isAction = true;
            SetState(PlayerState.Attack);
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            //isJump = true;
            isAction = true;
            SetState(PlayerState.Jump);
        }
    }

   
    private void LookAtMouse(Vector3 mousePos)
    {
        //��ũ������ ����� ���̸� �׸�
        Ray ray = cameraView.ScreenPointToRay(mousePos);
        RaycastHit hit;
        
        if (Physics.Raycast(ray, out hit))
        {     
            //Vector3 mouseDir;
            //��Ʈ ������ ���콺 �������� ����
            mouseDir = new Vector3(hit.point.x, player.position.y, hit.point.z) - player.position;
            
            //�÷��̾��� �� ������ ���콺 �������� ����
            player.forward = mouseDir;
              
            //hit�� ������Ʈ�� true�̸� �÷��̾���� �Ÿ��� ǥ�����ش�.
            if (CheckTag(hit.transform.tag))
            {
                CheckDistance = mouseDir.magnitude;
            }
            else
            {
                //�׷��� ������ �Ÿ� �ʱ�ȭ
                CheckDistance = 100f;
            }
        }    

    }


    //���콺�� ���� ������Ʈ �Ǻ�
    private bool CheckTag(string hitTag)
    {
        if (hitTag == "Item") return true;
        if (hitTag == "Npc") return true;
        if (hitTag == "Object") return true;
        return false;
    }

    //�÷��̾� ���¸� �޾ƿ� �÷��̾� �ִϸ��̼��� ����
    public void SetState(PlayerState _state)
    {
        //���� ���¸� ���� �� �ִϸ��̼� ������ �ʰ� �������ش�.
        if (pState == _state)
        {
            return;
        }     
        pState = _state;
        playerAnime.SetInteger("pState", (int)pState);

    }

    //��ų ���� ��ų�� �ִϸ��̼� Ŭ���� �޾� ����� ��ų�� �´� �ִϸ��̼� ���
    public void SetAnime(AnimationClip _clip)
    {
        //���� �������� Ŭ���� ���� ���� Ŭ���� �ٸ� Ŭ���� ���� ����
        if (ActionState != _clip.name)
        {
            //ActionState�� Ŭ���� �������ش�.
            myOverrideAnim[ActionState] = _clip;

            //ovrride controller�� �ٲ��� �ʾ��� ���� ����
            if (playerAnime.runtimeAnimatorController != myOverrideAnim)
            {
                playerAnime.runtimeAnimatorController = myOverrideAnim;
            }
        }  
    }

    public void SetAttackAnime(AnimationClip _clip)
    {
        myOverrideAnim["Player_Attack"] = _clip;
      
        if (playerAnime.runtimeAnimatorController != myOverrideAnim)
        {
            playerAnime.runtimeAnimatorController = myOverrideAnim;
        }

    }

    public void SetCastMotion(float _motionSlect)
    {
        playerAnime.SetFloat("MotionSelect", _motionSlect);
    }

    public void SetActionSpeed(float _acSpeed)
    {
        playerAnime.SetFloat("ActionSpeed", _acSpeed);
    }
}

//�÷��̾� ���¸� ��Ÿ���� enum
public enum PlayerState
{
    Idle,               //0
    Walk,
    Run,
    Attack,
    Jump,
    Action,             //5
    Casting,
    Death = 100,

}

//Ŭ���� ������ �̵�
/*
private void clickMove(Vector3 mousePos)
{

    if ((transform.position - mousePos).magnitude < 0.1f)
    {
        isMove = false;
    }
    transform.localPosition -= (transform.localPosition - mousePos).normalized * Time.deltaTime * 5f;

}
*/

//�� �ö󰡱�
/*
if ((hit.point.y - (transform.position.y + posOffset.y)) > 0f && (hit.point.y - (transform.position.y + posOffset.y)) <= 1f)
{
    mouseDir = new Vector3(hit.point.x- player.position.x, hit.point.y - (transform.position.y + posOffset.y), hit.point.z- player.position.z);

}
else
{
    mouseDir = new Vector3(hit.point.x, player.position.y, hit.point.z) - player.position;
}

Ŭ���� ������ �̵��ϱ� ����
if (Input.GetMouseButtonUp(0))
{
    test = hit.point;
    isMove = true;
    Debug.Log(test);
}

if (isMove)
{
    clickMove(test);
}
*/
