using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Used", menuName = "Item/Used")]
public class Used : Item
{
    public PotionTpye potionTpye;

    //���� �ִ���� �ѵ�
    public int ownershipLimit;

    //���� ���� ����
    public int quntity = 0;

    //public GameObject potionItemObject;

}

public enum PotionTpye
{
    Hp,         //HP ȸ�� ����
    Mp,         //MP ȸ�� ����
    Detoxion,   //�����̻� ���� ����
    Buff,       //���� ����
}