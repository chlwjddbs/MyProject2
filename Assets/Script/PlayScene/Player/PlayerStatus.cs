using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using TMPro;

public class PlayerStatus : MonoBehaviour , ISlow_StatusEffect
{
    private GameData dataManager;

    public PlayerStatusUI playerStatusUI;
    private PlayerController player;
    //플레이어의 기본 이동속도
    public float baseMoveSpeed = 14f;

    //플레이어의 현재 이동속도
    [SerializeField] private float moveSpeed;
    public float MoveSpeed { get { return moveSpeed; } }

    public float SpeedRate { get { return 1 + plusRate - minusRate; } }
    public float plusRate { get; set; }
    public float minusRate { get; set; }
    private Slow_StatusEffect previusRate;
    private float rateCount;
    public Dictionary<string, Slow_StatusEffect> slowEffects = new Dictionary<string, Slow_StatusEffect>();

    public Transform statusEffectPos;


    //시작 체력
    public float startHealth = 150f;
    //최대 체력(기본 체력 + 장비나 기타 등으로 올라가는 모든 수치를 합산한 체력)
    public float maxHealth;
    //플레이어의 자체 체력
    public float baseHealth;
    //플레이어의 남은 체력
    public float remainHealth;
    //장비로 오른 체력
    private float equipTotalHealth;
    public float[] equipHealth;

    //시작 마나
    public float startMana = 50f;
    //최대 마나
    public float maxMana;
    //플레이어의 마나
    public float baseMana;
    //플레이어의 남은 마나
    public float remainMana;
    //장비로 오른 체력
    private float equipTotalMana;
    public float[] equipMana;

    //공격력
    public float currentDamage;
    private float basicDamage = 15f;  
    private float equipTotalDamage;
    public float[] equipDamage;

    //방어력
    public float currentDefence;
    private float basicDefence = 5f; 
    private float equipTotalDefence;
    public float[] equipDefence;

    private GameObject target;

    public static bool isDeath;

    public int playerLv = 1;
    public float currentExp = 0;
    private float nextLvExp = 100;

    private bool usetext = true;

    public static UnityAction stausUI;

    public TextMeshProUGUI userNameUI;
    public TextMeshProUGUI levelUI;
    public TextMeshProUGUI expUI;
    public TextMeshProUGUI hpUI;
    public TextMeshProUGUI mpUI;
    public TextMeshProUGUI attackUI;
    public TextMeshProUGUI defenceUI;
    public TextMeshProUGUI moveSpeedUI;

    public LevelUp LevelUPEffect;

    /*
    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<PlayerController>();

        moveSpeed = baseMoveSpeed;
        //처음 게임을 시작하면 시작 스텟이 현재 스텟이자 최대 스텟이다.
        currentHealth = startHealth;
        maxHealth = currentHealth;
        currentMana = startMana;
        maxMana = currentMana;
        playerStatusUI.SetHpBar();
        playerStatusUI.SetMpBar();
        CurrentStatus();
    }
    */

    public void SetData()
    {
        dataManager = GameData.instance;
        player = GetComponent<PlayerController>();
        isDeath = false;

        int equipItems = (int)(EquipType.EquipTypeMax);
        equipDamage = new float[equipItems];
        equipDefence = new float[equipItems];
        equipHealth = new float[equipItems];
        equipMana = new float[equipItems];

        player.enabled = true;
        GetComponent<NavMeshAgent>().enabled = true;
        moveSpeed = baseMoveSpeed;
        //처음 게임을 시작하면 시작 스텟이 현재 스텟이자 최대 스텟이다.
        baseHealth = startHealth;
        remainHealth = startHealth;
        maxHealth = baseHealth;

        baseMana = startMana;
        remainMana = startMana;
        maxMana = baseMana;

        //basicDamage
        //basicDefence

        playerLv = 1;
        currentExp = 0;
        nextLvExp = 100;

        RemainHPSound();

        if (GameData.instance.newGame)
        {
            SetUI();
            SetCondition();
            SetCombatStatus();
        }

    }

