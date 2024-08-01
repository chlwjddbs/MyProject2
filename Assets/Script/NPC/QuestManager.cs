using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System.Xml;

public class QuestManager : MonoBehaviour
{
    #region Singleton
    public static QuestManager instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    private Player player;
    private Inventory inventory;
    private GameData gameData;

    //xml
    public string xmlFile = "Quest/Quest";
    public XmlNodeList allNodes;
  
    public List<Quest> performingQuest = new List<Quest>();     //진행중인 퀘스트  
    public Quest currentQuest;                                  //현재 상호작용 중인 퀘스트 ex) npc가 퀘스트를 부여하는 대화를 진행 중 부여할 주체가 되는 퀘스트. 퀘스트 수락 버튼을 눌렀을때 진행중인 퀘스트에 등록되어야 하는 퀘스트. 
    public QuestState cuurentState;                             //현재 퀘스트 상태(수락중인지, 진행중인지 등등)
    public ItemManager itemManager;                             //퀘스트 진행시 리워드 아이템 목록을 리스트로 만들고 아이템 저장

    public List<Quest> completeQuest = new List<Quest>();       //완료한 퀘스트

    public Dictionary<string, XmlNodeList> NodeAll = new Dictionary<string, XmlNodeList>();     //퀘스트 정보를 담고있는 xml node
    public Dictionary<string, List<Quest>> AllQuest = new Dictionary<string, List<Quest>>();    //게임 시작시 등록된 퀘스트

    private string codeStr; //퀘스트 보상 아이템을 확인하기 위한 string 변수

    public UnityAction<bool> setQuestUI;
    public UnityAction<Quest> removeList;
    public UnityAction<Quest> loadData;

    public QuestSlot selectSlot; //QuestList에서 선택한 Quest 정보를 담고 있는 슬롯

