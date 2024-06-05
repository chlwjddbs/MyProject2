using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeteorStrike : SkillManager
{
    //public bool isCasting;
    //캐스팅시 보여지는 오라
    public ParticleSystem castAuraPrefab;
    public GameObject meteorPrefab;
    //스킬이 시전될 위치를 표시해주는 마법진
    public ParticleSystem meteorCirclePrefab;

    //스킬 공격력
    public float skillDamage;

    //캐스팅 모션 블랜드 트리안에서 선택한 모션트리 번호
    private float castMotionSelect;

    //현재 생성된 캐스팅 오라
    private ParticleSystem castAura;

    //현재 생성된 스킬 마법진
    private ParticleSystem meteorCIrcle;
    //마우스 위치에 따라 변경될 스킬 마법진 좌표
    private Vector3 circlePos;

    public AudioSource castingSound;
    public AudioSource startCasting;

    //메테오 사용후 낙하 시키거나 취소 시키는 동작은 한번만 들어가야 된다. 
    //처음 시전시 castState를 true로 해주고 낙하나 취소 동작 후 false로 바꿔 추가 동작하지 않도록 방어코드를 설계한다.
    private bool castState;


    protected override void Update()
    {
        base.Update();
        if (!castState)
        {
            return;
        }

        //스킬이 사용되어 캐스팅 중일 때
        if (player.isCasting)
        {
            RaycastHit hit;
            Vector3 hitpos;

            //마우스위치에서 레이캐스트를 쏴서 마법진 이펙트를 표시시켜준다.
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, 1))
            {
                hitpos = hit.point;
                if (meteorCIrcle != null)
                {
                    circlePos = hitpos;
                    circlePos.y += 0.5f;
                    meteorCIrcle.transform.position = circlePos;
                }
                //마우스 좌클릭시
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

            //스킬 마법진 생성
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

        //해당 위치의 오브젝트를 판별하여
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out _hit, Mathf.Infinity))
        {
            //히트된 오브젝트가 water, obstacle , object가 아니고
            if (/*!_hit.transform.CompareTag("Water") &&*/ !_hit.transform.CompareTag("Obstacle") && !_hit.transform.CompareTag("Object"))
            {
                //시야가 확보된 공간이면
                _hitpos = _hit.point;
                if (player.GetComponentInChildren<PlayerSight>().Dropable(_hitpos))
                {
                    //메테오 시전
                    {
                        castState = false;
                        castingSound.loop = false;
                        player.isAction = true;
                        //블랜드된 캐스팅 모션 실행
                        player.playerAnimeControl.playerAnime.SetFloat("MotionProccess", 1);
                        //블랜드 중이기 때문에 모션을 처음부터 실행하여 자연스러운 연출 부여
                        player.playerAnimeControl.playerAnime.Play("Player_Cast_Tree", -1, 0);

                        //블랜드 트리를 사용하지 않았을 때 애니메이션 구간 반복을 넘어가기 위해
                        //addEvent가 등록된 구간 보다 앞구간(0.41f)을 실행 시켜준다.
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