    public void SetUI()
    {
        userNameUI.text = GameData.instance.userData.userName;
        levelUI.text = playerLv.ToString();
        expUI.text = currentExp + " / " + nextLvExp;
        hpUI.text = remainHealth + " / " + maxHealth;
        mpUI.text = remainMana + " / " + maxMana;
        attackUI.text = currentDamage.ToString();
        defenceUI.text = currentDefence.ToString();
        moveSpeedUI.text = moveSpeed.ToString();
        stausUI?.Invoke();
    }

    public void SaveData()
    {
        dataManager.userData.playerPos = transform.position;

        dataManager.userData.moveSpeed = baseMoveSpeed;

        dataManager.userData.remainHealth = remainHealth;
        dataManager.userData.health = baseHealth;
        dataManager.userData.remainMana = remainMana;
        dataManager.userData.mana = baseMana;

        dataManager.userData.basicDamage = basicDamage;
        dataManager.userData.basicDefence = basicDefence;

        dataManager.userData.level = playerLv;
        dataManager.userData.nextLvExp = nextLvExp;
        dataManager.userData.currentExp = currentExp;
    }

    public void LoadBaseData()
    {
        GetComponent<NavMeshAgent>().enabled = false;
        transform.position = dataManager.userData.playerPos;
        player.enabled = true;
        GetComponent<NavMeshAgent>().enabled = true;

        baseMoveSpeed = dataManager.userData.moveSpeed;
        moveSpeed = baseMoveSpeed;

        baseHealth = dataManager.userData.health;
        //maxHealth = baseHealth;

        baseMana = dataManager.userData.mana;
        //maxMana = baseMana;

        basicDamage = dataManager.userData.basicDamage;
        basicDefence = dataManager.userData.basicDefence;

        playerLv = dataManager.userData.level;
        nextLvExp = dataManager.userData.nextLvExp;
        currentExp = dataManager.userData.currentExp;
    }

    public void LoadRemainHpMp()
    {
        remainHealth = dataManager.userData.remainHealth;
        remainMana = dataManager.userData.remainMana;

        SetUI();
        SetCondition();
        SetCombatStatus();
    }

