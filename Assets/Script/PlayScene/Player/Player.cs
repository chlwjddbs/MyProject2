using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class Player : MonoBehaviour , ICombatable , IAttackable
{
    private GameData dataManager;
    private PlayerStatusUI playerStatusUI;
    private PlayerStateMachine pStateMachine;
    

    private Camera cameraView;

    public Animator playerAnime;
    public AnimatorOverrideController myOverrideAnim;   //장착된 스킬 변경시 변경될 ActionClip을 적용할 overrideAinme
    private string ActionState = "ActionState";         //변경된 clip이 적용될 트렌지션 이름

    #region Player 상태
    [Header("PlayerState")]
    public bool isDeath = false;
    public bool isAction = false;
    public bool isCasting = false;
    public bool isUI = false;
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
   
    [SerializeField] private float attackDamage;         //최종 공격력
    [SerializeField] private float startDamage = 15f;    //게임 시작시 설정된 공격력
    [SerializeField] private float attackDelay;
    [SerializeField] private float attackRange;
    [SerializeField] private Collider attackCollider;
    [SerializeField] private bool multiAttackAble;
         
    private float baseDamage;                            //기본 공격력  
    private float equipDamage;                           //장비로 추가 된 공격력
    
    private float attackCoefficient = 3f;                //레벨 당 추가 공격력 계수

    protected List<GameObject> attackedTargets = new List<GameObject>();

    public float AttackDamage { get { return attackDamage; } }
    public float AttackDelay { get { return attackDelay; } }
    public Collider AttackCollider { get { return attackCollider; } }
    public bool MultiAttackAble { get { return multiAttackAble; } }
    public List<GameObject> AttackedTargets { get { return attackedTargets; } }
    #endregion

    #region Attackable data
    [Header("Attackable Data")]
    [SerializeField] private float maxHealth;               //최종 HP
    [SerializeField] private float remainHealth;            //남아 있는 HP
    [SerializeField] private float startHealth = 150f;      //게임 시작시 설정된 HP    
    
    private float baseHealth;                               //기본 HP
    private float equipHealth;                              //장비로 추가 된 HP

    private float healthCoefficient = 10f;                  //레벨 당 추가 HP 계수

    [SerializeField] private float defencePoint;            //최종 방어력
    [SerializeField] private float startDefence = 5f;     //게임 시작시 설정된 방어력

    private float baseDefence;                              //기본 방어력
    private float equipDefence;                             //장비로 추가 된 방어력

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
    //마법 공격력 등 마력과 관련된 데이터 추후에 추가
    [Header("spell Data")]
    [SerializeField] private float maxMana;                  //최종 MP
    [SerializeField] private float remainMana;               //남아 있는 MP
    [SerializeField] private float startMana = 50f;          //게임 시작시 설정된 MP   

    private float baseMana;                                  //기본 MP
    private float equipMana;                                 //장비로 추가 된 MP

    private float manaCoefficient = 3f;                     //레벨 당 추가 MP 계수

    //시작 마나
    public float StartMana { get { return startMana; } }
    //최대 마나
    public float MaxMana { get { return maxMana; } }
    //플레이어의 마나
    public float BaseMana{ get { return baseMana; } }
    //플레이어의 남은 마나
    public float RemainMana { get { return remainMana; } }
    //장비로 오른 체력
    private float EquipMana { get { return equipMana; } }
    #endregion

    [Header("Etc Data")]
    public Vector3 mousePos;
    
    
    public bool isObject = false;     //마우스 포인터가 Object에 있는지 판정
    public float checkObjectDis;                             //player와 object의 거리

    private float moveSpeed;
    [SerializeField] private float baseMoveSpeed = 14f;
    public float MoveSpeed { get { return moveSpeed; } }

    public Transform body;
    public Transform effectPos;

    

    // Start is called before the first frame update
    void Start()
    {
        SetData();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.timeScale == 0)
        {
            return;
        }

        if (!GameData.instance.isSet)
        {
            return;
        }

        //if (playerStatus.isDeath)
        if (isDeath)
        {
            return;
        }

        if (!isDeath)
        {
            pStateMachine.Update(Time.deltaTime);
        }
    }

    public void LookAtMouse(Vector3 _mousePos)
    {
        if (isAction)
        {
            return;
        }
        //스크린에서 월드로 레이를 그림
        Ray ray = cameraView.ScreenPointToRay(_mousePos);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            //Vector3 mouseDir;
            //히트 지점을 마우스 방향으로 저장
            mousePos = (new Vector3(hit.point.x, body.position.y, hit.point.z) - body.position);

            //플레이어의 앞 방향을 마우스 방향으로 지정
            body.forward = mousePos.normalized;

            //hit된 오브젝트가 true이면 플레이어와의 거리를 표시해준다.
            if (CheckTag(hit.transform.tag))
            {
                checkObjectDis = (hit.transform.position - body.position).magnitude;
            }
            else
            {
                //그렇지 않으면 거리 초기화
                checkObjectDis = Mathf.Infinity;
            }
        }
    }

    public void Move(Vector3 mouseDir)
    {
        //마우스 포인트가 UI 일때 이동 금지
        if (isUI)
        {
            //SetState(PlayerState.Idle);
            return;
        }

        /*
        //마우스 우클릭 유지시
        if (Input.GetMouseButton(1))
        {
            //마우스와 플레이어의 거리가 runRange미만이고 달리지 않고 있을때
            //달리는 도중에는 걷기로 모션 전환 불가
            if (mouseDir.magnitude < runRange && !isRun)
            {
                //걷는다.
                SetState(PlayerState.Walk);
            }

            //마우스와 플레이어의 거리가 runRange이상 일때 달린다.
            else
            {
                SetState(PlayerState.Run);
            }
        }
        */

        //우클릭을 끝낼시 그자리에서 멈춘다.
        if (Input.GetMouseButtonUp(1))
        {
            //SetState(PlayerState.Idle);
        }
    }

    public void TakeDamage(float _damage, Transform _attacker)
    {
        //죽은 상태에서는 더 이상 데미지를 받지 않는다.
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
        //Debug.Log("남은 체력" + currentHealth);

        //RemainHPSound();

        if (remainHealth <= 0)
        {
            Die();
        }
        else
        {
            if (_damage / maxHealth > 0.5f)
            {
                AudioManager.instance.PlayExSound("Damaged3");
            }
            else if (_damage / maxHealth > 0.3f)
            {
                AudioManager.instance.PlayExSound("Damaged2");
            }
            else
            {
                int ran = UnityEngine.Random.Range(0, 2);
                AudioManager.instance.PlayExSound("Damaged" + ran);
            }
        }
    }
    public void Die()
    {
        ChangeState(new DeathPState());
        isDeath = true;
        hitBox.enabled = false;
        StartCoroutine("GotoGameOverScene");
    }

  

    private bool CheckTag(string hitTag)
    {
        if (hitTag == "Item") return true;
        if (hitTag == "Npc") return true;
        if (hitTag == "Object") return true;
        return false;
    }

    public string CheckState()
    {
        return pStateMachine.CurrentState.ToString();
    }

    public virtual State ChangeState(State newState)
    {
        return pStateMachine.ChangeState(newState);
    }

    #region 장비 장착
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

    #region Hp, Mp 관리
    public void SetHP()
    {
        float _health;
        _health = remainHealth / maxHealth;
        //소수점 5번쨰 자리 반올림 (hp 차이를 최소화 하기 위해 반올림 수를 크게 잡음)
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
        Debug.Log("남은 체력 : " + remainHealth);
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
        Debug.Log("남은 마나 : " + remainMana);
    }

    public void UseMana(float _useMana)
    {
        remainMana -= _useMana;
        playerStatusUI.SetMpUI();
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

            //시작 레벨 1 -> 레벨업 -> (1레벨 * 성장 계수) 만큼 성장 -> 레벨업(레벨2)
            //레벨업부터 하게되면 1레벨에서 2레벨에 넘어가는 성장 계수가 2가 된다. -> 시작 레벨1 -> 레벨업 -> (2레벨 * 성장 계수)가 되기 때문에 레벨업을 성정 후 한다.
            playerLv++;

            RecoveryHP(maxHealth);
            RecoveryMP(maxMana);

            playerStatusUI.SetAllUI();
            LevelUp();
        }
    }
    #endregion

    #region 애니메이터 관리 : 컨트롤러, 클립, 속도, 모션 등
    public void SetAnime(AnimationClip _clip)
    {
        //현재 적용중인 클립과 새로 들어온 클립이 다른 클립일 때만 실행
        if (ActionState != _clip.name)
        {
            //ActionState의 클립을 변경해준다.
            myOverrideAnim[ActionState] = _clip;

            //ovrride controller로 바뀌지 않았을 때만 실행
            //현재 애니메이터컨트롤러가 변경 될 애니메이터 컨트롤러가 아닐때만 바꿔준다. 애니메이션이 변경될 때 마다 바꿔주지 않기 위함.
            if (playerAnime.runtimeAnimatorController != myOverrideAnim)
            {
                playerAnime.runtimeAnimatorController = myOverrideAnim;
            }
        }
    }

    public void SetAttackAnime(AnimationClip _clip)
    {
        myOverrideAnim["Player_Attack"] = _clip;

        if (playerAnime.runtimeAnimatorController != myOverrideAnim)
        {
            playerAnime.runtimeAnimatorController = myOverrideAnim;
        }

    }

    public void SetCastMotion(float _motionSlect)
    {
        playerAnime.SetFloat("MotionSelect", _motionSlect);
    }

    public void SetActionSpeed(float _acSpeed)
    {
        playerAnime.SetFloat("ActionSpeed", _acSpeed);
    }
    #endregion

    #region Data Manager
    public Animator CallPlayerAnime()
    {
        return playerAnime;
    }

    public void SetData()
    {
        dataManager = GameData.instance;
        cameraView = Camera.main;
        playerStatusUI = GameObject.Find("UIManager").GetComponent<PlayerStatusUI>();
        playerAnime = GetComponentInChildren<Animator>();

        SetState();
        SetValue();
    }

    public void SetState()
    {
        pStateMachine = new PlayerStateMachine(this, new IdlePState());
        pStateMachine.RegisterPState(new MovePState());
        pStateMachine.RegisterPState(new AttackPState());
        pStateMachine.RegisterPState(new CastPState());
        pStateMachine.RegisterPState(new ActionPState());
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
            //처음 게임을 시작하면 시작 스텟이 현재 스텟이자 최대 스텟이다.
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
        }
    }

    public void LoadData()
    {
        GetComponent<NavMeshAgent>().enabled = false;
        transform.position = dataManager.userData.playerPos;
        enabled = true;
        GetComponent<NavMeshAgent>().enabled = true;

        baseMoveSpeed = dataManager.userData.moveSpeed;
        moveSpeed = baseMoveSpeed;

        playerLv = dataManager.userData.level;
        currentExp = dataManager.userData.currentExp;

        for (int i = 0; i < playerLv; i++)
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

        //로드시 레벨업을 for문으로 돌릴지 레벨업 후 올라간 스텟 자체를 저장할지 고민중...
        dataManager.userData.health = baseHealth;
        dataManager.userData.mana = baseMana;
        dataManager.userData.basicDamage = baseDamage;
        dataManager.userData.basicDefence = baseDefence;
        dataManager.userData.nextLvExp = nextLvExp;
    }

    #endregion

    private void OnCollisionEnter(Collision collision)
    {
        //Enemy와 충돌을 감지한 collider가 
        if (collision.transform.CompareTag("Enemy"))
        {
            //attackCollider이면 데미지를 준다.
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
                    //처음 중돌한 대상은 저장하고 데미지를 준다.
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
