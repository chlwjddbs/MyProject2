using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class FireElemental : Enemy_FSM
{
    private bool isPase2nd;
    public GameObject pase2Effect;
    public ParticleSystem FireAura;
    public GameObject DeathEffect;

    public Transform burnPos;
    public GameObject burnPrefab;

    public Transform burnEffectPos;
    public GameObject burnCirclePrefab;
    public GameObject burnEffectPrefab;

    private int burnCount; 
    private int startburnStack = 3;

    [HideInInspector]public ObjectPoolingManager poolingManager;
    public IObjectPool<GameObject> circlePool;
    public IObjectPool<GameObject> burnPool;
    public IObjectPool<GameObject> burnEffectPool;

    public AudioClip test1;

    public override void SetState()
    {
        eStateMachine = new EnemyStateMachine(this, new IdleEState());
        eStateMachine.RegisterEState(new AttackEState());
        eStateMachine.RegisterEState(new CastEState());
        eStateMachine.RegisterEState(new DeathEState());
    }
    public override void SetPool()
    {
        poolingManager = ObjectPoolingManager.instance;
        poolingManager.RegisetPoolObj(burnCirclePrefab, new ObjectPool<GameObject>(CreateCirclePool, poolingManager.OnGet, poolingManager.OnRelease, poolingManager.OnDes, maxSize: 10));
        poolingManager.RegisetPoolObj(burnPrefab, new ObjectPool<GameObject>(CreateBurnPool, poolingManager.OnGet, poolingManager.OnRelease, poolingManager.OnDes, maxSize: 10));
        poolingManager.RegisetPoolObj(burnEffectPrefab, new ObjectPool<GameObject>(CreateBurnEffectPool, poolingManager.OnGet, poolingManager.OnRelease, poolingManager.OnDes, maxSize: 10));

        circlePool = poolingManager.FindPool(burnCirclePrefab.name);
        burnPool = poolingManager.FindPool(burnPrefab.name);
        burnEffectPool = poolingManager.FindPool(burnEffectPrefab.name);

        for (int i = 0; i < 6; i++)
        {
            GameObject burnCircle = CreateCirclePool();
            circlePool.Release(burnCircle);
        }

        for (int i = 0; i < 9; i++)
        {
            GameObject burn = CreateBurnPool();
            burnPool.Release(burn);
        }

        for (int i = 0; i < 3; i++)
        {
            GameObject burnEffect = CreateBurnEffectPool();
            burnEffectPool.Release(burnEffect);
        }
    }
    public GameObject CreateCirclePool()
    {
        GameObject burnCircle = Instantiate(burnCirclePrefab, burnEffectPos);
        return burnCircle;
    }
    public GameObject CreateBurnPool()
    {
        GameObject burnCircle = Instantiate(burnPrefab, burnPos);
        burnCircle.GetComponent<RotateAroundBurn>().SetBurn(this);
        return burnCircle;
    }

    public GameObject CreateBurnEffectPool()
    {
        GameObject burnEffect = Instantiate(burnEffectPrefab, burnEffectPos);
        return burnEffect;
    }

    public override void TakeDamage(float _damage, Transform _attacker, Vector3 _damagedDir = new Vector3())
    {
        if (isDeath)
        {
            return;
        }

        if (!isAttackable)
        {
            return;
        }

        if (_damage > 0)
        {
            remainHealth -= _damage;
            Damaged();
            Debug.Log(remainHealth / maxHealth * 100 + " %");
            PlayESound(damagedSound);
        }

        if (RemainHealth <= 0)
        {
            Die();
        }
    }
    public override void Damaged()
    {
        if (remainHealth <= maxHealth / 2 && !isPase2nd)
        {
            PlayESound("page2");
            StartCoroutine("ChangePase");
        }
    }
    IEnumerator ChangePase()
    {
        isCasting = true;
        isPase2nd = true;

        pase2Effect.SetActive(true);
        StartCoroutine(RotateBurn());

        ChangeState(new CastEState());
        attackDelay = 0;

        yield return new WaitForSeconds(4f);

        isCasting = false;
        ChangeState(new IdleEState());
    }

    public override void Die()
    {
        base.Die();
        StartCoroutine(FireElementalDie());
    }
    IEnumerator FireElementalDie()
    {
        isDeath = true;
        ChangeState(new DeathEState());
        FireAura.Stop();
        AudioManager.instance.StopBGM(1f);
        PlayESound(deathSound);

        yield return new WaitForSeconds(0.3f);

        DeathEffect.SetActive(true);
        PlayESound("gn_explosion");

        yield return new WaitForSeconds(1.3f);
        gameObject.SetActive(false);
        AudioManager.instance.PlayBGM("PlayScene_Floor_1", 1f);
    }

    public override void LoadData(Enemy_FSM.EnemyData _enemyData)
    {
        enemyData = _enemyData;

        eStateMachine.LoadData(enemyData.currentState, enemyData.attackCoolTime);

        transform.rotation = enemyData.enemyQuat;

        if (enemyData.isDeath)
        {
            isDeath = enemyData.isDeath;
            searchPlayer.StopSearch();
            enemyMark.SetActive(false);
            remainHealth = 0;
            hitBox.enabled = false;
            attackCollider.enabled = false;          
            FireAura.Stop();
            gameObject.SetActive(false);
            return;
        }

        isAttackable = enemyData.isAttackable;        
        remainHealth = enemyData.remainHealth;
        previousDamage = enemyData.previousDamage;
    }

    public void BurnExplosion()
    {
        StartCoroutine("SpawnBurn");
    }
    IEnumerator SpawnBurn()
    {
        //공격시 Burn이 소환 될 위치를 미리 알려주는 마법진 생성
        //소환 될 위치 : 플레이어의 위치
        Vector3 burnPoint = new Vector3(Target.transform.position.x, 1f, Target.transform.position.z);
        GameObject _burnCircle = circlePool.Get();
        _burnCircle.transform.position = burnPoint;

        yield return new WaitForSeconds(1f);

        circlePool.Release(_burnCircle);

        GameObject _burn = burnPool.Get();
        if(_burn.TryGetComponent<RotateAroundBurn>(out RotateAroundBurn value))
        {
            value.transform.position = burnPoint;

            //페이즈 1
            if (!isPase2nd)
            {
                value.attackDamage = attackDamage;
            }
            //페이즈 2
            else
            {
                //소횐되는 burn의 속도가 1.5배 증가
                value.turnSpeed *= 1.5f;
                value.attackDamage =attackDamage * 1.5f;
            }
        }

        //burn의 스텍 카운트 증가
        burnCount++;
        if(burnCount == startburnStack)
        {
            StartCoroutine(RotateBurn());
        }
    }

    IEnumerator RotateBurn()
    {
        yield return new WaitForSeconds(.5f);

        //burnPos.GetComponentsInChildren<RotateAroundBurn>();

        foreach (RotateAroundBurn _burn in burnPos.GetComponentsInChildren<RotateAroundBurn>())
        {
            if (_burn.gameObject.activeSelf)
            {
                _burn.isTurn = true;
            }
        }

        burnCount = 0;
    }
}
