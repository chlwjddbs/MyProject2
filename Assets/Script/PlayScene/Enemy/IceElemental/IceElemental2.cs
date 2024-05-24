using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceElemental2 : Enemy_FSM
{
    public GameObject iceEffect;

    public GameObject deathEffect;
    public TeleportGate teleportGate;

    [SerializeField]private float iceBoomRange = 5f;
    public GameObject iceBoom;
    public GameObject iceBoomCircle;
    private int iceBoolCount = 3;

    private int MaxGauge = 10;
    private int blastGauge;

    public Teleport teleport;
    public GameObject body;

    public IceBlast iceBlast;
    private Vector3 castingPos;

    public Transform support;

    public void SetAwake()
    {
        SetData();
        isAttackable = true;
        iceEffect.SetActive(true);
        enabled = true;
        StartCoroutine(MutipleBoom());
        castingPos = transform.position;
    }

    public override void SetState()
    {
        base.SetState();
        eStateMachine.RegisterEState(new CastEState());
    }

    public override void LookRotate()
    {
        //공격중에는 방향을 바꾸지 않는다.
        if (eStateMachine.CurrentState.ToString() != new AttackEState().ToString())
        {
            base.LookRotate();
        }
    }

    public override void Die()
    {
        base.Die();
        deathEffect.SetActive(true);
        iceEffect.SetActive(false);
        Destroy(deathEffect, 3f);
        if (teleportGate != null)
        {
            teleportGate.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
            teleportGate.gateCoordinate = teleportGate.transform.position;
            teleportGate.gameObject.SetActive(true);
            teleportGate.OpenGate();
        }
        StopAllCoroutines();
        iceBlast.StopSound();
        iceBlast.StopParticle();
    }

    public override void Damaged(float _damage, Transform _attacker, Vector3 _damagedDir = default)
    {
        base.Damaged(_damage, _attacker, _damagedDir);
        #region 남은 HP에 따라 iceBoom 갯수 추가 하기
        if (remainHealth / maxHealth * 100 > 70)
        {
            iceBoolCount = 3;
        }
        else if(remainHealth / maxHealth * 100 > 40)
        {
            iceBoolCount = 4;
        }
        else if(remainHealth / maxHealth * 100 > 10)
        {
            iceBoolCount = 5;
        }
        else if(remainHealth / maxHealth * 100 > 0)
        {
            iceBoolCount = 6;
        }
        #endregion
        if (eStateMachine.CurrentState.ToString() != new ChaseEState().ToString())
        {
            //캐스팅 도중 맞으면 아이스블라스트의 크기가 줄어든다.
            iceBlast.BlastGimmick();
        }
    }

    IEnumerator MutipleBoom()
    {
        while (!isDeath)
        {
            //Debug.Log($"iceBoom이 {iceBoolCount}번 발동");
            for (int i = 0; i < iceBoolCount; i++)
            {
                StartCoroutine(IceBoom());
            }
            yield return new WaitForSeconds(.8f);

            //PlayEnemySound("olf_iceboom");
            yield return new WaitForSeconds(1.7f);
        }
    }

    IEnumerator IceBoom()
    {
        Vector3 iceBoomPos;
        //icicleBoom을 몬스터를 기준으로 iceBoomRange거리 내에 랜덤으로 뿌려주는 좌표 얻기
        iceBoomPos.x = transform.position.x + (Random.insideUnitCircle.normalized.x * iceBoomRange);
        iceBoomPos.y = 1.5f;
        iceBoomPos.z = transform.position.z + (Random.insideUnitCircle.normalized.y * iceBoomRange);

        GameObject _iceCIrcle = Instantiate(iceBoomCircle, iceBoomPos, Quaternion.identity,support);
        Destroy(_iceCIrcle, 1f);

        yield return new WaitForSeconds(1f);
        //icicleBoom 생성 후 2초 뒤 삭제
        GameObject _iceBoom = Instantiate(iceBoom, iceBoomPos, Quaternion.identity,support);
        Destroy(_iceBoom, 2f);
    }

    public void AddBlastGauge(int _gauge)
    {
        blastGauge += _gauge;
        blastGauge = Mathf.Clamp(blastGauge, 0, MaxGauge);
        Debug.Log(blastGauge);
        if (blastGauge >= MaxGauge)
        {
            ChangeState(new CastEState());
        }
    }

    public override void CastingAction()
    {
        //eStateMachine.ElapsedTime
    }

    public override void CastingStart()
    {
        //(eStateMachine.CurrentState as CastEState).ChangeEAnime(EnemyState.Idle);
        StartCoroutine(IceBlast());
    }

    IEnumerator IceBlast()
    {
        teleport.SetTeleport();

        AttackCollider.enabled = false;

        //iceElemental.PlayEnemySound("olf_telpoin");

        body.SetActive(false);

        yield return new WaitForSeconds(1.25f);

        //iceElemental.PlayEnemySound("olf_telpoout");

        yield return new WaitForSeconds(0.9f);

        transform.position = castingPos;
        body.SetActive(true);
        Debug.Log(eStateMachine.CurrentState.ToString());
        (eStateMachine.CurrentState as CastEState).ChangeEAnime(EnemyState.Casting);

        iceBlast.SetBlast();

        Debug.Log("IceBlast");

        yield return new WaitForSeconds(5f);

        iceBlast.StopParticle();

        yield return new WaitForSeconds(1f);

        iceBlast.StopSound();
        iceBlast.iceBlast.SetActive(true);

        yield return new WaitForSeconds(.5f);

        iceBlast.coll.enabled = true;

        yield return new WaitForSeconds(1f);

        iceBlast.coll.enabled = false;
        blastGauge = 0;
        ChangeState(new IdleEState());

        yield return new WaitForSeconds(2f);
        iceBlast.gameObject.SetActive(false);
    }
}

/*
    protected override void Update()
    {
        if (!isDeath)
        {
            eStateMachine.Update(Time.deltaTime);

            if (eStateMachine.AttackCoolTime <= AttackDelay)
            {
                eStateMachine.AttackTimeCount();
            }

            if (chaseMode)
            {
                SearchChaseTarget();
            }
            else
            {
                if(Target != null)
                {
                    chaseMode = true;
                    SetChaseTarget(Target);
                }
            }
        }
    }
    */