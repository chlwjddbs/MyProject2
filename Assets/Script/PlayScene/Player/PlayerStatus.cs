using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using TMPro;

public class PlayerStatus : MonoBehaviour
{
    private DataManager dataManager;

    public PlayerStatusUI playerStatusUI;
    private PlayerController player;
    //�÷��̾��� �⺻ �̵��ӵ�
    public float baseMoveSpeed = 14f;

    //�÷��̾��� ���� �̵��ӵ�
    public float moveSpeed;

    //���� ü��
    public float startHealth = 150f;
    //�ִ� ü��(�⺻ ü�� + ��� ��Ÿ ������ �ö󰡴� ��� ��ġ�� �ջ��� ü��)
    public float maxHealth;
    //�÷��̾��� ü��
    public float baseHealth;
    //�÷��̾��� ���� ü��
    public float remainHealth;
    //���� ���� ü��
    private float equipTotalHealth;
    public float[] equipHealth;

    //���� ����
    public float startMana = 50f;
    //�ִ� ����
    public float maxMana;
    //�÷��̾��� ����
    public float baseMana;
    //�÷��̾��� ���� ����
    public float remainMana;
    //���� ���� ü��
    private float equipTotalMana;
    public float[] equipMana;

    //���ݷ�
    public float currentDamage;
    private float basicDamage = 15f;  
    private float equipTotalDamage;
    public float[] equipDamage;

    //����
    public float currentDefence;
    private float basicDefence = 5f; 
    private float equipTotalDefence;
    public float[] equipDefence;

    private GameObject target;

    public bool isDeath;

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
        //ó�� ������ �����ϸ� ���� ������ ���� �������� �ִ� �����̴�.
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
        dataManager = DataManager.instance;
        player = GetComponent<PlayerController>();

        int equipItems = (int)(EquipType.EquipTypeMax);
        equipDamage = new float[equipItems];
        equipDefence = new float[equipItems];
        equipHealth = new float[equipItems];
        equipMana = new float[equipItems];

        if (DataManager.instance.newGame)
        {
            player.enabled = true;
            GetComponent<NavMeshAgent>().enabled = true;
            moveSpeed = baseMoveSpeed;
            //ó�� ������ �����ϸ� ���� ������ ���� �������� �ִ� �����̴�.
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
        }
        else
        {
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = dataManager.userData.playerPos;
            player.enabled = true;
            GetComponent<NavMeshAgent>().enabled = true;

            baseMoveSpeed = dataManager.userData.moveSpeed;
            moveSpeed = baseMoveSpeed;

            remainHealth = dataManager.userData.remainHealth;
            baseHealth = dataManager.userData.health;
            maxHealth = baseHealth;

            remainMana = dataManager.userData.remainMana;
            baseMana = dataManager.userData.mana;
            maxMana = baseMana;

            basicDamage = dataManager.userData.basicDamage;
            basicDefence = dataManager.userData.basicDefence;

            playerLv = dataManager.userData.level;
            nextLvExp = dataManager.userData.nextLvExp;
            currentExp = dataManager.userData.currentExp;
        }

        SetUI();
        SetCondition();
        SetCombatStatus();
        RemainHPSound();
    }

    public void SetUI()
    {
        userNameUI.text = DataManager.instance.userData.userName;
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

    public void ResetMoveSpeed()
    {
        moveSpeed = baseMoveSpeed;
        moveSpeedUI.text = moveSpeed.ToString();
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
        //Debug.Log("���� ü��" + currentHealth);

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
        playerStatusUI.SetHpBar();
        RemainHPSound();
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
        playerStatusUI.SetMpBar();
        Debug.Log("���� ���� : " + remainMana);
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
        //�Ҽ��� 5���� �ڸ� �ݿø� (hp ���̸� �ּ�ȭ �ϱ� ���� �ݿø� ���� ũ�� ����)
        _health = MathF.Round(_health * 10000) * 0.0001f;
        maxHealth = baseHealth + equipTotalHealth;
        remainHealth = _health * maxHealth;
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

    //����ġ�� ���� ���� ��� �ѹ��� ���� ������ ���� ���� �ִ�.
    //�׷��� ������ ������ ���� �� LevelUp �Լ��� �ٽ� �ҷ� ����ġ üũ�� ���ش�.
    public void LevelUp()
    {
        if (currentExp >= nextLvExp)
        {
            LevelUPEffect.LevelUpEffect();
            
            playerLv++;
            currentExp -= nextLvExp;
            nextLvExp += (50 * playerLv);

            baseHealth += (10 * playerLv);
            //Debug.Log(baseHealth + " ��������");

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

        //if���� ���� ����Լ� ����� �Լ��� ������ if�� ������ ���� �� �־ �Լ����� �ѹ��� ����ȴ�.
        //ex) 1������ 2000�� ����ġ�� 6�������� �ϰ� �Ǹ� if�� ���ΰ� 6�� ����
        //�� �Ŀ� if�� �ۿ� �ִ� Debug.Log(baseHealth + " ��������"); �� 6�� �������� ����ȴ�.
        //Debug.Log(baseHealth + " ��������");

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
}

