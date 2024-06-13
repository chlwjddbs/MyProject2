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

    #region Target정보
    public Transform Target { get { return searchPlayer.Target; } }

    public float TargetDis { get { return searchPlayer.TargetDis; } }
    public Vector3 TargetDir { get { return searchPlayer.TargetDir; } }

    protected Transform chaseTarget;
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

    #region 기타 정보
    [Header("Etc Data")]
    //Enemy가 Target을 향해 회전하는 속도
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
            Debug.Log($"{soundName} 사운드는 존재하지 않습니다.");
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
            Debug.Log($"{soundName} 사운드는 존재하지 않습니다.");
        }
    }

    #region 행동 : LookRotate, Attack, Casting
    public virtual void LookRotate()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(TargetDir), Time.deltaTime * rotateSpeed);
    }
    public virtual void CastingStart() { }
    public virtual void CastingAction() { }
    public virtual void CastingEnd() { }
    public virtual void Attack()
    {
        //공격
    }
    public void EndAttack()
    {

    }
    #endregion

    #region attackable
    //_attacker는 enemy를 공격한 대상이다. 현재는 enemy를 공격할 대상이 Player 밖에 없지만 추후 대상을 확장 됐을때를 대비해 미리 만들어둔다.
    public virtual void TakeDamage(float _damage , Transform _attacker, Vector3 _damagedDir = new Vector3())
    {
        //죽은 상태에서는 더 이상 데미지를 받지 않는다.
        if (isDeath)
        {
            Debug.Log($"{name}은 이미 죽었습니다.");
            return;
        }

        if (!isAttackable)
        {
            Debug.Log("무적입니다.");
            return;
        }

        if (_damage > 0)
        {
            remainHealth -= _damage;
            Damaged();
            eStatusUI.SetHpBar(true);
            Debug.Log(remainHealth / maxHealth * 100 + " %");
            PlayESound(damagedSound);

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
        //자신을 죽인 타겟에게 경험치를 주도록 구현
        //ex) Die로부터 자신을 죽은 타겟을 받아와 해당 타겟의 경험치를 상승시키도록 한다.
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
            //Sound 클래스를 통해 만든 사운드 정보를 가져와 AudioSource에 세팅해 준다.
            //Sound 클래스에는 재생할 오디오 clip과 
            //그 클립에 접근할 오디오 이름, 처음 설정될 volume과 pitch값과 반복 여부를 가져와 세팅해준다.
            s.source.clip = s.clip;
            s.source.volume = s.volume;
            s.source.pitch = s.pitch;
            s.source.loop = s.loop;
            s.source.playOnAwake = s.playOnAwake;
            s.source.spatialBlend = 1;
            s.source.rolloffMode = AudioRolloffMode.Custom;
            s.source.maxDistance = 50;

            //사운드마다 사용할 용도가 다르기 때문에 용도에 따라 구분하여 믹서 그룹에 저장해 준다.

            //bgm은 bgm 믹서에 저장
            if (s.soundType == SoundType.BGM)
            {
                s.source.outputAudioMixerGroup = AudioManager.instance.audioGroups[1];
            }
            //효과음은 sfx에 저장
            else if (s.soundType == SoundType.SFX)
            {
                s.source.outputAudioMixerGroup = AudioManager.instance.audioGroups[2];
            }
            //나머지(현재는 환경음) Ambience에 저장
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
