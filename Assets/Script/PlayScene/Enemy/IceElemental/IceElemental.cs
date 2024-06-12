using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.Events;

public class IceElemental : Enemy_FSM
{
    private ObjectPoolingManager poolingManager;
    public UnityAction deathAction;

    [Header("iceElemental Data")]
    public GameObject iceEffect;
    public GameObject deathEffect;

    public GameObject body;
    public Transform support;
    public TeleportGate teleportGate;

    [Header("iceBoom")]
    [SerializeField] private float iceBoomRange = 5f;
    public GameObject iceBoom;
    private IObjectPool<GameObject> iceBoomPool;
    public GameObject iceBoomCircle;
    private IObjectPool<GameObject> iceBoomCirclePool;
    private int iceBoolCount = 3;

    [Header("iceBlast")]
    public IceBlast iceBlast;
    private Vector3 castingPos;
    private int MaxGauge = 100;
    private int blastGauge;
    public Teleport teleport;

    public void SetAwake()
    {
        isAttackable = true;
        iceEffect.SetActive(true);
        enabled = true;
        StartCoroutine("MutipleBoom");
        castingPos = transform.position;
    }

    public override void Die()
    {
        base.Die();

        deathEffect.SetActive(true);
        iceEffect.SetActive(false);
        Destroy(deathEffect, 3f);

        teleportGate.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        teleportGate.gateCoordinate = teleportGate.transform.position;
        teleportGate.gameObject.SetActive(true);
        teleportGate.OpenGate();

        StopAllCoroutines();
        iceBlast.StopSound();
        iceBlast.StopParticle();
        deathAction?.Invoke();
    }

