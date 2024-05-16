using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_FSM : MonoBehaviour, EnemyStatusDataInterface, Damageable
{
    protected EnemyStateMachine eStateMachine;
    protected Animator eAnime;

    [HideInInspector] public SearchPlayer searchPlayer;

    #region Target정보
    public Transform Target { get { return searchPlayer.Target; } }

    public float TargetDis { get { return searchPlayer.TargetDis; } }
    public Vector3 TargetDir { get { return searchPlayer.TargetDir; } }
    #endregion   
    #region Enemy 범위 정보
    protected float attackRange = 3.5f;
    public float AttackRange { get { return attackRange; } }

    public float ActionRange { get { return searchPlayer.ActionRange; } } //15f;

    public float DetectRange { get { return searchPlayer.DetectRange; } } //30f;
    #endregion
    #region Enemy Spect Data
    [SerializeField]protected float startHealth;
    public float StartHealth { get { return startHealth; } }

    protected float maxHealth;
    public float MaxHealth { get { return maxHealth; } }

    [SerializeField] protected float currentHealth;
    public float CurrentHealth { get { return currentHealth; } }

    [SerializeField] protected float attackDamage;
    public float AttackDamage { get { return attackDamage; } }

    [SerializeField] protected float defencePoint;
    public float DefencePoint { get { return defencePoint; } }

    [SerializeField] int exp;
    public int Exp { get { return exp; } }
    
    //Enemy가 Target을 향해 회전하는 속도
    protected float rotateSpeed = 7f;
    #endregion
    #region Enmey 상태
    [SerializeField] private bool isDamageable = true;
    public bool IsDamageable { get { return isDamageable; } }

    public bool VisibleTarget { get { return searchPlayer.visibelTarget; } }
    public bool chaseMode;
    public bool returnHome;
    public bool isDeath;

    public bool patrolUnit;
    #endregion
 
    public Vector3 StartPoint { get; set; }
    public Transform[] PatrolPoint;

    public GameObject enemyMark;

    public CapsuleCollider hitBox;

    public Collider attackCollider;
    private float previousDamage;
    public List<GameObject> attackedTargets = new List<GameObject>();

    protected virtual void Start()
    {
        SetInitData();
        SetData();
    }

    protected virtual void Update()
    {
        if (!isDeath)
        {
            eStateMachine.Update();
        }
    }

    public Animator CallEnemyAnime()
    {
        return eAnime;
    }

    public virtual State ChangeState(State newState)
    {
        return eStateMachine.ChangeState(newState);
    }

    public void EndAttack()
    {

    }

    public void PlayEnemySound()
    {

    }

    public void LookRotate()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(TargetDir), Time.deltaTime * rotateSpeed);
    }

    public virtual void SaveData()
    {
        
    }

    public virtual void LoadData()
    {
       
    }

    //_attacker는 enemy를 공격한 대상이다. 현재는 enemy를 공격할 대상이 Player 밖에 없지만 추후 대상을 확장 됐을때를 대비해 미리 만들어둔다.
    public virtual void TakeDamage(float _damage , Transform _attacker, Vector3 _damagedDir = new Vector3())
    {
        //죽은 상태에서는 더 이상 데미지를 받지 않는다.
        if (isDeath)
        {
            return;
        }

        if (_damage > 0)
        {
            currentHealth -= _damage;
            //Dagamed();
            Debug.Log(currentHealth / maxHealth * 100 + " %");
            //PlayEnemySound(damagedSound);

            //Target이 null이 아니라는 것은 enemy의 DetectRange안에서 Target이 공격 한 상태이다.
            if (Target != null)
            {
                //추후 Target이 Player 외에 더 생길 수 있다.
                //그때 enemy가 공격 받을때마다 목표로 하는 Target을 바꾸지 않도록 가장 강한 공격을 한 Target만을 쫓도록 해준다.
                if (Target == _attacker)
                {
                    if (previousDamage < _damage)
                    {
                        previousDamage = _damage;
                        SetChaseTarget(_attacker);
                    }
                }
                //enemy를 공격한 대상을 찾지 못했다면 주변의 다른 가까운 Target을 찾아 쫓아간다.
                else
                {
                    SetChaseTarget(Target);
                }
            }
            //enemy의 감지범위 내에 Target이 존재하지 않으면 enemy는 도망가는 상태가 된다.
            else
            {
                ChangeState(new RunawayEState());
                (eStateMachine.states[new RunawayEState().GetType()] as RunawayEState).SetDamagedDir(_damagedDir);
            }
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public void SetChaseTarget(Transform _attacker)
    {
        chaseMode = true;
        //추적 모드에서는 Target이 DetectRange밖으로 나가도 일정 시간 이상 쫓아가게 된다.
        //Target이 DetectRange 밖으로 나가면 Target을 잃게 되는데 목표를 잃지 않게 Target을 따로 저장해준다.
        (eStateMachine.states[new ChaseEState().GetType()] as ChaseEState).SetTarget(_attacker);

        returnHome = false;
    }

    public void ResetChaseTarget()
    {
        chaseMode = false;
        previousDamage = 0;
    }

    public virtual void Die()
    {
        isDeath = true;
        eStateMachine.ChangeState(new DeathEState());
        searchPlayer.StopSearch();
    }

    public virtual void SetInitData()
    {
        maxHealth = startHealth;
        currentHealth = maxHealth;
        StartPoint = transform.position;
    }

    public virtual void SetData()
    {
        eAnime = GetComponent<Animator>();
        eStateMachine = new EnemyStateMachine(this, new IdleEState());
        eStateMachine.RegisterEState(new MoveEState());
        eStateMachine.RegisterEState(new AttackEState());
        eStateMachine.RegisterEState(new DeathEState());

        if (patrolUnit)
        {
            eStateMachine.RegisterEState(new PatrolEState());
        }

        searchPlayer = GetComponent<SearchPlayer>();
        hitBox = GetComponent<CapsuleCollider>();
    }

    private void OnCollisionEnter(Collision collision)
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
                    if (collision.transform.TryGetComponent<PlayerStatus>(out PlayerStatus value))
                    {
                        value.TakeDamage(attackDamage);
                        Debug.Log(value.remainHealth);
                    }
                }
            }
        }
        else if (collision.transform.CompareTag("Obstacle"))
        {
            if (eStateMachine.CurrentState.GetType() == new RunawayEState().GetType())
            {
                if (collision.contacts[0].thisCollider == hitBox)
                {
                    Vector3 _damagedDir = Vector3.Reflect(transform.forward, collision.GetContact(0).normal);
                    (eStateMachine.states[new RunawayEState().GetType()] as RunawayEState).SetDamagedDir(_damagedDir);
                }
            }
        }
    }
}
