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
    public AnimatorOverrideController myOverrideAnim;   //장착된 스킬 변경시 변경될 ActionClip을 적용할 overrideAinme
    private string ActionState = "ActionState";         //변경된 clip이 적용될 트렌지션 이름

    #region Player 상태
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
    private float saveHealth;
    
    private float baseHealth;                               //기본 HP
    private float equipHealth;                              //장비로 추가 된 HP

    private float healthCoefficient = 10f;                  //레벨 당 추가 HP 계수

    [SerializeField] private float defencePoint;            //최종 방어력
    [SerializeField] private float startDefence = 5f;       //게임 시작시 설정된 방어력

    private float baseDefence;                              //기본 방어력
    private float equipDefence;                             //장비로 추가 된 방어력

    private float defenceCoefficient = 5f;

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
    private float saveMana;

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
    
    public bool isObject = false;                             //마우스 포인터가 Object에 있는지 판정
    [HideInInspector] public bool isUI = false;               //마우스 포인터가 UI에 있는지 판정
    public static float checkObjectDis;                       //player와 object의 거리
    [SerializeField]private SetCursorImage overMouseItem;
    public LayerMask objMask;

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
            //isAction을 요구하는 상태로 전환시 LookAtMouse 를 사용하지 않는 상태가 있으므로 현재 스크립트에서 overMouseItem = null 처리를 해주는것은 옳지 않다.
            //오브젝트에 overMouse 판정을 담당하는 SetCursorImage 스크립트에서 처리하도록 한다.

            /*
            if (overMouseItem != null)
            {
                checkObjectDis = Mathf.Infinity;
                overMouseItem.DontAction();
                if (overMouseItem.TryGetComponent<AddItem>(out AddItem item))
                {
                    item.DontAction();
                }
                overMouseItem = null;
            }
            */
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

            if (Physics.Raycast(ray, out RaycastHit obj, Mathf.Infinity,objMask))
            {
                //Debug.Log(obj.transform.name);
                
                //플레이어가 아이템위에 마우스를 올린 후
                //기존에 확인한 아이템이 없을 경우
                if(overMouseItem == null)           
                {
                    //새로 확인한 아이템 정보를 저장한다.
                    if (obj.transform.TryGetComponent<SetCursorImage>(out SetCursorImage item))
                    {
                        item.overMouse = true;
                        overMouseItem = item;
                        checkObjectDis = (obj.transform.position - body.position).magnitude;
                    }
                }
                //현재 확인한 아이템과 기존에 확인한 아이템이 다를경우
                else if(overMouseItem.transform != obj.transform)
                {
                    //기존에 확인한 아이템이 연결을 해제 한 후 현재 확인한 아이템을 등록해준다.
                    overMouseItem.overMouse = false;
                    if (obj.transform.TryGetComponent<SetCursorImage>(out SetCursorImage item))
                    {
                        overMouseItem = item;
                        overMouseItem.overMouse = true;
                        checkObjectDis = (obj.transform.position - body.position).magnitude;
                    }
                }
                //현재 확인한 아이템과 기존 확인한 아이템이 같을 경우 거리만 갱신해 준다.
                else if(overMouseItem.transform == obj.transform)
                {
                    overMouseItem.overMouse = true; //아이템에 마우스를 올려 놓고 Action 동작을 하면 overMouse가 false처리되어 마우스 이미지가 갱신되지 않는다. isAction 이후에도 마우스가 아이템 위에 있으면 overMouse true체크를 해준다.
                    checkObjectDis = (obj.transform.position - body.position).magnitude;
                }
            }
            else
            {
                //그렇지 않으면 거리 초기화 
                checkObjectDis = Mathf.Infinity;
                if(overMouseItem != null)
                {
                    overMouseItem.overMouse = false;
                    overMouseItem.DontAction();
                    if (overMouseItem.TryGetComponent<AddItem>(out AddItem item))
                    {
                        item.DontAction();
                    }
                    overMouseItem = null;
                }
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
    public void ResetAttack()
    {
        attackCollider.enabled = false;
        attackedTargets.Clear();
        isAction = false;
    }

    public void SetDamage(float _damageCoefiicient = 1f , bool multiAttack = false)
    {
        attackDamage = (baseDamage + equipDamage) * _damageCoefiicient;
        multiAttackAble = multiAttack;
    }

    public void TakeDamage(float _damage, Transform _attacker = null, Vector3 _damagedDir = new Vector3())
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

        _damage = _damage - defencePoint;
        _damage = Mathf.Clamp(_damage, 1, _damage);

        remainHealth -= _damage;
        remainHealth = Mathf.Clamp(remainHealth, 0, maxHealth);
        playerStatusUI.SetHpUI();
        //Debug.Log("남은 체력" + currentHealth);

        HPSound();

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

    #region 장비 장착
    public void Equip(EquipItem _equipItem)
    {
        if (_equipItem.health != 0)
        {
            if (_equipItem.health + maxHealth < 0) //장비의 체력감소 옵션이 현재 최대 체력보다 높을때 체력은 음수가 될 수 없으므로 1이 된다. 이때는 장비를 장착하기 전 체력을 저장해놓고 장비 장착 해제시 hp를 돌려주도록 한다.
            {
                if (maxHealth != 1)
                {
                    saveHealth = remainHealth / maxHealth;
                    saveHealth = MathF.Round(saveHealth * 10000) * 0.0001f;
                }
            }
            equipHealth += _equipItem.health;
            SetHP();
        }

        if (_equipItem.mana != 0)
        {
            if (_equipItem.mana + maxMana < 0) //장비의 체력감소 옵션이 현재 최대 체력보다 높을때 체력은 음수가 될 수 없으므로 1이 된다. 이때는 장비를 장착하기 전 체력을 저장해놓고 장비 장착 해제시 hp를 돌려주도록 한다.
            {
                if (maxMana != 1)
                {
                    saveMana = remainMana / maxMana;
                    saveMana = MathF.Round(saveMana * 10000) * 0.0001f;
                }
            }
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
            equipDefence += _equipItem.defence;
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
            equipDefence -= _unEquipItem.defence;
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
            maxHealth = baseHealth + equipHealth;
            maxHealth = Mathf.Clamp(maxHealth,1, baseHealth + equipHealth);

            baseMana += playerLv * manaCoefficient;
            maxMana = baseMana + equipMana;
            maxMana = Mathf.Clamp(maxMana, 1, baseMana + equipMana);

            baseDamage += playerLv * attackCoefficient;
            attackDamage = baseDamage + equipDamage;
            baseDefence += defenceCoefficient;
            defencePoint = baseDefence + equipDefence;

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

    #region Hp, Mp 관리
    public void SetHP()
    {
        float _health;
        _health = remainHealth / maxHealth;
        //소수점 5번쨰 자리 반올림 (hp 차이를 최소화 하기 위해 반올림 수를 크게 잡음)
        _health = MathF.Round(_health * 10000) * 0.0001f;
        maxHealth = baseHealth + equipHealth;
        maxHealth = Mathf.Clamp(maxHealth, 1, baseHealth + equipHealth);

        if (saveHealth > 0)                      //saveHealth가 있다는건 기존 MaxHealth가 음수였다는 소리이다. 
        {
            if (baseHealth + equipHealth > 0)    //saveHealth가 있고 maxHealth가 음수를 벗어났을때 음수가 되기전 hp수치를 적용해 준다. %위에서 clamp로 maxHealth를 묶었기 때문에 baseHealth + equipHealth를 조건문으로 넣어준다.
            {
                remainHealth = MathF.Round(saveHealth * maxHealth);
                saveHealth = 0;
            }
            else
            {
                remainHealth = 1;
            }
        }
        else
        {
            remainHealth = MathF.Round(_health * maxHealth);
            //remainHealth = Mathf.Clamp(remainHealth, 1, remainHealth);
        }
        playerStatusUI.SetHpUI();
        HPSound();
    }

    public void HPSound()
    {
        if (remainHealth / maxHealth <= 0.0f)
        {
            AudioManager.instance.StopAm("DangerHeart");
            AudioManager.instance.StopAm("WarningHeart");
        }
        else if (remainHealth / maxHealth <= 0.25f)
        {
            AudioManager.instance.StopAm("WarningHeart");
            AudioManager.instance.PlayAmSond("DangerHeart");
        }
        else if (remainHealth / maxHealth <= 0.4f)
        {
            AudioManager.instance.StopAm("DangerHeart");
            AudioManager.instance.PlayAmSond("WarningHeart");
        }
        else
        {
            AudioManager.instance.StopAm("DangerHeart");
            AudioManager.instance.StopAm("WarningHeart");
        }
    }

    public void SetMp()
    {
        float _mana;
        _mana = remainMana / maxMana;
        _mana = MathF.Round(_mana * 10000) / 10000;
        maxMana = baseMana + equipMana;
        maxMana = Mathf.Clamp(maxMana, 1, baseMana + equipMana);

        if (saveMana > 0)                      
        {
            if (baseMana + equipMana > 0)
            {
                remainMana = MathF.Round(saveMana * maxMana);
                saveMana = 0;
            }
            else
            {
                remainMana = 1;
            }
        }
        else
        {
            remainMana = MathF.Round(_mana * maxMana);
        }
        
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
        HPSound();
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
            if (playerAnimeControl.playerAnime.runtimeAnimatorController != myOverrideAnim)
            {
                playerAnimeControl.playerAnime.runtimeAnimatorController = myOverrideAnim;
            }
        }
    }

    //무기에 따른 attack Clip 변경해주기
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

        //사용할 사운드 등록
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

        //레벨 1때는 성장 스텟이 존재하지 않는다.
        //int = 0 부터면 불러오기 시 for문이 한번 돌기 때문에 계수가 있는 스텟은 0이 들어가면 0으로 들어오지만
        //고정 성장치를 가지는 스텟은 0이 들어가면 0->1레벨로 상승한걸로 간주하여 성장 스텟이 들어온다.
        //int = 1부터해서 게임 불러오기시에 0레벨 성장을 막아준다.
        for (int i = 1; i < playerLv; i++)
        {
            nextLvExp += i * addExpCoefficient;
            baseDamage += i * attackCoefficient;
            baseDefence += defenceCoefficient;
            baseHealth += i * healthCoefficient;
            baseMana += i * manaCoefficient;
        }

        attackDamage = baseDamage + equipDamage;
        defencePoint = baseDefence + equipDefence;

        maxHealth = baseHealth + equipHealth;
        SetHP();
        maxMana = baseMana + equipMana;
        SetMp();

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
                //Debug.Log(MultiAttackAble);
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