    public void TakeDamage(float _damage)
    {
        if (isDeath)
        {
            return;
        }

        _damage = _damage- currentDefence;
        _damage = Mathf.Clamp(_damage, 1, _damage);
        

        remainHealth -= _damage;
        remainHealth = Mathf.Clamp(remainHealth, 0, maxHealth);
        playerStatusUI.SetHpBar();
        //Debug.Log("남은 체력" + currentHealth);

        RemainHPSound();

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

    public void RemainHPSound()
    {
        if(remainHealth / maxHealth <= 0.0f)
        {
            AudioManager.instance.StopAm("hp20under");
            AudioManager.instance.StopAm("hp50under");
        }
        else if (remainHealth / maxHealth <= 0.2f)
        {
            AudioManager.instance.StopAm("hp50under");
            AudioManager.instance.PlayAmSond("hp20under");
        }
        else if (remainHealth / maxHealth <= 0.35f)
        {
            AudioManager.instance.StopAm("hp20under");
            AudioManager.instance.PlayAmSond("hp50under");
        }
        else
        {
            AudioManager.instance.StopAm("hp20under");
            AudioManager.instance.StopAm("hp50under");
        }
    }

    public void UseMana(float _useMana)
    {
        remainMana -= _useMana;
        playerStatusUI.SetMpBar();
    }

    public bool CheckMana(float _skillMana)
    {
        if(_skillMana > remainMana)
        {
            if (usetext)
            {
                usetext = false;
                SequenceText.instance.SetSequenceText(null, "NotEnoughMana");
                Invoke("isUseText", .5f);
            }
            return false;
        }
        else
        {   
            return true;
        }
    }

    public void Attack(GameObject _target)
    {
        target = _target;

    }

    private void Die()
    {
        isDeath = true;
        player.SetState(PlayerState.Death);
        GetComponent<CapsuleCollider>().enabled = false; 
        StartCoroutine("GotoGameOverScene");
        //AudioManager.instance.PlayeSound("Die");   
    }

    IEnumerator GotoGameOverScene()
    {
        yield return new WaitForSeconds(3);
        GameObject.Find("FadeCanvas").GetComponent<Fader>().SceneLoad("GameOverScene");
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
        playerStatusUI.SetHpBar();
        RemainHPSound();
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
        playerStatusUI.SetMpBar();
        Debug.Log("남은 마나 : " + remainMana);
    }

    
    /*
    public void SetCondition()
    {
        maxHealth = baseHealth + equipTotalHealth;
        playerStatusUI.SetHpBar();
        maxMana = baseMana + equipTotalMana;
        playerStatusUI.SetMpBar();
    }
    */
    

    public void SetCondition()
    {
        float _health;
        _health = remainHealth / maxHealth;
        //소수점 5번쨰 자리 반올림 (hp 차이를 최소화 하기 위해 반올림 수를 크게 잡음)
        _health = MathF.Round(_health * 10000) * 0.0001f;
        maxHealth = baseHealth + equipTotalHealth;
        remainHealth = _health * maxHealth;
        remainHealth = MathF.Round(remainHealth);
        playerStatusUI.SetHpBar();
     
        float _mana;
        _mana = remainMana / maxMana;
        _mana = MathF.Round(_mana * 10000) / 10000;
        maxMana = baseMana + equipTotalMana;
        remainMana = _mana * maxMana;
        remainMana = MathF.Round(remainMana);
        playerStatusUI.SetMpBar();

        RemainHPSound();
        stausUI?.Invoke();
    }

    public void SetCombatStatus()
    {
        currentDamage = basicDamage + equipTotalDamage;
        attackUI.text = currentDamage.ToString();

        currentDefence = basicDamage + equipTotalDefence;
        defenceUI.text = currentDefence.ToString();

        stausUI?.Invoke();

        GetComponentInChildren<Weapon>().SetDamage();
    }

    public void Equip(int _equipType , EquipItem equipItem)
    {      
        equipDamage[_equipType] = equipItem.attack;
        float _equipTotalDamage = 0;
        for (int i = 0; i < equipDamage.Length; i++)
        {
            _equipTotalDamage += equipDamage[i];
        }
        equipTotalDamage = _equipTotalDamage;

        equipDefence[_equipType] = equipItem.defence;
        float _equipTotalDefence = 0;
        for (int i = 0; i < equipDefence.Length; i++)
        {
            _equipTotalDefence += equipDefence[i];
        }
        equipTotalDefence = _equipTotalDefence;

        equipHealth[_equipType] = equipItem.health;
        float _euipTotalHealth = 0;
        for (int i = 0; i < equipDefence.Length; i++)
        {
            _euipTotalHealth += equipHealth[i];
        }
        equipTotalHealth = _euipTotalHealth;

        equipMana[_equipType] = equipItem.mana;
        float _euipTotalMana = 0;
        for (int i = 0; i < equipDefence.Length; i++)
        {
            _euipTotalMana += equipMana[i];
        }
        equipTotalMana = _euipTotalMana;

        SetCondition();
        SetCombatStatus();
    }

    public void Unequip(int _equipType, EquipItem item)
    {
        equipDamage[_equipType] = 0;   
        equipTotalDamage -= item.attack;

        equipDefence[_equipType] = 0; 
        equipTotalDefence -= item.defence;

        equipHealth[_equipType] = 0;       
        equipTotalHealth -= item.health;

        equipMana[_equipType] = 0;       
        equipTotalMana -= item.mana;

        SetCondition();
        SetCombatStatus();
    }
     
    public void AddExp(float _exp)
    {
        currentExp += _exp;
        expUI.text = currentExp + " / " + nextLvExp;
        stausUI?.Invoke();

        LevelUp();
    }

    //경험치를 많이 얻은 경우 한번에 많은 레벨이 오를 수도 있다.
    //그렇기 때문에 레벨이 오른 뒤 LevelUp 함수를 다시 불러 경험치 체크를 해준다.
    public void LevelUp()
    {
        if (currentExp >= nextLvExp)
        {
            LevelUPEffect.LevelUpEffect();
            
            playerLv++;
            currentExp -= nextLvExp;
            nextLvExp += (50 * playerLv);

            baseHealth += (10 * playerLv);
            //Debug.Log(baseHealth + " 레벨업중");

            baseMana += (3 * playerLv);

            basicDamage += (3*playerLv);
            basicDefence += 15;

            SetCondition();
            SetCombatStatus();

            RecoveryHP(maxHealth);
            RecoveryMP(maxMana);

            levelUI.text = playerLv.ToString();
            expUI.text = currentExp + " / " + nextLvExp;
            stausUI?.Invoke();
            //Equipment.instance.UpdateDamage?.Invoke();
            LevelUp();
        }

        //if문을 통한 재귀함수 실행시 함수가 끝나야 if문 밖으로 나갈 수 있어서 함수들이 한번에 실행된다.
        //ex) 1레벨에 2000의 경험치로 6레벨업을 하게 되면 if문 내부가 6번 돌고
        //그 후에 if문 밖에 있는 Debug.Log(baseHealth + " 레벨업끝"); 이 6개 연속으로 실행된다.
        //Debug.Log(baseHealth + " 레벨업끝");

    }

    private void isUseText()
    {
        usetext = true;
    }

    private void Abnomal()
    {

    }

    private void OnDestroy()
    {
        if (AudioManager.instance != null)
        {
            AudioManager.instance.StopAm("hp20under");
            AudioManager.instance.StopAm("hp50under");
        }
    }

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
            rateCount = _slowEfc.duration;
            SetSlowEffect();
        }
    }

    public void SetSlowEffect()
    {
        float bestRate = 0f;
        foreach (var item in slowEffects)
        {
            if (bestRate < item.Value.slowRate)
            {
                bestRate = item.Value.slowRate;
            }
        }
        minusRate = bestRate;

        moveSpeed = baseMoveSpeed * SpeedRate;
    }

    public void RemoveStatusEffect(string _slowEfc)
    {
        slowEffects.Remove(_slowEfc);
        SetSlowEffect();
    }
    /*

public void PlusMoveSpeed(float _rate, float _duration = 0)
{
 plusRate = _rate;
 moveSpeed = baseMoveSpeed * Mathf.Clamp(SpeedRate,0,SpeedRate);
 moveSpeedUI.text = moveSpeed.ToString();

 //지속 시간이 있는 효과일때만 지속 시간 후에 변화 해체
 if (_duration != 0)
 {
     StartCoroutine(SetRateCount(SpeedRateEnum.plusRate,plusRate, _duration));
 }
}

public void MinusMoveSpeed(float _rate, float _duration)
{
 moveSpeed = baseMoveSpeed * Mathf.Clamp(SpeedRate, 0, SpeedRate);
 moveSpeedUI.text = moveSpeed.ToString();      

 StartCoroutine(SetRateCount(SpeedRateEnum.minusRate, plusRate, _duration));
}

IEnumerator SetRateCount(SpeedRateEnum _spdEnum, float _rate, float _duration = 0)
{
 while (rateCount > 0)
 {
     yield return new WaitForSeconds(1f);
     rateCount--;
     if (rateCount <= 2)
     {

     }
     else if (rateCount <= 5)
     {

     }
 }

 ResetMoveSpeed(_spdEnum, _rate);
}

public void ResetMoveSpeed()
{

  if(_spdEnum == SpeedRateEnum.plusRate)
  {
      plusRate -= _rate;
  }
  else
  {
      minusRate -= _rate;
  }

  moveSpeed = baseMoveSpeed * Mathf.Clamp(SpeedRate, 0, SpeedRate);
  moveSpeedUI.text = moveSpeed.ToString();
}

*/
}

