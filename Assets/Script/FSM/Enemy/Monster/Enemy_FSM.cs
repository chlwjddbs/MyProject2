using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;



public class Enemy_FSM : MonoBehaviour, IEnemyData, ICombatable, IAttackable, IRenderer
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
        
        public Vector3 enemyPos;
        public Quaternion enemyQuat;
        public int patrolNum;

        public string chaseTargetName;
        public float chaseCount;
    }

    protected Enemy_FSM.EnemyData enemyData;

    protected EnemyStateMachine eStateMachine;
    protected Animator eAnime;
    protected NavMeshAgent agent;
    public EnemyStatusUI eStatusUI;

    [HideInInspector] public SearchPlayer searchPlayer;

    #region Target����
    public Transform Target { get { return searchPlayer.Target; } }

    public float TargetDis { get { return searchPlayer.TargetDis; } }
    public Vector3 TargetDir { get { return searchPlayer.TargetDir; } }

    protected Transform chaseTarget;
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
    [SerializeField] protected bool isAttackable = true;
    public bool chaseMode;
    public bool returnHome;
    public bool isCasting;
    
    public bool VisibleTarget { get { return searchPlayer.visibelTarget; } }
    #endregion

    #region Combatable
    [Header("Combat Data")]
    [SerializeField] protected float attackDamage;
    [SerializeField] protected float attackDelay;
    [SerializeField] protected float attackRange = 3.5f;
    [SerializeField] protected Collider attackCollider;
    [SerializeField] protected bool multiAttackAble;

    protected List<GameObject> attackedTargets = new List<GameObject>();

    public float AttackDamage { get { return attackDamage; } }
    public float AttackDelay { get { return attackDelay; } }
    public Collider AttackCollider { get { return attackCollider; } }
    public bool MultiAttackAble { get { return multiAttackAble; } }
    public List<GameObject> AttackedTargets { get { return attackedTargets; } }
    #endregion

    #region Attackable data
    [Header("Attackable Data")]
    [SerializeField] protected float remainHealth;
    [SerializeField] protected float startHealth;
    protected float maxHealth;
    [SerializeField] protected float defencePoint;
    [SerializeField] protected CapsuleCollider hitBox;

    protected float previousDamage;

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

    #region ��Ÿ ����
    [Header("Etc Data")]
    //Enemy�� Target�� ���� ȸ���ϴ� �ӵ�
    [SerializeField] protected float rotateSpeed = 7f;
    [SerializeField] protected float moveSpeed = 3.5f;
    [SerializeField] int exp;
    [SerializeField] protected GameObject renderBox;
    public GameObject enemyMark;
    
    public int Exp { get { return exp; } }
    public GameObject RenderBox { get { return renderBox; } }
    #endregion

    [Header("Sound Data")]
    public Sound[] sounds;
    [SerializeField]protected GameObject audios;
    protected Dictionary<string, Sound> soundDic = new Dictionary<string, Sound>();
    [SerializeReference] protected string attackSound;
    [SerializeReference] protected string moveSound;
    [SerializeReference] protected string damagedSound;
    [SerializeReference] protected string deathSound;

    protected virtual void Start()
    {
        //SetInitData();
        //SetData();
    }

    protected virtual void Update()
    {
        if (isDeath)
        {
            return;
        }

        eStateMachine.Update(Time.deltaTime);
        eStatusUI?.Updata();

        if (eStateMachine.AttackCoolTime <= AttackDelay)
        {
            eStateMachine.AttackTimeCount();
        }

        if (chaseMode)
        {
            SearchChaseTarget();
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
    public void PlayESound(string soundName)
    {
        if (soundDic.TryGetValue(soundName, out Sound s))
        {
            s.source.Play();
        }
        else
        {
            Debug.Log($"{soundName} ����� �������� �ʽ��ϴ�.");
        }
    }
    public void StopESound(string soundName)
    {
        if (soundDic.TryGetValue(soundName, out Sound s))
        {
            s.source.Stop();
        }
        else
        {
            Debug.Log($"{soundName} ����� �������� �ʽ��ϴ�.");
        }
    }

    #region �ൿ : LookRotate, Attack, Casting
    public virtual void LookRotate()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(TargetDir), Time.deltaTime * rotateSpeed);
    }
    public virtual void CastingStart() { }
    public virtual void CastingAction() { }
    public virtual void CastingEnd() { }
    public virtual void Attack()
    {
        //����
    }
    public void EndAttack()
    {

    }
    #endregion

    #region attackable
    //_attacker�� enemy�� ������ ����̴�. ����� enemy�� ������ ����� Player �ۿ� ������ ���� ����� Ȯ�� �������� ����� �̸� �����д�.
    public virtual void TakeDamage(float _damage , Transform _attacker, Vector3 _damagedDir = new Vector3())
    {
        //���� ���¿����� �� �̻� �������� ���� �ʴ´�.
        if (isDeath)
        {
            Debug.Log($"{name}�� �̹� �׾����ϴ�.");
            return;
        }

        if (!isAttackable)
        {
            Debug.Log("�����Դϴ�.");
            return;
        }

        if (_damage > 0)
        {
            remainHealth -= _damage;
            Damaged();
            eStatusUI.SetHpBar(true);
            Debug.Log(remainHealth / maxHealth * 100 + " %");
            PlayESound(damagedSound);

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

    public virtual void Damaged()
    {
        
    }

    public virtual void Die()
    {
        isDeath = true;
        eStateMachine.ChangeState(new DeathEState());
        PlayESound(deathSound);
        searchPlayer.StopSearch();
        enemyMark.SetActive(false);
        hitBox.enabled = false;
        attackCollider.enabled = false;
        eStatusUI.EnemyDeath();
        //�ڽ��� ���� Ÿ�ٿ��� ����ġ�� �ֵ��� ����
        //ex) Die�κ��� �ڽ��� ���� Ÿ���� �޾ƿ� �ش� Ÿ���� ����ġ�� ��½�Ű���� �Ѵ�.
    }

    public void OnRenderBox()
    {
        if (!renderBox.activeSelf)
        {
            renderBox.SetActive(true);
        }
    }

    public void OffRenderBox()
    {
        if (renderBox.activeSelf)
        {
            renderBox.SetActive(false);
        }
    }
    #endregion

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

    #region Data Manager
    public virtual void SetData()
    {
        eAnime = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        searchPlayer = GetComponent<SearchPlayer>();
        hitBox = GetComponent<CapsuleCollider>();
        eStatusUI = GetComponentInChildren<EnemyStatusUI>();
      
        SetState();
        SetValue();
        SetPool();
        SetSound();
    }

    public virtual void SetState()
    {
        eStateMachine = new EnemyStateMachine(this, new IdleEState());
        eStateMachine.RegisterEState(new MoveEState());
        eStateMachine.RegisterEState(new AttackEState());
        eStateMachine.RegisterEState(new DeathEState());
        eStateMachine.RegisterEState(new ChaseEState());
        eStateMachine.RegisterEState(new RunawayEState());
        if (patrolUnit)
        {
            eStateMachine.RegisterEState(new PatrolEState());
        }
    }

    public virtual void SetValue()
    {
        maxHealth = startHealth;
        remainHealth = maxHealth;
        StartPoint = transform.position;
        agent.speed = moveSpeed;

        eStatusUI?.SetData(this);
    }

    public virtual void SetPool()
    {

    }

    public void SetSound()
    {
        if (sounds == null)
        {
            return;
        }

        foreach (var s in sounds)
        {
            if(gameObject.name == "Olaf")
            {
                Debug.Log(s.name);
            }
            s.source = audios.AddComponent<AudioSource>();
            //Sound Ŭ������ ���� ���� ���� ������ ������ AudioSource�� ������ �ش�.
            //Sound Ŭ�������� ����� ����� clip�� 
            //�� Ŭ���� ������ ����� �̸�, ó�� ������ volume�� pitch���� �ݺ� ���θ� ������ �������ش�.
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;
            s.source.spatialBlend = 1;
            s.source.rolloffMode = AudioRolloffMode.Custom;
            s.source.maxDistance = 50;

            //���帶�� ����� �뵵�� �ٸ��� ������ �뵵�� ���� �����Ͽ� �ͼ� �׷쿡 ������ �ش�.

            //bgm�� bgm �ͼ��� ����
            if (s.soundType == SoundType.BGM)
            {
                s.source.outputAudioMixerGroup = AudioManager.instance.audioGroups[1];
            }
            //ȿ������ sfx�� ����
            else if (s.soundType == SoundType.SFX)
            {
                s.source.outputAudioMixerGroup = AudioManager.instance.audioGroups[2];
            }
            //������(����� ȯ����) Ambience�� ����
            else
            {
                s.source.outputAudioMixerGroup = AudioManager.instance.audioGroups[3];
            }

            soundDic[s.name] = s;
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

        enemyData.enemyPos = transform.position;
        enemyData.enemyQuat = transform.rotation;

        enemyData.chaseTargetName = chaseTarget == null ? "" : chaseTarget.name;
        if (eStateMachine.CurrentState.ToString() == new ChaseEState().ToString())
        {
            enemyData.chaseCount = (eStateMachine.CurrentState as ChaseEState).Save();
        }

        if (patrolUnit)
        {
            enemyData.patrolNum = (eStateMachine.states[new PatrolEState().ToString()] as PatrolEState).SaveData();
        }

        return enemyData;
    }

    public virtual void LoadData(Enemy_FSM.EnemyData _enemyData)
    {
        enemyData = _enemyData;
        isDeath = enemyData.isDeath;

        eStateMachine.LoadData(enemyData.currentState, enemyData.attackCoolTime);

        transform.position = enemyData.enemyPos;
        transform.rotation = enemyData.enemyQuat;
        
        if (enemyData.isDeath)
        {
            //Die();       
            searchPlayer.StopSearch();
            enemyMark.SetActive(false);
            remainHealth = 0;
            hitBox.enabled = false;
            attackCollider.enabled = false;    
            return;
        }

        isAttackable = enemyData.isAttackable;
        chaseMode = enemyData.chaseMode;
        returnHome = enemyData.returnHome;

        if (chaseMode)
        {
            chaseTarget = GameObject.Find(enemyData.chaseTargetName).transform;

            if (enemyData.currentState == new ChaseEState().ToString())
            {
                (eStateMachine.states[new ChaseEState().ToString()] as ChaseEState).Load(enemyData.chaseCount);
            }
        }

        remainHealth = enemyData.remainHealth;
        previousDamage = enemyData.previousDamage;

        if (patrolUnit)
        {
            (eStateMachine.states[new PatrolEState().ToString()] as PatrolEState).LoadData(enemyData.patrolNum);
        }
    }
    #endregion

    protected virtual void OnCollisionEnter(Collision collision)
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
                    if (collision.transform.TryGetComponent<IAttackable>(out IAttackable value))
                    {
                        value.TakeDamage(attackDamage,null);
                    }
                }
            }
        }

        if (collision.transform.CompareTag("Obstacle"))
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
