using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class Enemy_FSM : MonoBehaviour, IEnemyData, ICombatable, IAttackable
{
    [System.Serializable]
    public struct EnemyData
    {
        public string currentState;

        public bool isDeath;
        public bool isAttackable;
        public bool chaseMode;
        public bool returnHome;

        public float attackCoolTime;
        
        public float remainHealth;      
        public float previousDamage;
        
        public Vector3 startPoint;
        public Vector3 enemyPos;
        public Quaternion enemyQuat;
        public int patrolNum;

        public string chaseTargetName;
    }

    protected Enemy_FSM.EnemyData enemyData;

    protected EnemyStateMachine eStateMachine;
    protected Animator eAnime;
    protected NavMeshAgent agent;

    [HideInInspector] public SearchPlayer searchPlayer;

    #region Target정보
    public Transform Target { get { return searchPlayer.Target; } }

    public float TargetDis { get { return searchPlayer.TargetDis; } }
    public Vector3 TargetDir { get { return searchPlayer.TargetDir; } }

    [SerializeField]protected Transform chaseTarget;
    public Transform ChaseTarget { get { return chaseTarget; } }
    [HideInInspector] public float chaseTargetDis;
    [HideInInspector] public Vector3 chaseTargetDir;
    #endregion
    #region Enemy 범위 정보

    public float AttackRange { get { return attackRange; } }

    public float ActionRange { get { return searchPlayer.ActionRange; } } //15f;

    public float DetectRange { get { return searchPlayer.DetectRange; } } //30f;
    #endregion  
    #region Enmey 상태
    [Header("EnemyState")]
    public bool isDeath;
    [SerializeField] private bool isAttackable = true;
    public bool chaseMode;
    public bool returnHome;
    
    public bool VisibleTarget { get { return searchPlayer.visibelTarget; } }
    #endregion
    #region Combatable
    [Header("Combat Data")]
    [SerializeField] protected float attackDamage;
    [SerializeField] protected float attackDelay;
    [SerializeField] protected float attackRange = 3.5f;
    [SerializeField] protected Collider attackCollider;
    protected List<GameObject> attackedTargets = new List<GameObject>();

    public float AttackDamage { get { return attackDamage; } }
    public float AttackDelay { get { return attackDelay; } }
    public Collider AttackCollider { get { return attackCollider; } }
    public List<GameObject> AttackedTargets { get { return attackedTargets; } }
    #endregion
    #region Attackable data
    [Header("Attackable Data")]
    [SerializeField] protected float remainHealth;
    [SerializeField] protected float startHealth;
    protected float maxHealth;
    [SerializeField] protected float defencePoint;
    [SerializeField] protected CapsuleCollider hitBox;

    private float previousDamage;

    public bool IsAttackable { get { return isAttackable; } }
    public float StartHealth { get { return startHealth; } }
    public float MaxHealth { get { return maxHealth; } }
    public float RemainHealth { get { return remainHealth; } }
    public float DefencePoint { get { return defencePoint; } }
    public Collider HitBox { get { return hitBox; } }
    #endregion

    [Header("Patrol Info")]
    public bool patrolUnit;
    public Transform[] PatrolPoint;
    public Vector3 StartPoint { get; set; }

    [Header("Etc Data")]
    //Enemy가 Target을 향해 회전하는 속도
    [SerializeField] protected float rotateSpeed = 7f;
    [SerializeField] protected float moveSpeed = 3.5f;
    [SerializeField] int exp;
    public GameObject enemyMark;
    public int Exp { get { return exp; } }
   
    protected virtual void Start()
    {
        //SetInitData();
        //SetData();
    }

    protected virtual void Update()
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

    //_attacker는 enemy를 공격한 대상이다. 현재는 enemy를 공격할 대상이 Player 밖에 없지만 추후 대상을 확장 됐을때를 대비해 미리 만들어둔다.
    public virtual void TakeDamage(float _damage , Transform _attacker, Vector3 _damagedDir = new Vector3())
    {
        //죽은 상태에서는 더 이상 데미지를 받지 않는다.
        if (isDeath)
        {
            return;
        }

        if (!isAttackable)
        {
            _damage = 0;
        }

        if (_damage > 0)
        {
            remainHealth -= _damage;
            //Dagamed();
            Debug.Log(remainHealth / maxHealth * 100 + " %");
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
                (eStateMachine.states[new RunawayEState().ToString()] as RunawayEState).SetDamagedDir(_damagedDir);
            }
        }

        if (RemainHealth <= 0)
        {
            Die();
        }
    }

    #region ChaseTarget
    public void SetChaseTarget(Transform _attacker)
    {
        chaseMode = true;
        //추적 모드에서는 Target이 DetectRange밖으로 나가도 일정 시간 이상 쫓아가게 된다.
        //Target이 DetectRange 밖으로 나가면 Target을 잃게 되는데 목표를 잃지 않게 Target을 따로 저장해준다.
        chaseTarget = _attacker;      
        returnHome = false;
    }

    protected void SearchChaseTarget()
    {
        chaseTargetDir = (chaseTarget.position - transform.position).normalized;
        chaseTargetDis = (chaseTarget.position - transform.position).magnitude;
    }

    public bool VisibleChaseTarget(Vector3 _dir, float _dis)
    {
        if (Physics.Raycast(transform.position + searchPlayer.SightOffset, _dir, _dis, 1 << 12))
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void ResetChaseTarget()
    {
        chaseMode = false;
        chaseTarget = null;
        previousDamage = 0;
    }
    #endregion

    public virtual void Die()
    {
        isDeath = true;
        eStateMachine.ChangeState(new DeathEState());
        searchPlayer.StopSearch();
        enemyMark.SetActive(false);
        hitBox.enabled = false;
        attackCollider.enabled = false;
        //자신을 죽인 타겟에게 경험치를 주도록 구현
        //ex) Die로부터 자신을 죽은 타겟을 받아와 해당 타겟의 경험치를 상승시키도록 한다.
    }

    public virtual void SetInitData()
    {
        maxHealth = startHealth;
        remainHealth = maxHealth;
        StartPoint = transform.position;
    }

    public virtual void SetData()
    {
        eAnime = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();

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

        agent.speed = moveSpeed;

        if (GameData.instance.newGame)
        {
            SetInitData();
        }
    }

    public virtual Enemy_FSM.EnemyData SaveData()
    {
        enemyData.currentState = eStateMachine.CurrentState.ToString();

        enemyData.isDeath = isDeath;
        enemyData.isAttackable = isAttackable;
        enemyData.chaseMode = chaseMode;
        enemyData.returnHome = returnHome;

        enemyData.attackCoolTime = eStateMachine.AttackCoolTime;

        enemyData.remainHealth = remainHealth;
        enemyData.previousDamage = previousDamage;

        enemyData.startPoint = StartPoint;
        enemyData.enemyPos = transform.position;
        enemyData.enemyQuat = transform.rotation;

        enemyData.chaseTargetName = chaseTarget == null ? "" : chaseTarget.name;

        if (patrolUnit)
        {
            enemyData.patrolNum = (eStateMachine.states[new PatrolEState().ToString()] as PatrolEState).SaveData();
        }

        return enemyData;
    }

    public virtual void LoadData(Enemy_FSM.EnemyData _enemyData)
    {
        enemyData = _enemyData;

        transform.position = enemyData.enemyPos;
        transform.rotation = enemyData.enemyQuat;
        
        eStateMachine.LoadData(enemyData.currentState.ToString(), enemyData.attackCoolTime);

        if (enemyData.currentState == new DeathEState().GetType().ToString())
        {
            //Die();
            isDeath = enemyData.isDeath;
            searchPlayer.StopSearch();
            enemyMark.SetActive(false);
            remainHealth = 0;
            hitBox.enabled = false;
            attackCollider.enabled = false;    
            return;
        }

        isAttackable = enemyData.isAttackable;
        chaseMode = enemyData.chaseMode;

        if (chaseMode)
        {
            chaseTarget = GameObject.Find(enemyData.chaseTargetName).transform;
        }

        returnHome = enemyData.returnHome;

        maxHealth = startHealth;
        remainHealth = enemyData.remainHealth;
        previousDamage = enemyData.previousDamage;

        StartPoint = enemyData.startPoint;

        if (patrolUnit)
        {
            (eStateMachine.states[new PatrolEState().ToString()] as PatrolEState).LoadData(enemyData.patrolNum);
        }
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
                    (eStateMachine.states[new RunawayEState().ToString()] as RunawayEState).SetDamagedDir(_damagedDir);
                }
            }
        }
    }
}
