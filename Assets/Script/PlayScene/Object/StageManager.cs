using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;

public class StageManager : MonoBehaviour
{
    protected Fader fader;

    public Player player;
    protected string startStage;
    public string currentStage;

    protected string startArea;
    public string currentArea;

    protected GameData gameData;

    public DropItemManager dropItemManager;
    public List<GameObject> startFieldItems;

    public List<Enemy_FSM> stageEnemy;
    public List<NPC> stageNpc;

    //public List<Enemy> spawnEnemy;

    

    private void Awake()
    {
        gameData = GameData.instance;
        fader = gameData.fader;
        SaveFileManager.isMain = false;
        gameData.FindPlayer();
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

    #region SetData
    public virtual void SetData()
    {
        SetPlayer();
        SetEnemy();
        SetNpc();

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
        player.SetData();
        Inventory.instance.SetData();
        Equipment.instance.SetData();
        SkillBook.instance.SetData();
        GateManager.instence.SetData();
    }

    public virtual void SetEnemy()
    {
        //StageEnemy�� �⺻ ����
    }

    public virtual void SetNpc()
    {
        //StageNpc�� �⺻ ����
    }

    #endregion
    #region LoadData
    public virtual void LoadData()
    {
        currentStage = gameData.userData.currentStage;
        currentArea = gameData.userData.currentArea;

        LoadEnemy();
        LoadItem();
        LoadPlayer();
        LoadQuest();
        LoadNpc();
    }

    public virtual void LoadPlayer()
    { 
        Inventory.instance.LoadData();
        Equipment.instance.LoadData();
        SkillBook.instance.LoadData();       
        GateManager.instence.LoadData();
        player.LoadData();
        //player.GetComponent<PlayerStatus>().LoadBaseData();
        //player.GetComponent<PlayerStatus>().LoadRemainHpMp();
    }

    public virtual void LoadEnemy()
    {
        //StateEnemy�� ������ �ε��Ѵ�.
    }

    public virtual void LoadItem()
    {
        //���� ������ �� Player�� ���� ������ �� Stage�� ������ ������ �ε��Ѵ�.
    }

    public virtual void LoadQuest()
    {
        //�������� ����Ʈ �� �Ϸ��� ����Ʈ �� ����Ʈ ������ �ҷ��´�.
        QuestManager.instance.LoadData();
    }

    public virtual void LoadNpc() 
    { 
        //npc�� ������ �ҷ��´�.
        //����� �Ϸ�� ����Ʈ ������ �ִ�. 2024.07.19
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
        SaveQuest();
    }

    public virtual void SavePlayer()
    {
        player.SaveData();
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

    public virtual void SaveQuest()
    {
        QuestManager.instance.SaveData();
    }

    public void AutoSave()
    {
        gameData.userData.saveTime = DateTime.Now.ToString("yyyy-MM-dd [HH:mm]");
        gameData.userData.sceneName = SceneManager.GetActiveScene().name;
        SaveData();
        gameData.AutoSaveData();
    }
    #endregion
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