    public void RegistQuestXml(string npcName, string fileName)
    {
        TextAsset xmlFile = Resources.Load(fileName) as TextAsset;

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlFile.text);
        allNodes = xmlDoc.SelectNodes("root/quest");
        NodeAll[npcName] = xmlDoc.SelectNodes("root/quest");
        Debug.Log("load xml file complete");
    }

    //지정된 인덱스의 Npc에서 퀘스트를 호출
    public List<Quest> SetNpcQuest(string npcName)
    {
        List<Quest> quests = new List<Quest>();

        foreach (XmlNode node in NodeAll[npcName])
        {
            Quest quest = new Quest();
            
            quest.questProgress = new QuestProgress();

            quest.number = int.Parse(node["questNumber"].InnerText);         //퀘스트 번호
            quest.qName = node["questName"].InnerText;                       //퀘스트 이름
            quest.description = node["description"].InnerText;               //퀘스트 설명
            quest.dialogIndex = int.Parse(node["diologIndex"].InnerText);    //퀘스트 진행가능 상태일때 Npc의 dialog
            quest.randomIndex = int.Parse(node["randomIndex"].InnerText);    //퀘스트 진행중 다시 대화를 걸었을때의 dialog
            quest.completeIndex = int.Parse(node["completeIndex"].InnerText);//퀘스트 완료 dialog

            quest.level = int.Parse(node["level"].InnerText);                //퀘스트 레벨

            int qType = int.Parse(node["questType"].InnerText);             //퀘스트 타입 : 킬 퀘스트, 수집 퀘스트 등
      
            quest.questProgress.questType = (QuestType)qType;
            quest.questProgress.currentAmount = 0;                                          //퀘스트의 현재 진행상황 : 퀘스트를 처음 받으면 당연히 0이여야한다.
            quest.questProgress.reachedAmount = int.Parse(node["goalAmount"].InnerText);    //퀘스트 목표 : 퀘스트가 완료되기 위한 조건

            //아이템이나 목표 몬스터의 인덱스를 받아와 처리하도록 한다.
            //ex) 좀비를 잡아 오는 퀘스트를 진행시 좀비의 인덱스가 0이고 뮤턴트의 인덱스가 1이면
            // 인덱스가 0인 좀비만 카운트가 될 수 있게 한다.
            quest.questProgress.typeIndex = int.Parse(node["goalIndex"].InnerText);

            quest.goldReward = int.Parse(node["gold"].InnerText);
            quest.expReward = int.Parse(node["exp"].InnerText);

            //node의 item 번호를 받아와 rewardItem의 리스트 번호로 맞춰 보상 설정
            codeStr = "item";

            int itemIndex = int.Parse(node[codeStr].InnerText);
            int i = 0;
            while (itemIndex > -1)
            {
                quest.itemReward.Add(itemManager.itemManage[itemIndex]); //아이템을 관리하고 있는 itemManager에서 아이템 번호(itemIndex)로 보상으로 줄 아이템을 가져온다.
                i++;
                codeStr = $"item{i}";   //xml node로 부터 아이템을 찾기 때문에 다음 node를 검색할 수 있게 int i 로 반복문을 실행하고 try문을 통해 node를 찾이 못했을때 반목문을 종료한다.
                try
                {
                    itemIndex = int.Parse(node[codeStr].InnerText);
                }
                catch (System.Exception)
                {
                    itemIndex = -1;
                }
            }
            
            quests.Add(quest);
        }

        AllQuest[npcName] = quests; //npc 이름으로 된 모든 퀘스트를 따로 정리한다.

        return quests; //npc 이름으로 된 퀘스트를 반환하여 npc에 등록한다.
    }

    private void Start()
    {
        SetData();
    }

    public void SetData()
    {
        player = GameData.instance.player;
        inventory = Inventory.instance;
        gameData = GameData.instance;
        itemManager = ItemManager.instance;
    }

    public void SaveData()
    {
        GameData.instance.userData.PerperformingQuest = performingQuest;
        GameData.instance.userData.CompleteQuest = completeQuest;
    }

    public void LoadData()
    {
        performingQuest = new List<Quest>(gameData.userData.PerperformingQuest);
        completeQuest = new List<Quest>(gameData.userData.CompleteQuest);

        foreach (var quest in performingQuest)
        {
            currentQuest = quest;
            loadData?.Invoke(currentQuest);
        }
    }

    //플레이어 퀘스트 리스트 중 지정된 퀘스트를 뽑아서 그 퀘스트의 상태를 넘겨준다.
    public QuestState GetQuestState(Quest npcQuest)
    {
        //playerQuests
        cuurentState = QuestState.Ready;
        currentQuest = npcQuest;

        //npc가 여러명이고 퀘스트를 여러개 진행한다는 전재
        foreach (Quest quest in performingQuest)
        {
            if (quest.qName == npcQuest.qName)
            {
                cuurentState = quest.questState;
            }
        }

        return cuurentState;
    }

    private QuestType QuestTypeIndex(int type)
    {
        switch (type)
        {
            case 0:
                return QuestType.Kill;
            case 1:
                return QuestType.Gathering;

        }

        return QuestType.Kill;
    }

    //enemy kill 하면 현재 진행중인 kill quest를 모두 찾아서 currentAmmount++ 
    public void UpdateKillQuest(int enemyType)
    {
        foreach (Quest quest in performingQuest)
        {
            quest.EnemyKill(enemyType);
        }
    }

    //item을 획득 하면 현재 진행중인 gathering quest를 모두 찾아서 currentAmmount++ 
    public void AddCollectQuest(int itemIndex)
    {
        foreach (Quest quest in performingQuest)
        {
            quest.ItemCollect(itemIndex);
        }
    }

    public void RemoveCollectQuest(int itemIndex, int quantity)
    {
        foreach (Quest quest in performingQuest)
        {
            quest.ItemLost(itemIndex, quantity);
        }
    }

    public void CompleteQuest()
    {
        //보상 받기
        if (currentQuest.itemReward != null)
        {
            foreach (var item in currentQuest.itemReward)
            {
                ItemReward(item);
            }
        }

        if(currentQuest.questProgress.questType == QuestType.Gathering)
        {
            inventory.ConsumeQuestItem(currentQuest.questProgress.typeIndex, currentQuest.questProgress.reachedAmount);
            RemoveCollectQuest(currentQuest.questProgress.typeIndex, currentQuest.questProgress.reachedAmount);
        }

        inventory.AddGold(currentQuest.goldReward);
        Debug.Log(currentQuest.goldReward + " 골드를 획득 하였습니다.");
        player.AddExp(currentQuest.expReward);
        Debug.Log(currentQuest.expReward + " 경험치를 획득 하였습니다.");

        currentQuest.questState = QuestState.Complete;
        completeQuest.Add(currentQuest);

        //플레이어 퀘스트리스트에서 삭제
        removeList?.Invoke(currentQuest);
        //performingQuest.Remove(currentQuest);
        foreach (Quest quest in performingQuest)
        {
            if (quest.qName == currentQuest.qName)
            {
                performingQuest.Remove(quest);
                break;
            }
        }
        ResetQuest();
    }

    public void ItemReward(Item item) 
    {
        switch (item.itemType)
        {
            case ItemType.Used:

                inventory.CheckUseableSlot?.Invoke(item); //인벤토리에 현재 보상으로 받는 소모품과 같은 소모품이 있고, 중첩 가능한지 체크.
                if (inventory.UseableSlot)
                {
                    inventory.AddItems(item); //가능하면 보상 지급
                }
                else
                {
                    if (inventory.isAdd) //중첩할 아이템을 찾지 못했지만 인벤토리를 사용가능하면 보상 지급
                    {
                        inventory.AddItems(item);
                    }
                    else
                    {
                        Vector3 dropPos = player.transform.position;
                        dropPos.y = 0;
                        AddItem _potion = Instantiate(item.FieldObject, dropPos, item.FieldObject.transform.rotation, DropItemManager.instance.transform).GetComponent<AddItem>();
                        _potion.quantity = 1;
                    }
                }
                break;
            case ItemType.Ingredient:
                inventory.CheckUseableSlot?.Invoke(item); //인벤토리에 현재 보상으로 받는 소모품과 같은 소모품이 있고, 중첩 가능한지 체크.
                if (inventory.UseableSlot)
                {
                    inventory.AddItems(item); //가능하면 보상 지급
                }
                else
                {
                    if (inventory.isAdd) //중첩할 아이템을 찾지 못했지만 인벤토리를 사용가능하면 보상 지급
                    {
                        inventory.AddItems(item);
                    }
                    else
                    {
                        Vector3 dropPos = player.transform.position;
                        dropPos.y = 0;
                        AddItem _potion = Instantiate(item.FieldObject, dropPos, item.FieldObject.transform.rotation, DropItemManager.instance.transform).GetComponent<AddItem>();
                        _potion.quantity = 1;
                    }
                }
                break;
            default:
                if (inventory.isAdd)
                {
                    inventory.GetItem(item);
                }
                else
                {
                    Vector3 dropPos = player.transform.position;
                    dropPos.y = 0;
                    Instantiate(item.FieldObject, dropPos, item.FieldObject.transform.rotation, DropItemManager.instance.transform);
                }
                break;
        }
    }

    public void SetQuestUI()
    {
        setQuestUI?.Invoke(false);
    }

    public void AcceptQuest()
    {
        cuurentState = QuestState.Accept;
        currentQuest.questState = QuestState.Accept;
        performingQuest.Add(currentQuest);

        if(currentQuest.questProgress.questType == QuestType.Kill)
        {
            
        }
        else if(currentQuest.questProgress.questType == QuestType.Gathering) //수집 퀘스트의 경우 퀘스트에 필요한 아이템이 인벤토리에 미리 존재 할 수 있음으로 인벤토리를 한번 체크해 준다.
        {
            currentQuest.questProgress.currentAmount = inventory.CheckQuestitem(currentQuest.questProgress.typeIndex);
            if(currentQuest.questProgress.isReached())
            {
                currentQuest.questState = QuestState.Complete;
            }
        }
    }

    public void ResetQuest()
    {
        currentQuest = null;
        cuurentState = QuestState.None;
    }

    public void GiveupQuest()
    {
        performingQuest.Remove(currentQuest);
        Destroy(selectSlot.gameObject);
        selectSlot = null;
        ResetQuest();
    }
}

