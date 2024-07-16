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
    public List<Quest> performingQuest = new List<Quest>();     //�������� ����Ʈ
    public Quest currentQuest;                                  //���� ��ȣ�ۿ� ���� ����Ʈ ex) npc�� ����Ʈ�� �ο��ϴ� ��ȭ�� ���� �� �ο��� ��ü�� �Ǵ� ����Ʈ. ����Ʈ ���� ��ư�� �������� �������� ����Ʈ�� ��ϵǾ�� �ϴ� ����Ʈ. 
    public QuestState cuurentState;                             //���� ����Ʈ ����(����������, ���������� ���)
    public List<Item> rewardItem;                               //����Ʈ ����� ������ ������ ����� ����Ʈ�� ����� ������ ����

    public List<Quest> completeQuest = new List<Quest>();       //�Ϸ��� ����Ʈ

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

    //������ �ε����� Npc���� ����Ʈ�� ȣ��
    public List<Quest> SetNpcQuest(string npcName)
    {
        List<Quest> quests = new List<Quest>();

        foreach (XmlNode node in NodeAll[npcName])
        {
            Quest quest = new Quest();
            
            quest.questProgress = new QuestProgress();

            quest.number = int.Parse(node["questNumber"].InnerText);         //����Ʈ ��ȣ
            quest.qName = node["questName"].InnerText;                       //����Ʈ �̸�
            quest.description = node["description"].InnerText;               //����Ʈ ����
            quest.dialogIndex = int.Parse(node["diologIndex"].InnerText);    //����Ʈ ���డ�� �����϶� Npc�� dialog
            quest.randomIndex = int.Parse(node["randomIndex"].InnerText);    //����Ʈ ������ �ٽ� ��ȭ�� �ɾ������� dialog
            quest.completeIndex = int.Parse(node["completeIndex"].InnerText);//����Ʈ �Ϸ� dialog

            quest.level = int.Parse(node["level"].InnerText);                //����Ʈ ����

            int qType = int.Parse(node["questType"].InnerText);             //����Ʈ Ÿ�� : ų ����Ʈ, ���� ����Ʈ ��
      
            quest.questProgress.questType = (QuestType)qType;
            quest.questProgress.currentAmount = 0;                                          //����Ʈ�� ���� �����Ȳ : ����Ʈ�� ó�� ������ �翬�� 0�̿����Ѵ�.
            quest.questProgress.reachedAmount = int.Parse(node["goalAmount"].InnerText);    //����Ʈ ��ǥ : ����Ʈ�� �Ϸ�Ǳ� ���� ����

            //�������̳� ��ǥ ������ �ε����� �޾ƿ� ó���ϵ��� �Ѵ�.
            //ex) ���� ��� ���� ����Ʈ�� ����� ������ �ε����� 0�̰� ����Ʈ�� �ε����� 1�̸�
            // �ε����� 0�� ���� ī��Ʈ�� �� �� �ְ� �Ѵ�.
            quest.questProgress.typeIndex = int.Parse(node["goalIndex"].InnerText);

            quest.goldReward = int.Parse(node["gold"].InnerText);
            quest.expReward = int.Parse(node["exp"].InnerText);

            //node�� item ��ȣ�� �޾ƿ� rewardItem�� ����Ʈ ��ȣ�� ���� ���� ����
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

        AllQuest[npcName] = quests; //npc �̸����� �� ��� ����Ʈ�� ���� �����Ѵ�.

        return quests; //npc �̸����� �� ����Ʈ�� ��ȯ�Ͽ� npc�� ����Ѵ�.
    }

    //�÷��̾� ����Ʈ ����Ʈ �� ������ ����Ʈ�� �̾Ƽ� �� ����Ʈ�� ���¸� �Ѱ��ش�.
    public QuestState GetQuestState(Quest npcQuests)
    {
        //playerQuests
        cuurentState = QuestState.Ready;
        currentQuest = npcQuests;

        //npc�� �������̰� ����Ʈ�� ������ �����Ѵٴ� ����
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

    //enemy kill �ϸ� ���� �������� kill quest�� ��� ã�Ƽ� currentAmmount++ 
    public void UpdateKillQuest(int enemyType)
    {
        foreach (Quest quest in performingQuest)
        {
            quest.EnemyKill(enemyType);
        }
    }

    //item�� ȹ�� �ϸ� ���� �������� gathering quest�� ��� ã�Ƽ� currentAmmount++ 
    public void UpdateCollectQuest(int itemIndex)
    {
        foreach (Quest quest in performingQuest)
        {
            quest.ItemCollect(itemIndex);
        }
    }

    public void RewardQuest()
    {
        //���� �ޱ�
        if (currentQuest.itemReward != null)
        {
            foreach (var item in currentQuest.itemReward)
            {
                Inventory.instance.AddItem(item);
            }
        }

        //PlayerStats.instance.AddGold(currentQuest.goldReward);
        Debug.Log(currentQuest.goldReward + " ��带 ȹ�� �Ͽ����ϴ�.");
        //PlayerStats.instance.AddExp(currentQuest.expReward);
        Debug.Log(currentQuest.expReward + " ����ġ�� ȹ�� �Ͽ����ϴ�.");

        //�÷��̾� ����Ʈ����Ʈ���� ����
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

