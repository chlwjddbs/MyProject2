using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class Player : MonoBehaviour , ICombatable , IAttackable , ISlow_StatusEffect
{
    private GameData dataManager;
    private PlayerStatusUI playerStatusUI;
    private PlayerStateMachine pStateMachine;
    
    private Camera cameraView;

    public PlayerAnimControl playerAnimeControl;
    public AnimatorOverrideController myOverrideAnim;   //������ ��ų ����� ����� ActionClip�� ������ overrideAinme
    private string ActionState = "ActionState";         //����� clip�� ����� Ʈ������ �̸�

    #region Player ����
    [Header("PlayerState")]
    public static bool isDeath = false;
    public bool isAction = false;
    public bool isCasting = false;
    public bool isAttackable;
    #endregion

    #region Player Level
    public LevelUp LevelUPEffect;

    private int playerLv = 1;
    private float currentExp = 0;
    private float nextLvExp;
    private float startNextExp = 100;
    private float addExpCoefficient = 50;

    public int PlayerLv { get { return playerLv; } }
    public float CurrentExp { get { return currentExp; } }
    public float NextLvExp { get { return nextLvExp; } }
    #endregion

    #region Combatable
    [Header("Combat Data")]
   
    [SerializeField] private float attackDamage;         //���� ���ݷ�
    [SerializeField] private float startDamage = 15f;    //���� ���۽� ������ ���ݷ�
    [SerializeField] private float attackDelay;
    [SerializeField] private float attackRange;
    [SerializeField] private Collider attackCollider;
    [SerializeField] private bool multiAttackAble;
         
    private float baseDamage;                            //�⺻ ���ݷ�  
    private float equipDamage;                           //���� �߰� �� ���ݷ�
    
    private float attackCoefficient = 3f;                //���� �� �߰� ���ݷ� ���

    protected List<GameObject> attackedTargets = new List<GameObject>();

    public float AttackDamage { get { return attackDamage; } }
    public float AttackDelay { get { return attackDelay; } }
    public Collider AttackCollider { get { return attackCollider; } }
    public bool MultiAttackAble { get { return multiAttackAble; } }
    public List<GameObject> AttackedTargets { get { return attackedTargets; } }
    #endregion

    #region Attackable data
    [Header("Attackable Data")]
    [SerializeField] private float maxHealth;               //���� HP
    [SerializeField] private float remainHealth;            //���� �ִ� HP
    [SerializeField] private float startHealth = 150f;      //���� ���۽� ������ HP    
    
    private float baseHealth;                               //�⺻ HP
    private float equipHealth;                              //���� �߰� �� HP

    private float healthCoefficient = 10f;                  //���� �� �߰� HP ���

    [SerializeField] private float defencePoint;            //���� ����
    [SerializeField] private float startDefence = 5f;     //���� ���۽� ������ ����

    private float baseDefence;                              //�⺻ ����
    private float equipDefence;                             //���� �߰� �� ����

    private float defenceCoefficient = 15f;

    [SerializeField] private CapsuleCollider hitBox;

    private float previousDamage;

    public bool IsAttackable { get { return isAttackable; } }
    public float StartHealth { get { return startHealth; } }
    public float MaxHealth { get { return maxHealth; } }
    public float RemainHealth { get { return remainHealth; } }
    public float DefencePoint { get { return defencePoint; } }
    public Collider HitBox { get { return hitBox; } }
    #endregion

    #region Spell Data
    //���� ���ݷ� �� ���°� ���õ� ������ ���Ŀ� �߰�
    [Header("spell Data")]
    [SerializeField] private float maxMana;                  //���� MP
    [SerializeField] private float remainMana;               //���� �ִ� MP
    [SerializeField] private float startMana = 50f;          //���� ���۽� ������ MP   

    private float baseMana;                                  //�⺻ MP
    private float equipMana;                                 //���� �߰� �� MP

    private float manaCoefficient = 3f;                     //���� �� �߰� MP ���

    //���� ����
    public float StartMana { get { return startMana; } }
    //�ִ� ����
    public float MaxMana { get { return maxMana; } }
    //�÷��̾��� ����
    public float BaseMana{ get { return baseMana; } }
    //�÷��̾��� ���� ����
    public float RemainMana { get { return remainMana; } }
    //���� ���� ü��
    private float EquipMana { get { return equipMana; } }
    #endregion

    [Header("Etc Data")]
    public Vector3 mousePos;
    
    [HideInInspector] public bool isObject = false;         //���콺 �����Ͱ� Object�� �ִ��� ����
    [HideInInspector] public bool isUI = false;             //���콺 �����Ͱ� UI�� �ִ��� ����
    public float checkObjectDis;                            //player�� object�� �Ÿ�

    private float moveSpeed;
    [SerializeField] private float baseMoveSpeed = 14f;
    public float MoveSpeed { get { return moveSpeed; } }

    public Transform body;
    public Transform effectPos;

    public Sound[] playerSounds;

    #region StatusEffect
    [Header("StatusEffect")]
    public Dictionary<string, Slow_StatusEffect> slowEffects = new Dictionary<string, Slow_StatusEffect>();
    [SerializeField]private Transform statusEffectPos;
    private Slow_StatusEffect previusRate;

    public Transform StatusEffectPos { get { return statusEffectPos; } }
    public float plusRate { get; set; }
    public float minusRate { get; set; }
    public float SpeedRate { get { return 1 + plusRate - minusRate; } }
    #endregion

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        if (!dataManager.isSet)
        {
            return;
        }

        if (isDeath)
        {
            pStateMachine.UpdateElapsedTime();
            pStateMachine.states[new DeathPState().ToString()].OnUpdate();
            return;
        }

        pStateMachine.Update(Time.deltaTime);
    }

    public virtual State ChangeState(State newState)
    {
        return pStateMachine.ChangeState(newState);
    }

    public void LookAtMouse(Vector3 _mousePos)
    {
        if (isAction)
        {
            return;
        }
        //��ũ������ ����� ���̸� �׸�
        Ray ray = cameraView.ScreenPointToRay(_mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            //Vector3 mouseDir;
            //��Ʈ ������ ���콺 �������� ����
            mousePos = (new Vector3(hit.point.x, body.position.y, hit.point.z) - body.position);

            //�÷��̾��� �� ������ ���콺 �������� ����
            body.forward = mousePos.normalized;

            //hit�� ������Ʈ�� true�̸� �÷��̾���� �Ÿ��� ǥ�����ش�.
            if (CheckTag(hit.transform.tag))
            {
                checkObjectDis = (hit.transform.position - body.position).magnitude;
            }
            else
            {
                //�׷��� ������ �Ÿ� �ʱ�ȭ
                checkObjectDis = Mathf.Infinity;
            }
        }
    }

    public void Attack()
    {
        if (AttackRestriction())
        {
            if (Input.GetMouseButtonDown(0))
            {
                ChangeState(new AttackPState());
            }
        }
    }

    public void SetDamage(float _damageCoefiicient = 1f)
    {
        attackDamage = (baseDamage + equipDamage) * _damageCoefiicient;
    }

    public void TakeDamage(float _damage, Transform _attacker = null, Vector3 _damagedDir = new Vector3())
    {
        //���� ���¿����� �� �̻� �������� ���� �ʴ´�.
        if (isDeath)
        {
            return;
        }

        if (!isAttackable)
        {
            return;
        }

        //_damage = _damage - currentDefence;
        _damage = Mathf.Clamp(_damage, 1, _damage);

        remainHealth -= _damage;
        remainHealth = Mathf.Clamp(remainHealth, 0, maxHealth);
        playerStatusUI.SetHpUI();
        //Debug.Log("���� ü��" + currentHealth);

        //RemainHPSound();

        if (remainHealth <= 0)
        {
            Die();
        }
        else
        {
            if (_damage / maxHealth > 0.5f)
            {
                AudioManager.instance.PlayExternalSound("Damaged3");
            }
            else if (_damage / maxHealth > 0.3f)
            {
                AudioManager.instance.PlayExternalSound("Damaged2");
            }
            else
            {
                int ran = UnityEngine.Random.Range(0, 2);
                AudioManager.instance.PlayExternalSound("Damaged" + ran);
            }
        }
    }

    public void Die()
    {
        ChangeState(new DeathPState());
        isDeath = true;
        hitBox.enabled = false;
    }

    #region Check : Tag State Restriction
    private bool CheckTag(string hitTag)
    {
        if (hitTag == "Item") return true;
        if (hitTag == "Npc") return true;
        if (hitTag == "Object") return true;
        return false;
    }

    public bool AttackRestriction()
    {
        if (isAction == true) return false;
        if (isObject == true) return false;
        if (isCasting == true) return false;
        if (isUI == true) return false;
        return true;
    }

    public bool ActionRestriction()
    {
        if (isAction == true) return false;
        if (isCasting == true) return false;
        return true;
    }

    public bool MoveRestriction()
    {
        if (isAction == true) return false;
        if (isCasting == true) return false;
        if (isUI == true) return false;
        return true;
    }

    public string CheckState()
    {
        return pStateMachine.CurrentState.ToString();
    }

    #endregion

    #region ��� ����
    public void Equip(EquipItem _equipItem)
    {
        if (_equipItem.health != 0)
        {
            equipHealth += _equipItem.health;
            SetHP();
        }

        if (_equipItem.mana != 0)
        {
            equipMana += _equipItem.mana;
            SetMp();
        }

        if (_equipItem.attack != 0)
        {
            equipDamage += _equipItem.attack;
            attackDamage = baseDamage + equipDamage;
            playerStatusUI.SetDamageUI();
        }

        if (_equipItem.defence != 0)
        {
            equipDamage += _equipItem.defence;
            defencePoint = baseDefence + equipDefence;
            playerStatusUI.SetDefenceUI();
        }
    }

    public void UnEquip(EquipItem _unEquipItem)
    {
        if (_unEquipItem.health != 0)
        {
            equipHealth -= _unEquipItem.health;
            SetHP();
        }

        if (_unEquipItem.mana != 0)
        {
            equipMana -= _unEquipItem.mana;
            SetMp();
        }

        if (_unEquipItem.attack != 0)
        {
            equipDamage -= _unEquipItem.attack;
            attackDamage = baseDamage + equipDamage;
            playerStatusUI.SetDamageUI();
        }

        if (_unEquipItem.defence != 0)
        {
            equipDamage -= _unEquipItem.defence;
            defencePoint = baseDefence + equipDefence;
            playerStatusUI.SetDefenceUI();
        }
    }
    #endregion

    #region Level
    public void AddExp(float _exp)
    {
        currentExp += _exp;
        playerStatusUI.SetExpUI();

        LevelUp();
    }

    public void LevelUp()
    {
        if (currentExp >= nextLvExp)
        {
            LevelUPEffect.LevelUpEffect();

            currentExp -= nextLvExp;
            nextLvExp += playerLv * addExpCoefficient;

            baseHealth += playerLv * healthCoefficient;
            baseMana += playerLv * manaCoefficient;

            baseDamage += playerLv * attackCoefficient;
            baseDefence += defenceCoefficient;

            //���� ���� 1 -> ������ -> (1���� * ���� ���) ��ŭ ���� -> ������(����2)
            //���������� �ϰԵǸ� 1�������� 2������ �Ѿ�� ���� ����� 2�� �ȴ�. -> ���� ����1 -> ������ -> (2���� * ���� ���)�� �Ǳ� ������ �������� ���� �� �Ѵ�.
            playerLv++;

            RecoveryHP(maxHealth);
            RecoveryMP(maxMana);

            playerStatusUI.SetAllUI();
            LevelUp();
        }
    }
    #endregion

    #region Hp, Mp ����
    public void SetHP()
    {
        float _health;
        _health = remainHealth / maxHealth;
        //�Ҽ��� 5���� �ڸ� �ݿø� (hp ���̸� �ּ�ȭ �ϱ� ���� �ݿø� ���� ũ�� ����)
        _health = MathF.Round(_health * 10000) * 0.0001f;
        maxHealth = baseHealth + equipHealth;
        remainHealth = MathF.Round(_health * maxHealth);
        playerStatusUI.SetHpUI();
        //RemainHPSound();
    }

    public void SetMp()
    {
        float _mana;
        _mana = remainMana / maxMana;
        _mana = MathF.Round(_mana * 10000) / 10000;
        maxMana = baseMana + equipMana;
        remainMana = MathF.Round(_mana * maxMana);
        playerStatusUI.SetMpUI();
    }

    public void RecoveryHP(float _hp)
    {
        if (isDeath)
        {
            return;
        }
        remainHealth += _hp;
        remainHealth = Math.Clamp(remainHealth, 0, maxHealth);
        remainHealth = MathF.Round(remainHealth);
        playerStatusUI.SetHpUI();
        //RemainHPSound();
        Debug.Log("���� ü�� : " + remainHealth);
    }

    public void RecoveryMP(float _mp)
    {
        if (isDeath)
        {
            return;
        }
        remainMana += _mp;
        remainMana = Math.Clamp(remainMana, 0, maxMana);
        remainMana = MathF.Round(remainMana);
        playerStatusUI.SetMpUI();
        Debug.Log("���� ���� : " + remainMana);
    }

    public void UseMana(float _useMana)
    {
        remainMana -= _useMana;
        playerStatusUI.SetMpUI();
    }
    #endregion

    #region StatusEffect
    public void TakeSlowEffect(Slow_StatusEffect _slowEfc)
    {
        if (!slowEffects.TryGetValue(_slowEfc.name, out Slow_StatusEffect value))
        {
            Slow_StatusEffect slowEfc = Instantiate(_slowEfc, statusEffectPos);
            slowEfc.target = this;
            slowEffects.Add(_slowEfc.name, slowEfc);
            SetSlowEffect();
        }
        else
        {
            value.count = 0;
            SetSlowEffect();
        }
    }

    public void SetSlowEffect()
    {
        float rateHigh = 0f;
        foreach (var item in slowEffects)
        {
            if (rateHigh < item.Value.slowRate)
            {
                rateHigh = item.Value.slowRate;
            }
        }
        minusRate = rateHigh;
        moveSpeed = baseMoveSpeed * SpeedRate;
    }

    public void RemoveStatusEffect(string _effect)
    {
        slowEffects.Remove(_effect);
        SetSlowEffect();
    }
    #endregion

    #region �ִϸ����� ���� : ��Ʈ�ѷ�, Ŭ��, �ӵ�, ��� ��
    public void SetAnime(AnimationClip _clip)
    {
        //���� �������� Ŭ���� ���� ���� Ŭ���� �ٸ� Ŭ���� ���� ����
        if (ActionState != _clip.name)
        {
            //ActionState�� Ŭ���� �������ش�.
            myOverrideAnim[ActionState] = _clip;

            //ovrride controller�� �ٲ��� �ʾ��� ���� ����
            //���� �ִϸ�������Ʈ�ѷ��� ���� �� �ִϸ����� ��Ʈ�ѷ��� �ƴҶ��� �ٲ��ش�. �ִϸ��̼��� ����� �� ���� �ٲ����� �ʱ� ����.
            if (playerAnimeControl.playerAnime.runtimeAnimatorController != myOverrideAnim)
            {
                playerAnimeControl.playerAnime.runtimeAnimatorController = myOverrideAnim;
            }
        }
    }

    //���⿡ ���� attack Clip �������ֱ�
    public void SetAttackAnime(AnimationClip _clip)
    {
        myOverrideAnim["Player_Attack"] = _clip;

        if (playerAnimeControl.playerAnime.runtimeAnimatorController != myOverrideAnim)
        {
            playerAnimeControl.playerAnime.runtimeAnimatorController = myOverrideAnim;
        }

    }

    public void SetCastMotion(float _motionSlect)
    {
        playerAnimeControl.playerAnime.SetFloat("MotionSelect", _motionSlect);
    }

    public void SetActionSpeed(float _acSpeed)
    {
        playerAnimeControl.playerAnime.SetFloat("ActionSpeed", _acSpeed);
    }
    #endregion

    #region Data Manager
    public Animator CallPlayerAnime()
    {
        return playerAnimeControl.playerAnime;
    }

    public void SetData()
    {
        dataManager = GameData.instance;
        cameraView = Camera.main;
        playerStatusUI = GameObject.Find("UIManager").GetComponent<PlayerStatusUI>();
        playerAnimeControl = GetComponentInChildren<PlayerAnimControl>();

        //����� ���� ���
        foreach (var s in playerSounds)
        {
            AudioManager.instance.AddExternalSound(s);
        }

        playerAnimeControl.SetData();
        SetState();
        SetValue();
    }

    public void SetState()
    {
        pStateMachine = new PlayerStateMachine(this, new IdlePState());
        pStateMachine.RegisterPState(new MovePState());
        pStateMachine.RegisterPState(new JumpPState());
        pStateMachine.RegisterPState(new AttackPState());
        pStateMachine.RegisterPState(new CastPState());
        pStateMachine.RegisterPState(new ActionPState());
        pStateMachine.RegisterPState(new DeathPState());
    }

    public void SetValue()
    {
        isDeath = false;

        nextLvExp = startNextExp;

        baseHealth = startHealth;
        baseMana = startMana;
        baseDamage = startDamage;
        baseDefence = startDefence;

        moveSpeed = baseMoveSpeed;

        if (dataManager.newGame)
        {
            //ó�� ������ �����ϸ� ���� ������ ���� �������� �ִ� �����̴�.
            playerLv = 1;
            currentExp = 0;

            maxHealth = baseHealth;
            remainHealth = baseHealth;

            maxMana = baseMana;
            remainMana = baseMana;

            attackDamage = startDamage;
            defencePoint = startDefence;

            playerStatusUI.SetAllUI();

            enabled = true;
            isAttackable = true;
        }
    }

    public void LoadData()
    {
        GetComponent<NavMeshAgent>().enabled = false;
        transform.position = dataManager.userData.playerPos;
        enabled = true;
        isAttackable = true;
        GetComponent<NavMeshAgent>().enabled = true;

        baseMoveSpeed = dataManager.userData.moveSpeed;
        moveSpeed = baseMoveSpeed;

        playerLv = dataManager.userData.level;
        currentExp = dataManager.userData.currentExp;

        //���� 1���� ���� ������ �������� �ʴ´�.
        //int = 0 ���͸� �ҷ����� �� for���� �ѹ� ���� ������ ����� �ִ� ������ 0�� ���� 0���� ��������
        //���� ����ġ�� ������ ������ 0�� ���� 0->1������ ����Ѱɷ� �����Ͽ� ���� ������ ���´�.
        //int = 1�����ؼ� ���� �ҷ�����ÿ� 0���� ������ �����ش�.
        for (int i = 1; i < playerLv; i++)
        {
            baseDamage += i * attackCoefficient;
            baseDefence += defenceCoefficient;
            baseHealth += i * healthCoefficient;
            baseMana += manaCoefficient;
        }

        attackDamage = baseDamage + equipDamage;
        defencePoint = baseDefence + equipDefence;
        maxHealth = baseHealth + equipHealth;
        maxMana = baseMana + equipMana;

        remainHealth = dataManager.userData.remainHealth;
        remainMana = dataManager.userData.remainMana;

        playerStatusUI.SetAllUI();
    }

    public void SaveData()
    {
        dataManager.userData.playerPos = transform.position;

        dataManager.userData.level = playerLv;
        dataManager.userData.currentExp = currentExp;

        dataManager.userData.remainHealth = remainHealth;
        dataManager.userData.remainMana = remainMana;

        dataManager.userData.moveSpeed = baseMoveSpeed;

        //�ε�� �������� for������ ������ ������ �� �ö� ���� ��ü�� �������� �����...
        dataManager.userData.health = baseHealth;
        dataManager.userData.mana = baseMana;
        dataManager.userData.basicDamage = baseDamage;
        dataManager.userData.basicDefence = baseDefence;
        dataManager.userData.nextLvExp = nextLvExp;
    }

    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        //Enemy�� �浹�� ������ collider�� 
        if (collision.transform.CompareTag("Enemy"))
        {
            //attackCollider�̸� �������� �ش�.
            if ((collision.contacts[0].thisCollider == attackCollider))
            {
                if (!MultiAttackAble) 
                {
                    if(attackedTargets.Count >= 1)
                    {
                        return;
                    }
                }

                if (!attackedTargets.Contains(collision.gameObject))
                {
                    //ó�� �ߵ��� ����� �����ϰ� �������� �ش�.
                    attackedTargets.Add(collision.gameObject);
                    if (collision.transform.TryGetComponent<IAttackable>(out IAttackable value))
                    {
                        value.TakeDamage(attackDamage,transform);
                        Debug.Log(value.RemainHealth);
                    }
                }
            }
        }
    }
}

public enum PlayerState
{
    Idle,               //0
    Walk,
    Run,
    Attack,
    Jump,
    Action,             //5
    Casting,
    Death = 100,

}
