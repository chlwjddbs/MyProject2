using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Equip", menuName = "Equip")]
public class EquipItem : Item
{
    //����� Ÿ�� (����, ����, ���� ��)
    public EquipType equipType;

    //��� ���ݷ�
    public float attack;

    //��� ����
    public float defence;

    //��� ü��
    public float health;

    //��� ����
    public float mana;

    //���� ������ ���(�ΰ��ӿ� ǥ�õ� ��� ������Ʈ)
    public GameObject equipItemObject;

    //���� ������ ����� ��ġ ������( �ΰ��ӿ� ǥ�õǴ� ����� ��ġ)
    public Vector3 offset;

    //���� �� Ÿ�� ������ �ʿ��� �ݶ��̴��� mesh�� �޾ƿ´�.
    public Mesh mesh;

    //���⿡ ���� �⺻ ���� �ִϸ��̼�
    public AnimationClip attackClip;

    public override void Use(int _slotNum)
    {
        AudioManager.instance.PlayeSound(equipType.ToString() + "Sound");
        Equipment.instance.EquipItem(this, _slotNum);
    }
}

public enum EquipType
{
    Head,
    Weapon,
    Shield,
    Armor,
    Shoes,
    Ring,

    EquipTypeMax,
}
