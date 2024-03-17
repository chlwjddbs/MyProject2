using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Enemy : MonoBehaviour
{
    [Serializable]
    public class EnemyData
    {
        public EnemyState enemyState;
        public Vector3 ePos;
        public Quaternion eRotate;
        public float remainHP;
        public float remainMP;
        public bool patrol;
        public int patrolPoint;
        
        public EnemyData(EnemyState _eState, Vector3 _ePos, Quaternion _eRotate ,float _reHp, float _reMp = 0,bool _patrol = false,int _patNum =0)
        {
            enemyState = _eState;
            ePos = _ePos;
            eRotate = _eRotate;
            remainHP = _reHp;
            remainMP = _reMp;
            if (_patrol)
            {
                patrol = _patrol;
                patrolPoint = _patNum;
            }
        }
    }

    public EnemyData eData;

    public EnemyState eState = EnemyState.Idle;

    //Enemy�� Ȱ������
    public bool isActive;

    //Enemy�� �������� ���� ����
    public bool isSet = false;

    //ememy �ִϸ��̼�
    [HideInInspector]public Animator enemyAnime;

    //���� ü��
    public float startHealth;
    //���� ü��
    public float currentHealth;
    //�ִ� ü��
    protected float maxHealth;

    //ó���� ȹ�� ������ ����ġ��
    public float exp;

    //����
    //���� ����
    public float attackRange = 2.5f;
    //���ݷ�
    public float attackDamage = 15f;
    //���� ��Ÿ��
    public float attackCoolTime = 1.5f;
    public float attackTimer = 0;
    public bool isAttack = false;

    //Enemy ȸ���ӵ�
    public float rotateSpeed = 7;

    //���(�÷��̾�)
    public GameObject target;
    //�÷��̾� ��ġ ����
    protected Vector3 targetDir;
    //�÷������ �Ÿ�
    protected float targetDis;

    //Enemy�� ���� ��ġ
    protected Vector3 startPos;
    //���� ��ġ���� �Ÿ�(���ڸ��� ���ư��� ���)
    protected float startDis;

    protected NavMeshAgent agent;

    //�÷��̾� ���� ���� : �������� ���� ���� �÷�� ��� �߰��Ѵ�.
    public float detectRange = 15;

    //�������� : �ش� ���� ������ �÷��̾�� ������ ���� �� �÷��̾ �߰��Ѵ�.
    public float reactionRange = 30;

    protected float disRange;

    //��Ʈ�� �ϴ� �̳ʹ� ����
    public bool isPatroll;

    //��Ʈ�� ����Ʈ ����Ʈ
    public List<Transform> patrollpoints;
    //���� ������ ��Ʈ�� ����Ʈ
    protected int patrollNum;
  
    //�̳ʹ��� ���� üũ
    //���� ����
    public bool isDeath = false;

    //ĳ���� ����
    public bool isCasting = false;

    //���� ��ġ �ΰ���?
    public bool isReturnhome = false;
    
    //�����ϴ� ����
    public bool isChasemode = false;
    public float chaseResetCount;
    public float chaseResetTime = 5;

    //�÷��̾ ���̳���?
    public bool isSeePlayer;

    //�������� ����
    public bool isRunaway = false;
    //������ �Ÿ�
    public float runDis = 5f;
    protected float runDisCount;
    protected Vector3 damageDir;

    //Enemy�� ���� �� Off�� ��Ʈ�ڽ�
    public CapsuleCollider hitBox;

    //Enemy�� Renderer ������
    public GameObject RenderBox;
    //private Vector3 asd = new Vector3(44.5f, 0.5f, -324.75f);

    public GameObject EnemeyMark;

    public Vector3 offset;

    public bool isStop;

    public Sound[] enemySound;
    public string attackSound;
    public string moveSound;
    public string damagedSound;
    public string deadSound;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        /*
        enemyAnime = GetComponentInChildren<Animator>();
        //���� ü�¿� ���� ü���� �־��ش�.
        currentHealth = startHealth;
        //��κ��� ��Ȳ���� ���� ü���� �ִ� ü���̴�.
        maxHealth = startHealth;

        agent = GetComponent<NavMeshAgent>();

        //���� ����� �̳ʹ̰� ó�� �ִ� ��ġ, ��쿡 ���� ���� ��������
        startPos = transform.position;

        disRange = reactionRange * 1.5f;

        //���� �� ��Ʈ�� �̳ʹ��̸�
        if (isPatroll)
        {
            //��Ʈ�� ��ġ�� �̵�
            SetState(EnemyState.Walk);
            agent.SetDestination(patrollpoints[patrollNum].position);
        }
        //�ƴ϶�� ���ڸ��� �� �ֱ�
        else
        {
            SetState(EnemyState.Idle);
        }

        eData = new EnemyData(eState, transform.position, currentHealth);
        */
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!DataManager.instance.isSet)
        {
            return;
        }

        //�̳ʹ̰� �װų� Ȱ������ �ƴҶ� �������� �ʴ´�.
        if (isDeath || !isActive || isStop || !isSet)
        {
            return;
        }

        PlayerDeath();

        //�̳ʹ̰� ĳ���� ���� ���϶� �ٸ� ������ ���� �ʴ´�.
        if (isCasting)
        {
            //agent.ResetPath();
            return;
        }
        
        //�̳ʹ̰� �÷��̾��� ��ġ�� �����Ѵ�.
        CheckTargetPos();

        //���� ��Ÿ���� ȸ���Ǹ� �� �̻� ������� �ʴ´�.
        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        if (isSeePlayer)
        {
            if (attackRange == detectRange && attackRange == reactionRange)
            {
                NotcognizeRange();
            }
            //Debug.Log("����");
            else if (targetDis <= attackRange)
            {
                //Debug.Log("���ݹ���");
                AttackRange();
            }
            //�÷��̾ ���ݹ��� ������ ������ ���� ���� ���� ������
            else if (targetDis > attackRange && targetDis <= detectRange)
            {
                //Debug.Log("��������");
                DetectRange();
            }
            //�÷��̾ �������� �ۿ� ������ �׼ǹ��� ���� ���� ��
            else if (targetDis > detectRange && targetDis <= reactionRange)
            {
                //Debug.Log("�׼ǹ���");
                ActionRange();
            }
            //�÷��̾ �׼ǹ��� �ۿ� ������
            else if (targetDis > reactionRange)
            {
                //Debug.Log("��������");
                NotcognizeRange();
            }
        }
        else
        {
            //Debug.Log("�Ⱥ���");
            if (isChasemode)
            {
                if (targetDis <= detectRange)
                {
                    //Debug.Log("��������");
                    DetectRange();
                }
                //�÷��̾ �������� �ۿ� ������ �׼ǹ��� ���� ���� ��
                else if (targetDis > detectRange && targetDis <= reactionRange)
                {
                    //Debug.Log("�׼ǹ���");
                    ActionRange();
                }
                //�÷��̾ �׼ǹ��� �ۿ� ������
                else if (targetDis > reactionRange)
                {
                    //Debug.Log("��������");
                    NotcognizeRange();
                }
            }
            else
            {
                NotcognizeRange();
            }
        }

        SwitchState();
    }

    public virtual void SwitchState()
    {
        switch (eState)
        {
            case EnemyState.Idle:
                //agent.velocity = Vector3.zero;
                break;

            case EnemyState.Walk:
                //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation((startPos - transform.position)), Time.deltaTime * rotateSpeed);
                if (isRunaway)
                {
                    eState = EnemyState.Runaway;
                }
                else
                {
                    if (isPatroll)
                    {
                        //LookRate((patrollpoints[patrollNum].position - transform.position));
                    }
                    else
                    {
                        //LookRate(startPos - transform.position);
                    }
                }
                break;

            case EnemyState.Attack:
                //agent.ResetPath();

                LookChase();
                break;

            case EnemyState.Damaged:

                break;

            case EnemyState.Casting:
                //agent.ResetPath();

                break;

            case EnemyState.Chase:
                //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetPos), Time.deltaTime * rotateSpeed);
                //LookRate(targetDir);              

                agent.SetDestination(target.transform.position);

                /*
                if (distance > detectRange)
                {
                    SetNextPos();
                }
                */
                break;

            case EnemyState.Runaway:
               

                runDisCount -= Time.deltaTime;
                if (runDisCount > 0)
                {
                    LookRate(damageDir);
                    transform.Translate(Vector3.forward * 3.5f * Time.deltaTime);
                }
                else
                {
                    SetState(EnemyState.Idle);
                    Invoke("SetNextPos", 3f);
                }
                /*
                if(agent.remainingDistance < 1f && isRunaway)
                {
                    //������ ��ġ�� ������ �����ִٰ� �����ð� �ڿ� ������ġ�� ���ư���.
                    Debug.Log("��������");
                    SetState(EnemyState.Idle);
                    //isRunaway = false;
                    Invoke("SetNextPos", 3f);
                }
                */
                break;

            case EnemyState.Death:
                
                break;
        }
    }

    public virtual void AttackRange()
    {
        //�÷��̾ ���ݹ��� �ȿ� �ְ� ������ ������ ��
        if (attackTimer <= 0)
        {
            //����
            isAttack = true;
            SetState(EnemyState.Attack);

            //���ݿ� ��Ÿ�� �ֱ�
            attackTimer = attackCoolTime;
        }
        //�÷��̾ ���ݹ��� �ȿ� �ְ� ������ �Ұ��� �Ҷ�
        else if (attackTimer > 0 && !isAttack)
        {
            //�÷��̾ �ٶ󺻴�.
            SetState(EnemyState.Idle);
            //transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetPos), Time.deltaTime * rotateSpeed);
            LookRate(targetDir);
        }
    }

    public virtual void DetectRange()
    {
        //�÷��̾ �Ѵ´�.
        SetState(EnemyState.Chase);
    }

    public virtual void ActionRange()
    {

        //�ǰݽ� �÷��̾ �Ѿư���
        //�������� ������ �׼� ���������� �ٽ� �ǰ��� ���ϸ� ��� �����ϵ��� �Ѵ�.
        if (isChasemode)
        {
            SetState(EnemyState.Chase);
            chaseResetCount = chaseResetTime;
        }
        else if (isRunaway)
        {
            //����� ���¿��� �ǰݹ޾� ���� ���°� �� ��
            //�÷��̾ �׼� �������� ������ �������� �ʾƵ� ��� ���������Ѵ�.
            //�׷��� ������ �������� �϶� ��� ���� �� �� �ֵ��� �ƹ��� ������ ���� �ʴ´�.
        }
        else
        {
            SetNextPos();
        }
    }

    public virtual void NotcognizeRange()
    {
        if (isRunaway)
        {
        }
        else if (isChasemode)
        {
            chaseResetCount -= Time.deltaTime;
            
            if (chaseResetCount <= 0)
            {
                isChasemode = false;
                SetNextPos();
            }
        }
        else
        {
            SetNextPos();
        }
    }

    public virtual void ResetState()
    {
        isCasting = false;
        isAttack = false;
        isChasemode = false;
    }

    public virtual void SetState(EnemyState _state)
    {
        //���� ���¸� ���� �� �ִϸ��̼� ������ �ʰ� �������ش�.
        if (eState == _state || isDeath)
        {
            return;
        }

        eState = _state;
        //Debug.Log(eState);

        if (_state == EnemyState.Chase || _state == EnemyState.Runaway)
        {
            enemyAnime.SetInteger("eState", (int)(EnemyState.Walk));
        }
        else
        {
            enemyAnime.SetInteger("eState", (int)eState);
        }

        //���°� �ٲ� �� �н��� ������ �ش�.
        //�����ϰų� �װų� �� ���ߴ� �ൿ�� �� �� ��ΰ� ���������� �̲������µ��� �ִϸ��̼��� ������
        agent.ResetPath();
        agent.velocity = Vector3.zero;
    }
    
    public void CheckTargetPos()
    {
        if (reactionRange >= (target.transform.position - transform.position).magnitude)
        {
            //targetDir = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z) - transform.position;
            targetDir = (target.transform.position - transform.position).normalized;
            targetDis = (target.transform.position - transform.position).magnitude;
            SeeThePlayer();
        }
        else if(reactionRange < (target.transform.position - transform.position).magnitude && disRange > (target.transform.position - transform.position).magnitude)
        {
            isSeePlayer = false;
            //targetDis = (target.transform.position - transform.position).magnitude;
            targetDis = disRange;
        }
    }

    protected virtual void SetNextPos()
    {
        isRunaway = false;
        if (isPatroll)
        {
            Vector3 myPos = transform.position;
            if ((patrollpoints[patrollNum].position - transform.position).magnitude < 1f)
            {             
                if (patrollNum == patrollpoints.Count-1)
                {
                    patrollNum = 0;
                }
                else
                {
                    patrollNum++;
                }
                //Debug.Log(patrollNum);
                SetState(EnemyState.Walk);
                agent.SetDestination(patrollpoints[patrollNum].position);
            }
            SetState(EnemyState.Walk);
            agent.SetDestination(patrollpoints[patrollNum].position);
        }
        else
        {
            if ((startPos - transform.position).magnitude >= 1.0f)
            {
                isReturnhome = false;              
                SetState(EnemyState.Walk);
                agent.SetDestination(startPos);
                
            }
            else if ((startPos - transform.position).magnitude < 1.0f && !isReturnhome)
            {
                isReturnhome = true;
                SetState(EnemyState.Idle);
                
            }
        }
    }

    public virtual void LookChase() 
    {
        LookRate(targetDir);
    }

    public void LookRate(Vector3 _rotateDir)
    {
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(_rotateDir), Time.deltaTime * rotateSpeed);
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
            Dagamed();
            Debug.Log(currentHealth/maxHealth*100 + " %");
            PlayEnemySound(damagedSound);
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    //�̳ʹ̿� �÷��̾��� �Ÿ��� ���� �̳ʹ̰� �ǰݽ� ���ϴ� �ൿ
    public void Runaway(Vector3 _damageDir)//���� ���� ����
    {
        damageDir = _damageDir;

        //���� �̳ʹ� ��ġ�κ��� damageDir�������� runDis ������ ��ġ
        Vector3 runpos = transform.position + (damageDir.normalized * runDis);
        runpos.y = 0.5f;
        //Debug.Log(transform.name + " : " + runpos);

        //�÷��̾���� �Ÿ��� �׼ǹ��� ���϶� (����� ���� �� ��)
        if ((transform.position - target.transform.position).magnitude > reactionRange)
        {
            //����� ������ �� �ǰݽ� runpos ��ġ�� ��������.
            isRunaway = true;
            SetState(EnemyState.Runaway);
            runDisCount = runDis;
            //agent.SetDestination(runpos);
            //agent.SetDestination(new Vector3(-37.03f, 0.5f, 42.90f));          
        }
    }

    protected void PlayerDeath()
    {
        if (target.GetComponent<PlayerStatus>().isDeath && !isStop)
        {
            isStop = true;
            ResetState();
            if (eState != EnemyState.Death)
            {
                SetState(EnemyState.Idle);
            }
        }
    }

    public virtual void Dagamed()
    {
        //�÷��̾ �׼� ����(���ݹ���,�������� ����) �ȿ��� ���� ���� �� �������� �ٲ��ش�.
        if ((transform.position-target.transform.position).magnitude <= reactionRange)
        {
            //Debug.Log(targetDis);
            isChasemode = true;
            isRunaway = false;
            chaseResetCount = chaseResetTime;
        }
    }

    public void SeeThePlayer()
    {
        Debug.DrawRay(transform.position+offset, targetDir * targetDis, Color.black);
        //RaycastHit[] hit = Physics.RaycastAll(transform.position, targetDir, targetDis, 4160);
        //�÷��̾� �������� ���̸� ���� ���� ������
        if (Physics.Raycast(transform.position+ offset, targetDir, targetDis, 1<<12))
        {
            //�þ߰� ���� ������ ������ �÷��̾ �Ⱥ��δٰ� �����Ѵ�.
            isSeePlayer = false;
        }
        else
        {
            //�þ߰� ���� ������ �ʾұ� ������ �÷��̾ ���δٰ� �����Ѵ�.
            isSeePlayer = true;
        }
    }
    
    public virtual void Die()
    {
        isDeath = true;
        PlayEnemySound(deadSound);
        eState = EnemyState.Death;
        enemyAnime.SetBool("isDeath", isDeath);
        target.GetComponent<PlayerStatus>().AddExp(exp);
        EnemeyMark.SetActive(false);
        hitBox.enabled = false;
    }

    public void PlayEnemySound(string _eSound)
    {
        Sound sound = null;
        foreach (var s in enemySound)
        {
            if (s.name == _eSound)
            {
                sound = s;
                break;
            }
        }

        if(_eSound == "olf_telpoout")
        {
            Debug.Log(sound.clip.length);
        }

        if (sound != null)
        {
            sound.source.Play();
        }
        else
        {
            Debug.Log(_eSound);
        }
    }

    public void StopEnemySound(string _eSound)
    {
        Sound sound = null;
        foreach (var s in enemySound)
        {
            if (s.name == _eSound)
            {
                sound = s;
                break;
            }
        }

        if (sound != null)
        {
            sound.source.Stop();
        }
        else
        {
            Debug.Log(_eSound);
        }
    }

    public virtual void SetData()
    {
        //Debug.Log("����");
        enemyAnime = GetComponentInChildren<Animator>();

        //���� ����
        AddSound();

        //���� ü�¿� ���� ü���� �־��ش�.
        currentHealth = startHealth;
        //��κ��� ��Ȳ���� ���� ü���� �ִ� ü���̴�.
        maxHealth = startHealth;

        agent = GetComponent<NavMeshAgent>();

        //���� ����� �̳ʹ̰� ó�� �ִ� ��ġ, ��쿡 ���� ���� ��������
        startPos = transform.position;

        disRange = reactionRange * 1.5f;
        //���� �� ��Ʈ�� �̳ʹ��̸�
        if (isPatroll)
        {
            //��Ʈ�� ��ġ�� �̵�
            SetState(EnemyState.Walk);
            agent.SetDestination(patrollpoints[patrollNum].position);
        }
        //�ƴ϶�� ���ڸ��� �� �ֱ�
        else
        {
            SetState(EnemyState.Idle);
        }

        eData = new EnemyData(eState, transform.position, transform.rotation, currentHealth);
    }

    public virtual EnemyData SaveState()
    {
        eData.enemyState = eState;
        eData.ePos = transform.position;
        eData.eRotate = transform.rotation;
        eData.remainHP = currentHealth;
        if (isPatroll)
        {
            eData.patrol = true;
            eData.patrolPoint = patrollNum;
        }
        else
        {
            eData.patrol = false;
        }

        return eData;           
    }

    public virtual void LoadState(EnemyData _eData)
    {
        //Debug.Log("eDataLoad");
        eData = _eData;
        //eState = eData.enemyState;
        SetState(eData.enemyState);
        transform.position = eData.ePos;
        transform.rotation = eData.eRotate;

        if (eData.enemyState == EnemyState.Death)
        {
            isDeath = true;
            currentHealth = 0;
            hitBox.enabled = false;
            enemyAnime.SetBool("isDeath", isDeath);
            EnemeyMark.SetActive(false);
        }
        else
        {
            currentHealth = eData.remainHP;
            if (eData.patrol)
            {
                isPatroll = true;
                patrollNum = eData.patrolPoint;
            }
        }
    }

    public void AddSound()
    {
        foreach (var s in enemySound)
        {
            //AudioManager.instance.AddExternalSound(s);
            s.source = gameObject.AddComponent<AudioSource>();
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
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Obstacle") && isRunaway)
        {
            //Relect(�Ի簢(transform.forward), ��������(collision.GetContact(0).normal)) = �ݻ簢
            damageDir = Vector3.Reflect(transform.forward, collision.GetContact(0).normal);
        }
    }
}

public enum EnemyState
{
    Idle,
    Walk,
    Attack,
    Damaged,
    Casting,
    Chase = 11,
    Runaway = 12,
    Death = 100,
}
