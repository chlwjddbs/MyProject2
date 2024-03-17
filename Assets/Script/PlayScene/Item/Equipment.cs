using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Equipment : MonoBehaviour
{
    public static Equipment instance;
    //��� ����ĭ�� ���� �� �� �ִ� ������ ���̴�.
    //���� ��� ����, �� , �Ź��� ���� �����ϴٸ� ��� ����ĭ�� 3�� ���̴�.
    public int equipmentSize = (int)EquipType.EquipTypeMax;
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(this);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public UnityAction<EquipItem> AddUpdateEquip;

    public UnityAction<EquipItem> RemoveUpdateEquip;

    public UnityAction UpdateDamage;

    //��� ����ĭ �迭�� ����
    public EquipItem[] equipItems;

    private Inventory inven;

    public Weapon weapon;
    public Shield shield;
    public PlayerStatus playerStatus;

    public UnityAction SetEquipData;

    
    public void SetData()
    {
        if(instance == null)
        {
            Debug.Log("����â �̱��� ���� �����");
            SetData();
            return;
        }

        if(SetEquipData == null)
        {
            SetData();
            return;
        }

        inven = Inventory.instance;
        //�迭�� ũ�⸦ ������ �� �ִ� ��� ������ ���� �����Ѵ�.
        equipItems = new EquipItem[equipmentSize];

        SetEquipData?.Invoke();

        if (DataManager.instance.newGame)
        {

        }
        else
        {
            LoadData();
        }
    }

    //EquipItem���κ��� ������ �������� �޾ƿ� �� ������ ������ ǥ��
    public void EquipItem(EquipItem newEquipItem, int slotNum)
    {
        //���� ������ ������ Ÿ��
        int newEquipType = (int)newEquipItem.equipType;

        //���� �������� �������� �ִٸ�
        if (equipItems[newEquipType] != null)
        {
            //���� �������� �������� �κ��丮�� �Ѱ��� �� 
            //inven.AddItem(equipItems[newEquipTyep]);
            inven.SwapItem(equipItems[newEquipType], slotNum);
            //�������� ��� Ż��
            UnEquipItem(newEquipType);
        }

        //���� ��� ���Կ� ���� ���� ������ ����
        equipItems[newEquipType] = newEquipItem;
        AddUpdateEquip?.Invoke(newEquipItem);

        //������ �������� ���� �� ��� 
        if(newEquipItem.equipType == EquipType.Weapon)
        {
            //���� ������ �Ѱ��ش�.
            weapon.EquipWeapon(newEquipItem);
        }
        //������ �������� ���� �� ���
        else if(newEquipItem.equipType == EquipType.Shield)
        {
            //shield.EquipShield(newEquipItem);
        }

        //playerStatus.equipDamage += newEquipItem.attack;
        //playerStatus.equipDefence += newEquipItem.defence;
        //playerStatus.CurrentStatus();
        playerStatus.Equip(newEquipType, equipItems[newEquipType]);
        //UpdateDamage �̺�Ʈ�� ���� ���� ���� ������Ʈ�� Equip���� ���Չ�.
        //UpdateDamage?.Invoke();
    }

    public void UnEquipItem(int _slotNum)
    {
        RemoveUpdateEquip?.Invoke(equipItems[_slotNum]);
        if(equipItems[_slotNum].equipType == EquipType.Weapon)
        {
            weapon.UnequipWeapon();
        }
        //playerStatus.equipDamage -= equipItems[_slotNum].attack;
        //playerStatus.equipDefence -= equipItems[_slotNum].defence;

        //����� ������ ���־�� �ϱ� ������ �⺻ ���� 0�� ���� �� �ֵ��� ��� ������ �Ѱ����� �ʴ´�.
        playerStatus.Unequip(_slotNum, equipItems[_slotNum]);
        equipItems[_slotNum] = null;
        //UpdateDamage?.Invoke();
    }
    
    public void SetEquip()
    {
        for (int i = 0; i < equipItems.Length; i++)
        {
            if(equipItems[i] != null)
            {
                AddUpdateEquip?.Invoke(equipItems[i]);

                //������ �������� ���� �� ��� 
                if (equipItems[i].equipType == EquipType.Weapon)
                {
                    //���� ������ �Ѱ��ش�.
                    //�ΰ��� �� ���� mesh ����
                    weapon.EquipWeapon(equipItems[i]);
                }
                //������ �������� ���� �� ���
                else if (equipItems[i].equipType == EquipType.Shield)
                {
                    //shield.EquipShield(newEquipItem);
                }

                //playerStatus.equipDamage += equipItems[i].attack;
                //playerStatus.equipDefence += equipItems[i].defence;
                playerStatus.Equip(i, equipItems[i]);
            }
        }
    }

    public void SaveData()
    {
        DataManager.instance.userData.equipmentItem = equipItems;
    }

    public void LoadData()
    {
        equipItems = DataManager.instance.userData.equipmentItem;
        SetEquip();
    }
}
