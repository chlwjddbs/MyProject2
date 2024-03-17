using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorStrike : SkillManager
{
    //public bool isCasting;
    //ĳ���ý� �������� ����
    public ParticleSystem castAuraPrefab;
    public GameObject meteorPrefab;
    //��ų�� ������ ��ġ�� ǥ�����ִ� ������
    public ParticleSystem meteorCirclePrefab;

    //��ų ���ݷ�
    public float skillDamage;

    //ĳ���� ��� ���� Ʈ���ȿ��� ������ ���Ʈ�� ��ȣ
    private float castMotionSelect;

    //���� ������ ĳ���� ����
    private ParticleSystem castAura;

    //���� ������ ��ų ������
    private ParticleSystem meteorCIrcle;
    //���콺 ��ġ�� ���� ����� ��ų ������ ��ǥ
    private Vector3 circlePos;

    public AudioSource castingSound;
    public AudioSource startCasting;


    private void Update()
    {
        if (!isUse)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0)
            {
                isUse = true;
            }
        }

        //��ų�� ���Ǿ� ĳ���� ���� ��
        if (PlayerController.isCasting)
        {
            RaycastHit hit;
            Vector3 hitpos;

            //���콺��ġ���� ����ĳ��Ʈ�� ���� ������ ����Ʈ�� ǥ�ý����ش�.
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1))
            {
                hitpos = hit.point;
                if (meteorCIrcle != null)
                {
                    circlePos = hitpos;
                    circlePos.y += 0.5f;
                    meteorCIrcle.transform.position = circlePos;
                }
                //���콺 ��Ŭ����
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit _hit;
                    Vector3 _hitpos;

                    //�ش� ��ġ�� ������Ʈ�� �Ǻ��Ͽ�
                    if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _hit, Mathf.Infinity))
                    {
                        //��Ʈ�� ������Ʈ�� water, obstacle , object�� �ƴϰ�
                        if (/*!_hit.transform.CompareTag("Water") &&*/ !_hit.transform.CompareTag("Obstacle") && !_hit.transform.CompareTag("Object"))
                        {
                            //�þ߰� Ȯ���� �����̸�
                            _hitpos = _hit.point;
                            if (player.GetComponentInChildren<PlayerSight>().Dropable(hitpos))
                            {
                                //���׿� ����
                                {
                                    castingSound.loop = false;

                                    PlayerController.isCasting = false;
                                    //����� ĳ���� ��� ����
                                    player.GetComponentInChildren<Animator>().SetFloat("MotionProccess", 1);
                                    //���� ���̱� ������ ����� ó������ �����Ͽ� �ڿ������� ���� �ο�
                                    player.GetComponentInChildren<Animator>().Play("Player_Cast_Tree", -1, 0);

                                    //���� Ʈ���� ������� �ʾ��� �� �ִϸ��̼� ���� �ݺ��� �Ѿ�� ����
                                    //addEvent�� ��ϵ� ���� ���� �ձ���(0.41f)�� ���� �����ش�.
                                    //player.GetComponentInChildren<Animator>().Play("Footman_Action", -1, 0.41f);
                                    Cursor.visible = true;
                                    Destroy(meteorCIrcle.gameObject);
                                    Destroy(castAura.gameObject, 2f);

                                    GameObject _meteor = Instantiate(meteorPrefab, new Vector3(hitpos.x, 0, hitpos.z), Quaternion.identity);
                                    _meteor.GetComponent<Meteor>().SetMeor(new Vector3(hitpos.x, 0, hitpos.z));
                                    float _skillDamage = player.playerStatus.currentDamage * skillDamage;
                                    _meteor.GetComponent<Meteor>().SetDagage(_skillDamage);
                                }
                            }
                        }
                    }
                }
            }

            if (Input.GetMouseButtonDown(1) /*| Input.GetKeyDown(KeyCode.Escape)*/)
            {
                PlayerController.isCasting = false;
                player.SetState(PlayerState.Idle);
                player.GetComponentInChildren<Animator>().SetFloat("MotionProccess", 0);

                Cursor.visible = true;
                Destroy(meteorCIrcle.gameObject);
                Destroy(castAura.gameObject);

                castingSound.loop = false;
            }
        }
    }

    public void LateUpdate()
    {
        if (PlayerController.isCasting)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                PlayerController.isCasting = false;
                player.SetState(PlayerState.Idle);
                player.GetComponentInChildren<Animator>().SetFloat("MotionProccess", 0);

                Cursor.visible = true;
                Destroy(meteorCIrcle.gameObject);
                Destroy(castAura.gameObject);

                castingSound.loop = false;
            }
        }
    }


    public override void UseSkill(PlayerController _player)
    {
        if (isUse)
        {
            player = _player;
            player.GetComponent<PlayerStatus>().UseMana(cunsumeMana);
            player.GetComponentInChildren<Weapon>().SetSkillDage(skillDamage,true);
            player.SetState(PlayerState.Casting);
            player.SetCastMotion(castMotionSelect);
            /*
            player.SetState(PlayerState.Action);
            player.SetAnime(skillMotion);
            player.SetActionSpeed(actionSpeed);
            */

            PlayerController.isAction = true;

            isUse = false;
            PlayerController.isCasting = true;
            remainingTime = coolTime;

            Cursor.visible = false;

            startCasting.Play();
            castingSound.Play();
            castingSound.loop = true;

            //��ų ������ ����
            ParticleSystem _meteorCircle = Instantiate(meteorCirclePrefab, _player.effectPos);          
            meteorCIrcle = _meteorCircle;

            ParticleSystem _castAura = Instantiate(castAuraPrefab, new Vector3(_player.transform.position.x,0.5f,_player.transform.position.z), Quaternion.identity);
            castAura = _castAura;
        }
    }
}


