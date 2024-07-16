using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class QuestProgress
{
    public QuestType questType;

    public int reachedAmount; //Quest ��ǥ��
    public int currentAmount; //Quest ���෮

    public int typeIndex; //������, enemy�� �ε���. ex) ���� ��ƿ��� ����Ʈ ����� ���������� ������ �ε����� ���� ���� ���������� Ȯ���Ͽ� currentAmount�� �÷��ش�.

    //����Ʈ�� �޼� �ߴ���?
    public bool isReached()
    {
        return currentAmount >= reachedAmount;
    }
}

public enum QuestType
{
    Kill,       //ų ����Ʈ
    Gathering,  //���� ����Ʈ
}