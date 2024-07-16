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

    //xml
    public string xmlFile = "Quest/Quest";
    public XmlNodeList allNodes;

    //
    public List<Quest> performingQuest = new List<Quest>();     //진행중인 퀘스트
    public Quest currentQuest;                                  //현재 상호작용 중인 퀘스트 ex) npc가 퀘스트를 부여하는 대화를 진행 중 부여할 주체가 되는 퀘스트. 퀘스트 수락 버튼을 눌렀을때 진행중인 퀘스트에 등록되어야 하는 퀘스트. 
    public QuestState cuurentState;                             //현재 퀘스트 상태(수락중인지, 진행중인지 등등)
    public List<Item> rewardItem;                               //퀘스트 진행시 리워드 아이템 목록을 리스트로 만들고 아이템 저장

    public List<Quest> completeQuest = new List<Quest>();       //완료한 퀘스트

    public Dictionary<string, XmlNodeList> NodeAll = new Dictionary<string, XmlNodeList>();
    public Dictionary<string, List<Quest>> AllQuest = new Dictionary<string, List<Quest>>();

    private string codeStr;

    public UnityAction<Quest> setQuestUI;

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
            Debug.Log(itemIndex);
            int i = 0;
            while (itemIndex > -1)
            {
                quest.itemReward.Add(rewardItem[itemIndex]);
                i++;
                codeStr = $"item{i}";
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

    //플레이어 퀘스트 리스트 중 지정된 퀘스트를 뽑아서 그 퀘스트의 상태를 넘겨준다.
    public QuestState GetQuestState(Quest npcQuests)
    {
        //playerQuests
        cuurentState = QuestState.Ready;
        currentQuest = npcQuests;

        //npc가 여러명이고 퀘스트를 여러개 진행한다는 전재
        foreach (Quest quest in performingQuest)
        {
            if (quest == npcQuests)
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
    public void UpdateCollectQuest(int itemIndex)
    {
        foreach (Quest quest in performingQuest)
        {
            quest.ItemCollect(itemIndex);
        }
    }

    public void RewardQuest()
    {
        //보상 받기
        if (currentQuest.itemReward != null)
        {
            foreach (var item in currentQuest.itemReward)
            {
                Inventory.instance.AddItem(item);
            }
        }

        //PlayerStats.instance.AddGold(currentQuest.goldReward);
        Debug.Log(currentQuest.goldReward + " 골드를 획득 하였습니다.");
        //PlayerStats.instance.AddExp(currentQuest.expReward);
        Debug.Log(currentQuest.expReward + " 경험치를 획득 하였습니다.");

        //플레이어 퀘스트리스트에서 삭제
        performingQuest.Remove(currentQuest);
    }

    public void SetQuestUI()
    {
        setQuestUI?.Invoke(currentQuest);
    }

    public void ResetQuest()
    {
        currentQuest = null;
        cuurentState = QuestState.None;
    }
}