    public override void Damaged()
    {
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

    #region olaf spell : iceBoom, iceBlast
    IEnumerator MutipleBoom()
    {
        var wfs = new WaitForSeconds(.8f);
        var wfs1 = new WaitForSeconds(1.7f);

        while (!isDeath)
        {
            //Debug.Log($"iceBoom이 {iceBoolCount}번 발동");
            for (int i = 0; i < iceBoolCount; i++)
            {
                StartCoroutine(IceBoom());
            }
            yield return wfs;

            PlayESound("olaf_spawniceboom");
            yield return wfs1;
        }
    }

    IEnumerator IceBoom()
    {
        Vector3 iceBoomPos;
        //icicleBoom을 몬스터를 기준으로 iceBoomRange거리 내에 랜덤으로 뿌려주는 좌표 얻기
        iceBoomPos.x = transform.position.x + (Random.insideUnitCircle.normalized.x * iceBoomRange);
        iceBoomPos.y = 1.5f;
        iceBoomPos.z = transform.position.z + (Random.insideUnitCircle.normalized.y * iceBoomRange);

        GameObject _iceCIrcle = iceBoomCirclePool.Get();
        _iceCIrcle.transform.position = iceBoomPos;

        yield return new WaitForSeconds(1f);
        iceBoomCirclePool.Release(_iceCIrcle);
        
        GameObject _iceBoom = iceBoomPool.Get();
        _iceBoom.transform.position = iceBoomPos;

        yield return new WaitForSeconds(2f);
        iceBoomPool.Release(_iceBoom);
    }

    public void AddBlastGauge(int _gauge)
    {
        blastGauge += _gauge;
        blastGauge = Mathf.Clamp(blastGauge, 0, MaxGauge);
        Debug.Log(blastGauge);
        if (blastGauge >= MaxGauge)
        {
            isCasting = true;
            ChangeState(new CastEState());
        }
    }

    IEnumerator IceBlast()
    {
        teleport.SetTeleport();

        AttackCollider.enabled = false;

        PlayESound("olaf_telpoin");

        body.SetActive(false);

        yield return new WaitForSeconds(1.25f);

        PlayESound("olaf_telpoout");

        yield return new WaitForSeconds(0.9f);

        transform.position = castingPos;
        body.SetActive(true);

        (eStateMachine.CurrentState as CastEState).ChangeEAnime(EnemyState.Casting);

        iceBlast.SetBlast();

        yield return new WaitForSeconds(5f);

        iceBlast.StopParticle();

        yield return new WaitForSeconds(1f);

        iceBlast.StopSound();
        iceBlast.iceBlast.SetActive(true);

        yield return new WaitForSeconds(.5f);

        iceBlast.coll.enabled = true;
        (eStateMachine.CurrentState as CastEState).ChangeEAnime(EnemyState.Idle);
        yield return new WaitForSeconds(1f);

        iceBlast.coll.enabled = false;
        blastGauge = 0;
        ChangeState(new IdleEState());

        yield return new WaitForSeconds(2f);
        iceBlast.gameObject.SetActive(false);
    }
    #endregion

    #region enemy actcion
    public override void LookRotate()
    {
        //공격중에는 방향을 바꾸지 않는다.
        if (eStateMachine.CurrentState.ToString() != new AttackEState().ToString())
        {
            base.LookRotate();
        }
    }
    public override void CastingAction()
    {
        //eStateMachine.ElapsedTime
    }
    public override void CastingStart()
    {
        (eStateMachine.CurrentState as CastEState).ChangeEAnime(EnemyState.Idle);
        StartCoroutine(IceBlast());
    }
    #endregion

    #region Data Manager : set, save, load, pool
    public override void LoadData(EnemyData _enemyData)
    {
        base.LoadData(_enemyData);
        if (isDeath)
        {
            deathAction?.Invoke();
        }
    }
    public override void SetData()
    {
        base.SetData();
    }
    public override void SetState()
    {
        base.SetState();
        eStateMachine.RegisterEState(new CastEState());
    }
    public override void SetPool()
    {
        poolingManager = ObjectPoolingManager.instance;
        poolingManager.RegisetPoolObj(iceBoom, new ObjectPool<GameObject>(CreateIceBoomPool, poolingManager.OnGet, poolingManager.OnRelease, poolingManager.OnDes));
        poolingManager.RegisetPoolObj(iceBoomCircle, new ObjectPool<GameObject>(CreateIceBoomCirclePool, poolingManager.OnGet, poolingManager.OnRelease, poolingManager.OnDes));
        iceBoomPool = poolingManager.FindPool(iceBoom.name);
        iceBoomCirclePool = poolingManager.FindPool(iceBoomCircle.name);

        for (int i = 0; i < 12; i++)
        {
            GameObject _iceBoom = CreateIceBoomPool();
            iceBoomPool.Release(_iceBoom);
            GameObject _iceBoomCircle = CreateIceBoomCirclePool();
            iceBoomCirclePool.Release(_iceBoomCircle);
        }
    }
    public GameObject CreateIceBoomPool()
    {
        GameObject _iceboom = Instantiate(iceBoom, support);
        if (_iceboom.TryGetComponent<IceBoom>(out IceBoom boom))
        {
            boom.SetData(this);
        }
        return _iceboom;
    }

    public GameObject CreateIceBoomCirclePool()
    {
        GameObject _iceBoomCircle = Instantiate(iceBoomCircle, support);
        return _iceBoomCircle;
    }
    #endregion

    protected override void OnCollisionEnter(Collision collision)
    {
        //Player와 충돌을 감지한 collider가 
        if (collision.transform.CompareTag("Player"))
        {
            //attackCollider이면 데미지를 준다.
            if ((collision.contacts[0].thisCollider == attackCollider))
            {
                //공격 한번이 끝나기 전에 중복으로 데미지를 주는것을 방지.
                //공격 한번이 끝나기 전에 연속으로 충돌이 이루어져 한번에 공격에 두번의 충돌이 이루어 진다면
                //처음 충돌에 대상을 저장해놓고, 다시 충돌한 대상이 있으면 연속으로 충돌한 것으로 판명해 넘어간다.
                if (!attackedTargets.Contains(collision.gameObject))
                {
                    //처음 중돌한 대상은 저장하고 데미지를 준다.
                    attackedTargets.Add(collision.gameObject);
                    if (collision.transform.TryGetComponent<IAttackable>(out IAttackable value))
                    {
                        value.TakeDamage(attackDamage,null);
                        PlayESound("olaf_hit");
                    }
                }
            }
        }
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