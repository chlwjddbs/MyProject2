using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Posion", menuName = "Posion")]
public class Potion : Item
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