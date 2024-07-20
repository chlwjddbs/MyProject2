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
        startArea = "탑 입구";
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
        //StageEnemy의 기본 세팅
    }

    public virtual void SetNpc()
    {
        //StageNpc의 기본 세팅
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
        //StateEnemy의 정보를 로드한다.
    }

    public virtual void LoadItem()
    {
        //시작 아이템 및 Player가 버린 아이템 등 Stage의 아이템 정보를 로드한다.
    }

    public virtual void LoadQuest()
    {
        //진행중인 퀘스트 및 완료한 퀘스트 등 퀘스트 정보를 불러온다.
        QuestManager.instance.LoadData();
    }

    public virtual void LoadNpc() 
    { 
        //npc의 정보를 불러온다.
        //현재는 완료된 퀘스트 정보만 있다. 2024.07.19
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
        //StageEnemy 저장
    }

    public virtual void SaveItem()
    {
        //StageItem 저장
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

//플레이어 정보를 먼저 불러오고 아이템을 불러와야 플레이어 정보 세팅에 지장이 없음
player.GetComponent<PlayerStatus>().SetData();
Inventory.instance.SetData();
Equipment.instance.SetData();
SkillBook.instance.SetData();
GateManager.instence.SetData();

//로드시 남은 HP,MP를 불러오고 장비가 장착 되기 때문에 저장전보다 많은 HP가 생긴다.
//장비 장착이 끝나고 저장된 HP,MP로 돌려준다.
//코드 정렬 더 생각해보기.
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