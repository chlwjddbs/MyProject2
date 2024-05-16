using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_FSM : MonoBehaviour, EnemyStatusDataInterface, Damageable
{
    protected EnemyStateMachine eStateMachine;
    protected Animator eAnime;

    [HideInInspector] public SearchPlayer searchPlayer;

    #region Target����
    public Transform Target { get { return searchPlayer.Target; } }

    public float TargetDis { get { return searchPlayer.TargetDis; } }
    public Vector3 TargetDir { get { return searchPlayer.TargetDir; } }
    #endregion   
    #region Enemy ���� ����
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
    
    //Enemy�� Target�� ���� ȸ���ϴ� �ӵ�
    protected float rotateSpeed = 7f;
    #endregion
    #region Enmey ����
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

    //_attacker�� enemy�� ������ ����̴�. ����� enemy�� ������ ����� Player �ۿ� ������ ���� ����� Ȯ�� �������� ����� �̸� �����д�.
    public virtual void TakeDamage(float _damage , Transform _attacker, Vector3 _damagedDir = new Vector3())
    {
        //���� ���¿����� �� �̻� �������� ���� �ʴ´�.
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
        //���� ��忡���� Target�� DetectRange������ ������ ���� �ð� �̻� �Ѿư��� �ȴ�.
        //Target�� DetectRange ������ ������ Target�� �Ұ� �Ǵµ� ��ǥ�� ���� �ʰ� Target�� ���� �������ش�.
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
                    (eStateMachine.states[new RunawayEState().GetType()] as RunawayEState).SetDamagedDir(_damagedDir);
                }
            }
        }
    }
}
