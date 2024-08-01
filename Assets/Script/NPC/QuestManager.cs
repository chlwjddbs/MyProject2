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
  
    public List<Quest> performingQuest = new List<Quest>();     //�������� ����Ʈ  
    public Quest currentQuest;                                  //���� ��ȣ�ۿ� ���� ����Ʈ ex) npc�� ����Ʈ�� �ο��ϴ� ��ȭ�� ���� �� �ο��� ��ü�� �Ǵ� ����Ʈ. ����Ʈ ���� ��ư�� �������� �������� ����Ʈ�� ��ϵǾ�� �ϴ� ����Ʈ. 
    public QuestState cuurentState;                             //���� ����Ʈ ����(����������, ���������� ���)
    public ItemManager itemManager;                             //����Ʈ ����� ������ ������ ����� ����Ʈ�� ����� ������ ����

    public List<Quest> completeQuest = new List<Quest>();       //�Ϸ��� ����Ʈ

    public Dictionary<string, XmlNodeList> NodeAll = new Dictionary<string, XmlNodeList>();     //����Ʈ ������ ����ִ� xml node
    public Dictionary<string, List<Quest>> AllQuest = new Dictionary<string, List<Quest>>();    //���� ���۽� ��ϵ� ����Ʈ

    private string codeStr; //����Ʈ ���� �������� Ȯ���ϱ� ���� string ����

    public UnityAction<bool> setQuestUI;
    public UnityAction<Quest> removeList;
    public UnityAction<Quest> loadData;

    public QuestSlot selectSlot; //QuestList���� ������ Quest ������ ��� �ִ� ����

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
            int i = 0;
            while (itemIndex > -1)
            {
                quest.itemReward.Add(itemManager.itemManage[itemIndex]); //�������� �����ϰ� �ִ� itemManager���� ������ ��ȣ(itemIndex)�� �������� �� �������� �����´�.
                i++;
                codeStr = $"item{i}";   //xml node�� ���� �������� ã�� ������ ���� node�� �˻��� �� �ְ� int i �� �ݺ����� �����ϰ� try���� ���� node�� ã�� �������� �ݸ��� �����Ѵ�.
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

    //�÷��̾� ����Ʈ ����Ʈ �� ������ ����Ʈ�� �̾Ƽ� �� ����Ʈ�� ���¸� �Ѱ��ش�.
    public QuestState GetQuestState(Quest npcQuest)
    {
        //playerQuests
        cuurentState = QuestState.Ready;
        currentQuest = npcQuest;

        //npc�� �������̰� ����Ʈ�� ������ �����Ѵٴ� ����
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

    //enemy kill �ϸ� ���� �������� kill quest�� ��� ã�Ƽ� currentAmmount++ 
    public void UpdateKillQuest(int enemyType)
    {
        foreach (Quest quest in performingQuest)
        {
            quest.EnemyKill(enemyType);
        }
    }

    //item�� ȹ�� �ϸ� ���� �������� gathering quest�� ��� ã�Ƽ� currentAmmount++ 
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
        //���� �ޱ�
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
        Debug.Log(currentQuest.goldReward + " ��带 ȹ�� �Ͽ����ϴ�.");
        player.AddExp(currentQuest.expReward);
        Debug.Log(currentQuest.expReward + " ����ġ�� ȹ�� �Ͽ����ϴ�.");

        currentQuest.questState = QuestState.Complete;
        completeQuest.Add(currentQuest);

        //�÷��̾� ����Ʈ����Ʈ���� ����
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

                inventory.CheckUseableSlot?.Invoke(item); //�κ��丮�� ���� �������� �޴� �Ҹ�ǰ�� ���� �Ҹ�ǰ�� �ְ�, ��ø �������� üũ.
                if (inventory.UseableSlot)
                {
                    inventory.AddItems(item); //�����ϸ� ���� ����
                }
                else
                {
                    if (inventory.isAdd) //��ø�� �������� ã�� �������� �κ��丮�� ��밡���ϸ� ���� ����
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
                inventory.CheckUseableSlot?.Invoke(item); //�κ��丮�� ���� �������� �޴� �Ҹ�ǰ�� ���� �Ҹ�ǰ�� �ְ�, ��ø �������� üũ.
                if (inventory.UseableSlot)
                {
                    inventory.AddItems(item); //�����ϸ� ���� ����
                }
                else
                {
                    if (inventory.isAdd) //��ø�� �������� ã�� �������� �κ��丮�� ��밡���ϸ� ���� ����
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
        else if(currentQuest.questProgress.questType == QuestType.Gathering) //���� ����Ʈ�� ��� ����Ʈ�� �ʿ��� �������� �κ��丮�� �̸� ���� �� �� �������� �κ��丮�� �ѹ� üũ�� �ش�.
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

