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

    //dialogIndex를 참조하여 대화창이 열림
    public int dialogIndex;

    public int level;

    //보상 목록
    public int goldReward;
    public int expReward;
    public Item itemReward;

    public QuestState questState;

    //하나의 퀘스트에 목표가 여러개 일 경우가 있다.
    //그러기 위해서는 QuestProgress을 따로 분리하여 관리 해줘야 한다.
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
