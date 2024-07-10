using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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

        //PickUpQuestGiver에서 퀘스트 리스트를 받아오려면 QuestManager에서 LoadQuestXml이 먼저 실행되어야 한다.
        //그래서 LoadQuestXml이 Start에 있으면 PickUpQuestGiver에서 먼저 Start가 돌 가능성이 있기 때문에
        //Awake에서 실행하도록 한다.
        LoadQuestXml(xmlFile);
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

    private void LoadQuestXml(string fileName)
    {
        TextAsset xmlFile = Resources.Load(fileName) as TextAsset;

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlFile.text);
        allNodes = xmlDoc.SelectNodes("root/quest");
        Debug.Log("load xml file complete");
    }

    //지정된 인덱스의 Npc에서 퀘스트를 호출
    public List<Quest> GetNpcQuest(int npcIndex)
    {
        List<Quest> quests = new List<Quest>();

        foreach (XmlNode node in allNodes)
        {
            int index = int.Parse(node["npc"].InnerText);
            if (index == npcIndex)
            {
                Quest quest = new Quest();
                //QuestProgress도 직렬화된 클래스 이기 떄문에 new를 하여 객체를 생성해 줘야 데이터를 담을 수 있다.
                quest.questProgress = new QuestProgress();

                quest.number = int.Parse(node["number"].InnerText);
                quest.npcIndex = index;
                quest.qName = node["name"].InnerText;
                quest.description = node["description"].InnerText;
                quest.dialogIndex = int.Parse(node["diologIndex"].InnerText);
                quest.level = int.Parse(node["level"].InnerText);

                int qType = int.Parse(node["questType"].InnerText);
                //방법1 : 메서드를 통해 QuestType 받아오기
                //quest.questGoal.questType = QuestTypeIndex(qType);
                //방법2 : node["questType"].InnerText를 인트형으로 담아 QuestType으로 형 변환
                quest.questProgress.questType = (QuestType)qType;
                quest.questProgress.currentAmount = 0;
                quest.questProgress.reachedAmount = int.Parse(node["goalAmount"].InnerText);

                //아이템이나 목표 몬스터의 인덱스를 받아와 처리하도록 한다.
                //ex) 좀비를 잡아 오는 퀘스트를 진행시 좀비의 인덱스가 0이고 뮤턴트의 인덱스가 1이면
                // 인덱스가 0인 좀비만 카운트가 될 수 있게 한다.
                quest.questProgress.typeIndex = int.Parse(node["goalIndex"].InnerText);

                quest.goldReward = int.Parse(node["gold"].InnerText);
                quest.expReward = int.Parse(node["exp"].InnerText);

                //node의 item 번호를 받아와 rewardItem의 리스트 번호로 맞춰 보상 설정
                int itemIndex = int.Parse(node["item"].InnerText);
                //아이템 번호가 -1이면 아이템을 지급하지 않음. 보상null
                if (itemIndex > -1)
                {
                    quest.itemReward = rewardItem[itemIndex];
                }
                quests.Add(quest);
            }
        }

        return quests;
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
            Inventory.instance.AddItem(currentQuest.itemReward);
        }

        //PlayerStats.instance.AddGold(currentQuest.goldReward);
        Debug.Log(currentQuest.goldReward + " 골드를 획득 하였습니다.");
        //PlayerStats.instance.AddExp(currentQuest.expReward);
        Debug.Log(currentQuest.expReward + " 경험치를 획득 하였습니다.");

        //플레이어 퀘스트리스트에서 삭제
        performingQuest.Remove(currentQuest);
    }
}

