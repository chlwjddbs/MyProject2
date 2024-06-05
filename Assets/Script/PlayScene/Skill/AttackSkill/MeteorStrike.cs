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

    //���׿� ����� ���� ��Ű�ų� ��� ��Ű�� ������ �ѹ��� ���� �ȴ�. 
    //ó�� ������ castState�� true�� ���ְ� ���ϳ� ��� ���� �� false�� �ٲ� �߰� �������� �ʵ��� ����ڵ带 �����Ѵ�.
    private bool castState;


    protected override void Update()
    {
        base.Update();
        if (!castState)
        {
            return;
        }

        //��ų�� ���Ǿ� ĳ���� ���� ��
        if (player.isCasting)
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
                    FallingMeteor();
                }
            }
        }

        if (Input.GetMouseButtonDown(1) /*| Input.GetKeyDown(KeyCode.Escape)*/)
        {
            CancelMeteor();
        }
    }

    public void LateUpdate()
    {
        if (player.isCasting)
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                player.ChangeState(new IdlePState());

                Cursor.visible = true;
                Destroy(meteorCIrcle.gameObject);
                Destroy(castAura.gameObject);

                castingSound.loop = false;
            }
        }
    }


    public override void UseSkill()
    {
        base.UseSkill();

        if (isUse)
        {
            castState = true;

            player.UseMana(cunsumeMana);
            player.playerAnimeControl.playerAnime.SetFloat("MotionProccess", 0);
            player.ChangeState(new CastPState());
            player.SetCastMotion(castMotionSelect);

            //player.SetActionSpeed(actionSpeed);
            
            isUse = false;
            remainingTime = coolTime;

            Cursor.visible = false;

            startCasting.Play();
            castingSound.Play();
            castingSound.loop = true;

            //��ų ������ ����
            ParticleSystem _meteorCircle = Instantiate(meteorCirclePrefab, player.effectPos);          
            meteorCIrcle = _meteorCircle;

            ParticleSystem _castAura = Instantiate(castAuraPrefab, new Vector3(player.transform.position.x,0.5f, player.transform.position.z), Quaternion.identity);
            castAura = _castAura;
        }
    }

    private void FallingMeteor()
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
                if (player.GetComponentInChildren<PlayerSight>().Dropable(_hitpos))
                {
                    //���׿� ����
                    {
                        castState = false;
                        castingSound.loop = false;
                        player.isAction = true;
                        //����� ĳ���� ��� ����
                        player.playerAnimeControl.playerAnime.SetFloat("MotionProccess", 1);
                        //���� ���̱� ������ ����� ó������ �����Ͽ� �ڿ������� ���� �ο�
                        player.playerAnimeControl.playerAnime.Play("Player_Cast_Tree", -1, 0);

                        //���� Ʈ���� ������� �ʾ��� �� �ִϸ��̼� ���� �ݺ��� �Ѿ�� ����
                        //addEvent�� ��ϵ� ���� ���� �ձ���(0.41f)�� ���� �����ش�.
                        //player.GetComponentInChildren<Animator>().Play("Footman_Action", -1, 0.41f);
                        Cursor.visible = true;
                        Destroy(meteorCIrcle.gameObject);
                        Destroy(castAura.gameObject, 2f);

                        GameObject _meteor = Instantiate(meteorPrefab, new Vector3(_hitpos.x, 0, _hitpos.z), Quaternion.identity);
                        _meteor.GetComponent<Meteor>().SetMeor(new Vector3(_hitpos.x, 0, _hitpos.z));
                        float _skillDamage = player.AttackDamage * skillDamage;
                        _meteor.GetComponent<Meteor>().SetDagage(_skillDamage, player.transform);
                    }
                }
            }
        }
    }

    private void CancelMeteor()
    {
        castState = false;

        if (player.CheckState() != new CastPState().ToString() || player.isAction)
        {
            return;
        }
        player.ChangeState(new IdlePState());

        Cursor.visible = true;
        Destroy(meteorCIrcle.gameObject);
        Destroy(castAura.gameObject);

        castingSound.loop = false;
    }
}


