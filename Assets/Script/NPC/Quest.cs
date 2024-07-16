using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest
{
    public int number;
    public int npcIndex;
    public string qName;
    public string description;

    //dialogIndex�� �����Ͽ� ��ȭâ�� ����
    public int dialogIndex;                 //����Ʈ dialog����
    public int randomIndex;                 //����Ʈ ������ npc���� �ٽ� ���� �ɸ� ������ dialog�� ����
    public int completeIndex;               //����Ʈ �Ϸ� dialog

    public int level;                       //����Ʈ ���� ���� ����

    //���� ���
    public int goldReward;
    public int expReward;
    public List<Item> itemReward = new List<Item>();

    public QuestState questState;

    //�ϳ��� ����Ʈ�� ��ǥ�� ������ �� ��찡 �ִ�.
    //�׷��� ���ؼ��� QuestProgress�� ���� �и��Ͽ� ���� ����� �Ѵ�.
    public QuestProgress questProgress;


    public void EnemyKill(int enemyType)
    {
        Debug.Log(enemyType);
        if (questProgress.questType == QuestType.Kill && questProgress.typeIndex == enemyType)
        {
            questProgress.currentAmount++;
            if (questProgress.isReached())
            {
                questState = QuestState.Complete;
            }
        }
    }

    public void ItemCollect(int itemTpye)
    {
        if (questProgress.questType == QuestType.Gathering && questProgress.typeIndex == itemTpye)
        {
            questProgress.currentAmount++;
            if (questProgress.isReached())
            {
                questState = QuestState.Complete;
            }
        }
    }
}

public enum QuestState
{
    None,
    Ready,
    Accept,
    Complete,
}
