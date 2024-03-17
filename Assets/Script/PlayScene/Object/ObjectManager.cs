using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class ObjectManager : MonoBehaviour
{
    protected Fader fader;

    public GameObject player;
    protected string startStage;
    public string currentStage;

    protected string startArea;
    public string currentArea;

    protected DataManager dataManager;

    public DropItemManager dropItemManager;
    public List<GameObject> startFieldItems;

    public List<Enemy> stageEnemy;
    public List<Enemy> spawnEnemy;

    

    private void Awake()
    {
        dataManager = DataManager.instance;
        fader = dataManager.fader;
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
        if (DataManager.instance.newGame)
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
        if (!DataManager.instance.newGame)
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
    }

    public virtual void StartStage()
    {
        for (int i = 0; i < stageEnemy.Count; i++)
        {          
            stageEnemy[i].SetData();
            stageEnemy[i].isSet = true;
        }
    }

    public virtual void LoadData()
    {
        LoadMapData();
    }

    public virtual void LoadMapData()
    {
        currentStage = dataManager.userData.currentStage;
        currentArea = dataManager.userData.currentArea;
        for (int i = 0; i < stageEnemy.Count; i++)
        {
            stageEnemy[i].SetData();
        }
    }

    public virtual void SaveMapdata()
    {
        dataManager.userData.currentStage = currentStage;
        dataManager.userData.currentArea = currentArea;
    }

    public virtual void SaveData()
    {
        dataManager.userData.currentStage = currentStage;
        dataManager.userData.currentArea = currentArea;

        player.GetComponent<PlayerStatus>().SaveData();
        Inventory.instance.SaveData();
        Equipment.instance.SaveData();
        SkillBook.instance.SaveData();
        GateManager.instence.SaveData();
        SaveMapdata();
    }

    public void AutoSave()
    {
        dataManager.userData.saveTime = DateTime.Now.ToString("yyyy-MM-dd [HH:mm]");
        dataManager.userData.sceneName = SceneManager.GetActiveScene().name;
        SaveData();
        dataManager.AutoSaveData();
    }
}
