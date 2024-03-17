using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipItemSlot : MonoBehaviour , IPointerClickHandler
{
    private Inventory inven;
    private Equipment equipment;

    //������ ����� �̹���
    public GameObject equipItemImage;
    //���õ� ���� �̹���
    public GameObject selectImage;

    //������ Ÿ��(����� ���⽽�Կ��� ���� �� �ְ� �ϱ� ����)
    public EquipType slotType;
    public int slotTypeNum;

    //���� �������� ������
    private EquipItem equipItem;

    public void SetData()
    {
        inven = Inventory.instance;
        equipment = Equipment.instance;
        slotTypeNum = (int)slotType;
    }

    public void SelectEquipSlot()
    {       
        if (equipment.equipItems[slotTypeNum] == null)
        {
            return;
        }

        selectImage.SetActive(true);
    }

    public void UnEquipItemSlot()
    {
        inven.AddItem(equipment.equipItems[slotTypeNum]);
        equipment.UnEquipItem(slotTypeNum);
        /*
        equipItemImage.GetComponent<Image>().sprite = null;
        equipItemImage.SetActive(false);
        selectImage.SetActive(false);
        */
    }

    public void OnPointerClick(PointerEventData eventData)
    {

        if (eventData.button == PointerEventData.InputButton.Right)
        {
            //���� ������ �� ĭ�� �ƴ� ���� �۵�
            if (equipment.equipItems[slotTypeNum] != null)
            {
                UnEquipItemSlot();
                AudioManager.instance.PlayeSound("unEquipSound");
            }
        }
    }
}
