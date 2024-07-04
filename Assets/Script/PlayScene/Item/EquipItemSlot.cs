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

    //장착한 장비의 이미지
    public GameObject equipItemImage;
    //선택된 슬롯 이미지
    public GameObject selectImage;

    //슬롯의 타입(무기는 무기슬롯에만 들어올 수 있게 하기 위함)
    public EquipType slotType;
    public int slotTypeNum;

    private Vector3 slotPos;
    private RectTransform slotRect;

    //현재 장착중인 아이템
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
            //현재 슬롯이 빈 칸이 아닐 때만 작동
            if (equipment.equipItems[slotTypeNum] != null)
            {
                UnEquipItemSlot();
                AudioManager.instance.PlayeSound("unEquipSound");
            }
        }
    }
}
