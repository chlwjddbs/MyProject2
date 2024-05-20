using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    protected Fader fader;

    public GameObject player;
    protected string startStage;
    public string currentStage;

    protected string startArea;
    public string currentArea;

    protected GameData gameData;

    public DropItemManager dropItemManager;
    public List<GameObject> startFieldItems;

    public List<Enemy> stageEnemy;
    public List<Enemy> spawnEnemy;

    

    private void Awake()
    {
        gameData = GameData.instance;
        fader = gameData.fader;
        SaveFileManager.isMain = false;
    }

    void Start()
    {
        SetData();
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.F5))
        {
            AutoSave();
        }
    }

    public virtual void SetData()
    {
        SetPlayer();
        SetEnemy();

        if (GameData.instance.newGame)
        {
            NewStage();
            AutoSave();
        }
        else
        {
            LoadData();
        }

        Time.timeScale = 1;
        gameData.isSet = true;
    }

    public virtual void NewStage()
    {
        startStage = "1st_Floor";
        currentStage = startStage;
        startArea = "ž �Ա�";
        currentArea = startArea;
    }

    public virtual void SetPlayer()
    {
        player.GetComponent<PlayerStatus>().SetData();
        Inventory.instance.SetData();
        Equipment.instance.SetData();
        SkillBook.instance.SetData();
        GateManager.instence.SetData();
    }

    public virtual void SetEnemy()
    {
        //StageEnemy�� �⺻ ����
    }

    #region LoadData
    public virtual void LoadData()
    {
        currentStage = gameData.userData.currentStage;
        currentArea = gameData.userData.currentArea;

        LoadEnemy();
        LoadItem();
        LoadPlayer();
    }

    public virtual void LoadPlayer()
    {
        player.GetComponent<PlayerStatus>().LoadBaseData();
        Inventory.instance.LoadData();
        Equipment.instance.LoadData();
        SkillBook.instance.LoadData();
        GateManager.instence.LoadData();

        player.GetComponent<PlayerStatus>().LoadRemainHpMp();
    }

    public virtual void LoadEnemy()
    {
        //StateEnemy�� ������ �ε��Ѵ�.
    }

    public virtual void LoadItem()
    {
        //���� ������ �� Player�� ���� ������ �� Stage�� ������ ������ �ε��Ѵ�.
    }
    #endregion
    #region SaveData
    public virtual void SaveData()
    {
        gameData.userData.currentStage = currentStage;
        gameData.userData.currentArea = currentArea;

        SavePlayer();
        SaveEnemy();
        SaveItem();
    }

    public virtual void SavePlayer()
    {
        player.GetComponent<PlayerStatus>().SaveData();
        Inventory.instance.SaveData();
        Equipment.instance.SaveData();
        SkillBook.instance.SaveData();
        GateManager.instence.SaveData();
    }

    public virtual void SaveEnemy()
    {
        //StageEnemy ����
    }

    public virtual void SaveItem()
    {
        //StageItem ����
    }
    #endregion
    public void AutoSave()
    {
        gameData.userData.saveTime = DateTime.Now.ToString("yyyy-MM-dd [HH:mm]");
        gameData.userData.sceneName = SceneManager.GetActiveScene().name;
        SaveData();
        gameData.AutoSaveData();
    }
}


        /*       
        void SetData
        if (GameData.instance.newGame)
        {
            StartStage();
        }
        else
        {
            LoadData();
        }

        //�÷��̾� ������ ���� �ҷ����� �������� �ҷ��;� �÷��̾� ���� ���ÿ� ������ ����
        player.GetComponent<PlayerStatus>().SetData();
        Inventory.instance.SetData();
        Equipment.instance.SetData();
        SkillBook.instance.SetData();
        GateManager.instence.SetData();

        //�ε�� ���� HP,MP�� �ҷ����� ��� ���� �Ǳ� ������ ���������� ���� HP�� �����.
        //��� ������ ������ ����� HP,MP�� �����ش�.
        //�ڵ� ���� �� �����غ���.
        if (!GameData.instance.newGame)
        {
            player.GetComponent<PlayerStatus>().remainHealth = dataManager.userData.remainHealth;
            player.GetComponent<PlayerStatus>().playerStatusUI.SetHpBar();
            player.GetComponent<PlayerStatus>().remainMana = dataManager.userData.remainMana;
            player.GetComponent<PlayerStatus>().playerStatusUI.SetMpBar();        
        }
        else
        {
            AutoSave();
        }

        Time.timeScale = 1;
        dataManager.isSet = true;     
        */