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

    #region Target����
    public Transform Target { get { return searchPlayer.Target; } }

    public float TargetDis { get { return searchPlayer.TargetDis; } }
    public Vector3 TargetDir { get { return searchPlayer.TargetDir; } }

    [SerializeField]protected Transform chaseTarget;
    public Transform ChaseTarget { get { return chaseTarget; } }
    [HideInInspector] public float chaseTargetDis;
    [HideInInspector] public Vector3 chaseTargetDir;
    #endregion
    #region Enemy ���� ����

    public float AttackRange { get { return attackRange; } }

    public float ActionRange { get { return searchPlayer.ActionRange; } } //15f;

    public float DetectRange { get { return searchPlayer.DetectRange; } } //30f;
    #endregion  
    #region Enmey ����
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
    //Enemy�� Target�� ���� ȸ���ϴ� �ӵ�
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

    //_attacker�� enemy�� ������ ����̴�. ����� enemy�� ������ ����� Player �ۿ� ������ ���� ����� Ȯ�� �������� ����� �̸� �����д�.
    public virtual void TakeDamage(float _damage , Transform _attacker, Vector3 _damagedDir = new Vector3())
    {
        //���� ���¿����� �� �̻� �������� ���� �ʴ´�.
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

            //Target�� null�� �ƴ϶�� ���� enemy�� DetectRange�ȿ��� Target�� ���� �� �����̴�.
            if (Target != null)
            {
                //���� Target�� Player �ܿ� �� ���� �� �ִ�.
                //�׶� enemy�� ���� ���������� ��ǥ�� �ϴ� Target�� �ٲ��� �ʵ��� ���� ���� ������ �� Target���� �ѵ��� ���ش�.
                if (Target == _attacker)
                {
                    if (previousDamage < _damage)
                    {
                        previousDamage = _damage;
                        SetChaseTarget(_attacker);
                    }
                }
                //enemy�� ������ ����� ã�� ���ߴٸ� �ֺ��� �ٸ� ����� Target�� ã�� �Ѿư���.
                else
                {
                    SetChaseTarget(Target);
                }
            }
            //enemy�� �������� ���� Target�� �������� ������ enemy�� �������� ���°� �ȴ�.
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
        //���� ��忡���� Target�� DetectRange������ ������ ���� �ð� �̻� �Ѿư��� �ȴ�.
        //Target�� DetectRange ������ ������ Target�� �Ұ� �Ǵµ� ��ǥ�� ���� �ʰ� Target�� ���� �������ش�.
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
        //�ڽ��� ���� Ÿ�ٿ��� ����ġ�� �ֵ��� ����
        //ex) Die�κ��� �ڽ��� ���� Ÿ���� �޾ƿ� �ش� Ÿ���� ����ġ�� ��½�Ű���� �Ѵ�.
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
        //Player�� �浹�� ������ collider�� 
        if (collision.transform.CompareTag("Player"))
        {
            //attackCollider�̸� �������� �ش�.
            if ((collision.contacts[0].thisCollider == attackCollider))
            {
                //���� �ѹ��� ������ ���� �ߺ����� �������� �ִ°��� ����.
                //���� �ѹ��� ������ ���� �������� �浹�� �̷���� �ѹ��� ���ݿ� �ι��� �浹�� �̷�� ���ٸ�
                //ó�� �浹�� ����� �����س���, �ٽ� �浹�� ����� ������ �������� �浹�� ������ �Ǹ��� �Ѿ��.
                if (!attackedTargets.Contains(collision.gameObject))
                {
                    //ó�� �ߵ��� ����� �����ϰ� �������� �ش�.
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
