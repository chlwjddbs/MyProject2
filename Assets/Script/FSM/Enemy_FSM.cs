using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_FSM : MonoBehaviour, EnemyStatusDataInterface
{
    protected EnemyStateMachine eStateMachine;
    protected Animator eAnime;

    private SearchPlayer searchPlayer;

    #region Target����
    public Transform Target { get { return searchPlayer.Target; } }

    public float TargetDis { get { return searchPlayer.TargetDis; } }
    public Vector3 TargetDir { get { return searchPlayer.TargetDir; } }
    #endregion   
    #region Enemy ���� ����
    protected float attackRange = 3.5f;
    public float AttackRange { get { return attackRange; } }

    public float DetectRange { get { return searchPlayer.DetectRange; } } //15f;

    protected float reactionRange = 30f;
    public float ReactionRange { get { return reactionRange; } }
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

    public bool VisibleTarget { get { return searchPlayer.VisibelTarget; } }

    public Vector3 StartPoint { get; set; }

    public bool isDeath;

    public GameObject enemyMark;

    public CapsuleCollider hitBox;

    public Collider attackCollider;

    public List<GameObject> atarget = new List<GameObject>();

    public bool chaseMode;
    public bool returnHome;

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

    public virtual void TakeDamage()
    {
        
    }

    public virtual void SaveData()
    {
        
    }

    public virtual void LoadData()
    {
       
    }

    public virtual void TakeDamage(float _damage) 
    {
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
            chaseMode = true;
        }

        if (currentHealth <= 0)
        {
            Die();
        }
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
                if (!atarget.Contains(collision.gameObject))
                {
                    //ó�� �ߵ��� ����� �����ϰ� �������� �ش�.
                    atarget.Add(collision.gameObject);
                    if (collision.transform.TryGetComponent<PlayerStatus>(out PlayerStatus value))
                    {
                        value.TakeDamage(attackDamage);
                        Debug.Log(value.remainHealth);
                    }
                }
            }        
        }
    }
}
