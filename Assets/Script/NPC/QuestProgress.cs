using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestProgress
{
    public QuestType questType;

    public int reachedAmount; //Quest 목표량
    public int currentAmount; //Quest 진행량

    public int typeIndex; //아이템, enemy의 인덱스. ex) 뼈를 모아오는 퀘스트 진행시 뼈아이템의 아이템 인덱스를 통해 뼈가 수집됐음을 확인하여 currentAmount를 늘려준다.

    //퀘스트를 달성 했느냐?
    public bool isReached()
    {
        return currentAmount >= reachedAmount;
    }
}

public enum QuestType
{
    Kill,       //킬 퀘스트
    Gathering,  //수집 퀘스트
}