using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class Item : ScriptableObject
{
    //������ ���� ��ȣ
    public int itemNumber;

    //������ Ÿ��(���, ����, ���� ��)
    public ItemType itemType;

    //������ ���
    public ItemGrade itemGrade;

    //������ �̸�
    public string itemName;

    //������ ����
    public string description;

    //������ ����
    public float itemWeight;
    
    //������ �̹���
    public Sprite itemImege;

    //������ ���� ���԰�
    public int shopPrice;

    //������ ���� �Ű���
    public int sellPrice;


    //�ʵ忡�� ���̴� ������ ������Ʈ
    public GameObject FieldObject;

    public virtual void Use(int slotNum, Player player)
    {
        Debug.Log("������ ���");
    }
    
}

public enum ItemType
{
    Equip,          //���   
    Ingredient,     //���
    Used,           //�Ҹ�ǰ
    Key,            //����
    SkillBook,      //��ų��
    Etc,            //��Ÿ
}

public enum ItemGrade
{
    Common,
    Uncommon,
    Rare,
    Unique,
    Epic,
    Legendary,
    Mythic,
}