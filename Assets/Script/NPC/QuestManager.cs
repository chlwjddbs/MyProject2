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

        //PickUpQuestGiver���� ����Ʈ ����Ʈ�� �޾ƿ����� QuestManager���� LoadQuestXml�� ���� ����Ǿ�� �Ѵ�.
        //�׷��� LoadQuestXml�� Start�� ������ PickUpQuestGiver���� ���� Start�� �� ���ɼ��� �ֱ� ������
        //Awake���� �����ϵ��� �Ѵ�.
        LoadQuestXml(xmlFile);
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

    private void LoadQuestXml(string fileName)
    {
        TextAsset xmlFile = Resources.Load(fileName) as TextAsset;

        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.LoadXml(xmlFile.text);
        allNodes = xmlDoc.SelectNodes("root/quest");
        Debug.Log("load xml file complete");
    }

    //������ �ε����� Npc���� ����Ʈ�� ȣ��
    public List<Quest> GetNpcQuest(int npcIndex)
    {
        List<Quest> quests = new List<Quest>();

        foreach (XmlNode node in allNodes)
        {
            int index = int.Parse(node["npc"].InnerText);
            if (index == npcIndex)
            {
                Quest quest = new Quest();
                //QuestProgress�� ����ȭ�� Ŭ���� �̱� ������ new�� �Ͽ� ��ü�� ������ ��� �����͸� ���� �� �ִ�.
                quest.questProgress = new QuestProgress();

                quest.number = int.Parse(node["number"].InnerText);
                quest.npcIndex = index;
                quest.qName = node["name"].InnerText;
                quest.description = node["description"].InnerText;
                quest.dialogIndex = int.Parse(node["diologIndex"].InnerText);
                quest.level = int.Parse(node["level"].InnerText);

                int qType = int.Parse(node["questType"].InnerText);
                //���1 : �޼��带 ���� QuestType �޾ƿ���
                //quest.questGoal.questType = QuestTypeIndex(qType);
                //���2 : node["questType"].InnerText�� ��Ʈ������ ��� QuestType���� �� ��ȯ
                quest.questProgress.questType = (QuestType)qType;
                quest.questProgress.currentAmount = 0;
                quest.questProgress.reachedAmount = int.Parse(node["goalAmount"].InnerText);

                //�������̳� ��ǥ ������ �ε����� �޾ƿ� ó���ϵ��� �Ѵ�.
                //ex) ���� ��� ���� ����Ʈ�� ����� ������ �ε����� 0�̰� ����Ʈ�� �ε����� 1�̸�
                // �ε����� 0�� ���� ī��Ʈ�� �� �� �ְ� �Ѵ�.
                quest.questProgress.typeIndex = int.Parse(node["goalIndex"].InnerText);

                quest.goldReward = int.Parse(node["gold"].InnerText);
                quest.expReward = int.Parse(node["exp"].InnerText);

                //node�� item ��ȣ�� �޾ƿ� rewardItem�� ����Ʈ ��ȣ�� ���� ���� ����
                int itemIndex = int.Parse(node["item"].InnerText);
                //������ ��ȣ�� -1�̸� �������� �������� ����. ����null
                if (itemIndex > -1)
                {
                    quest.itemReward = rewardItem[itemIndex];
                }
                quests.Add(quest);
            }
        }

        return quests;
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
            Inventory.instance.AddItem(currentQuest.itemReward);
        }

        //PlayerStats.instance.AddGold(currentQuest.goldReward);
        Debug.Log(currentQuest.goldReward + " ��带 ȹ�� �Ͽ����ϴ�.");
        //PlayerStats.instance.AddExp(currentQuest.expReward);
        Debug.Log(currentQuest.expReward + " ����ġ�� ȹ�� �Ͽ����ϴ�.");

        //�÷��̾� ����Ʈ����Ʈ���� ����
        performingQuest.Remove(currentQuest);
    }
}

