using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class EquipItemSlot : MonoBehaviour , IPointerClickHandler
{
    private Inventory inven;
    private Equipment equipment;

    private InventoryUI invenUI;
    private EquipmentUI equipUI;

    //������ ����� �̹���
    public GameObject equipItemImage;
    //���õ� ���� �̹���
    public GameObject selectImage;

    //������ Ÿ��(����� ���⽽�Կ��� ���� �� �ְ� �ϱ� ����)
    public EquipType slotType;
    public int slotTypeNum;

    private Vector3 slotPos;
    private RectTransform slotRect;

    //���� �������� ������
    private EquipItem equipItem
    {
        get 
        { 
            if(equipment.equipItems[slotTypeNum] != null)
            {
                return equipment.equipItems[slotTypeNum];
            }
            else
            {
                return null;
            }
        }
    }

    private ItemInformation itemInformation;

    public void SetData(ItemInformation _itemInformation)
    {
        inven = Inventory.instance;
        equipment = Equipment.instance;
        slotTypeNum = (int)slotType;
        itemInformation = _itemInformation;
        invenUI = GetComponentInParent<InventoryUI>();
        equipUI = GetComponentInParent<EquipmentUI>();
        slotRect = GetComponent<RectTransform>();
    }

    public void SelectEquipSlot()
    {       
        if (equipment.equipItems[slotTypeNum] == null)
        {
            return;
        }

        selectImage.SetActive(true);
        slotPos = Camera.main.ViewportToScreenPoint(Camera.main.ScreenToViewportPoint(slotRect.position));
        itemInformation.SetDescription(equipItem, slotPos);
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
            invenUI.DeSelectAllSlots();
            equipUI.DeSelectAllSlots();
            //���� ������ �� ĭ�� �ƴ� ���� �۵�
            if (equipment.equipItems[slotTypeNum] != null)
            {
                UnEquipItemSlot();
                AudioManager.instance.PlayeSound("unEquipSound");
            }
        }
    }
}
