using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EquipmentUI : MonoBehaviour
{
    private Equipment equipment;

    //����â UI
    public GameObject equipUI;
    private RectTransform equipUIrect;
    private bool isOpen;

    private InventoryUI invenUI;

    //���������� �����ϴ� �θ� ������Ʈ
    public Transform equipItems;

    //������ ��� ����Ǵ� ��� ����
    private EquipItemSlot[] equipSlot;

    private EquipItem equipItem;

    
    //������ ����� ���� ��ġ
    public Transform weaponSlot;
    public Transform shieldSlot;

    //���õ� ������ ��ȣ
    public int slotNum = -1;

    private Vector3 resetPos;

    public RectTransform levelTitle;

    private void Awake()
    {
        Equipment.instance.SetEquipData = SetData;
        LanguageOption.SortingUI += SortingUI;
        PlayerStatus.stausUI += SortingUI;
    }

    // Start is called before the first frame update
    void Start()
    {
        resetPos = new Vector3(0, 0, 0);
        
        //CloseUI();       
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!DataManager.instance.isSet)
        {
            return;
        }
        if (GameSceneMenu.isMenuOpen)
        {
            return;
        }

        ToggleUI();
        /*
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            equipUI.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.U))
        {
            equipUI.SetActive(!equipUI.activeSelf);

            if(equipUI.activeSelf == false)
            {
                DeSelectAllSlots();
            }
        }
        */
    }

    public void SetData()
    {
        equipment = Equipment.instance;
        equipSlot = equipItems.GetComponentsInChildren<EquipItemSlot>();

        invenUI = GetComponent<InventoryUI>();
        equipUIrect = equipUI.GetComponent<RectTransform>();

        CloseUI();

        for (int i = 0; i < equipSlot.Length; i++)
        {
            int _slotNum = i;
            //0�� ������ ��ư �̺�Ʈ�� SelectSlot �޼��� ���. ������ ��ȣ�� ������ ������Ÿ��
            equipSlot[i].GetComponent<Button>().onClick.AddListener(delegate { SelectEquipSlot(_slotNum); });
            equipSlot[i].SetData();
        }

        equipment.AddUpdateEquip += AddEquipItem;
        equipment.RemoveUpdateEquip += RemoveEquipItem;
    }

    public void SortingUI()
    {
        RectTransform[] rec = levelTitle.GetComponentsInChildren<RectTransform>();
        for (int i = 0; i < rec.Length; i++)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rec[i]);
        }
    }

    public void ToggleUI()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DeSelectAllSlots();
            CloseUI();
        }
        

        if (Input.GetKeyDown(KeyCode.M))
        {
            isOpen = !isOpen;
            if (isOpen)
            {
                equipUIrect.anchoredPosition = new Vector3(0, 0, 0);
            }
            else
            {
                DeSelectAllSlots();
                equipUIrect.anchoredPosition = new Vector3(0, 1080f, 0);
            }
        }
    }
    public void CloseUI()
    {
        //equipUI.SetActive(false);
        DeSelectAllSlots();
        isOpen = false;
        equipUIrect.anchoredPosition = new Vector3(0, 1080f, 0);
    }

    //������ ���� �� ���� ������Ʈ(���� �̹��� �� ���� ��� ����)
    public void AddEquipItem(EquipItem _equipItem)
    {
        //������ ������ Ÿ���� int������ ����
        int equipType = (int)_equipItem.equipType;

        //������ ��� Ÿ�Կ� �´� ��� ������ ã�´�.
        for (int i = 0; i < equipSlot.Length; i++)
        {
            //equipSlot[i].slotNum : i��° equipSlot�� slotNum(����Ÿ�Գѹ�)
            //���� ���� ���� ��� ����(1��)�̸� equipSlot[1].slotNum �϶� True�� �Ǿ� ����1���� ���� �����ϰ� �ȴ�.
            if (equipSlot[i].slotTypeNum == equipType)
            {
                //��� �̹��� �����ֱ�
                equipSlot[i].equipItemImage.SetActive(true);
                equipSlot[i].equipItemImage.GetComponent<Image>().sprite = _equipItem.itemImege;             
            }
        }

        //�����ϴ� ��� ���� ����ȭ�鿡�� ����
        if(equipType == (int)EquipType.Weapon)
        {
            GetComponent<PlayerStatusUI>().EquipWeapon(_equipItem.itemImege);
            GameObject weapon = Instantiate(_equipItem.equipItemObject, weaponSlot.transform.position , Quaternion.identity, weaponSlot);
            //ĳ���Ͱ� ��� ��� ������ �ڿ������� ���̱� ���� ����� rotate offset �߰�
            weapon.transform.localPosition = resetPos;
            weapon.transform.localEulerAngles = resetPos;
            weaponSlot.transform.localScale = weapon.transform.localScale;
            weapon.transform.localScale = new Vector3(1, 1, 1);
            weaponSlot.transform.localEulerAngles = _equipItem.offset;
        }
        else if(equipType == (int)EquipType.Shield)
        {
            GameObject shield = Instantiate(_equipItem.equipItemObject, shieldSlot.transform.position, Quaternion.identity, shieldSlot);
            shield.transform.localPosition = resetPos;
            shield.transform.localEulerAngles = resetPos;
            shieldSlot.transform.localEulerAngles = _equipItem.offset;
        }
    }

    public void RemoveEquipItem(EquipItem _equipItem)
    {
        
        //Ż���� ������ Ÿ���� int������ ����
        int equipType = (int)_equipItem.equipType;

        //Ż���� ��� Ÿ�Կ� �´� ��� ������ ã�´�. ex) ����� ���⽽�� 
        for (int i = 0; i < equipSlot.Length; i++)
        {
            //��� ���԰� ��� Ÿ���� �´´ٸ� ex) ���⽽�� == ����Ÿ��           
            if (equipSlot[i].slotTypeNum == equipType)
            {
                //��� �̹��� ���� 
                equipSlot[i].equipItemImage.GetComponent<Image>().sprite = null;
                equipSlot[i].equipItemImage.SetActive(false);            

                //��񽽷��� ���õǾ� ������ ���� �̹��� �ʱ�ȭ
                if (equipSlot[i].selectImage.activeSelf == true)
                {
                    equipSlot[i].selectImage.SetActive(false);
                }
            }
        }

        //���� ���ӳ������� Ż���� ��� ����
        if (equipType == (int)EquipType.Weapon)
        {
            //���� ���� ����UI �̹��� ����
            GetComponent<PlayerStatusUI>().UnEquipWeapon();

            //weaponSlot�� �ڽ� ������Ʈ(�������� ���)�� �����´�.
            Transform[] weapons = weaponSlot.GetComponentsInChildren<Transform>();

            //GetComponentsInChildren ���� �θ� ������Ʈ�� ������ �� �ֱ� ������ �±׸� ����Ͽ� �ɷ��ش�.
            foreach (var weapon in weapons)
            {
                if (weapon.CompareTag("Item"))
                {
                    Destroy(weapon.gameObject);
                }
            }
            weaponSlot.transform.localEulerAngles = resetPos;
            weaponSlot.transform.localScale = new Vector3(1, 1, 1);
        }
        else if (equipType == (int)EquipType.Shield)
        {
            Transform[] shields = shieldSlot.GetComponentsInChildren<Transform>();

            foreach (var shield in shields)
            {
                if (shield.CompareTag("Item"))
                {
                    Destroy(shield.gameObject);
                }
            }
            shieldSlot.transform.localEulerAngles = resetPos;
            shieldSlot.transform.localScale = new Vector3(1, 1, 1);
        }

        //���� �ʱ�ȭ
        slotNum = -1;
    }

    public void SelectEquipSlot(int _slotNumber)
    {
        invenUI.DeSelectAllSlots();

        //�������� �����ϸ� ������ �̹����� ������ = �������� ������ ������ �̹����� �����ִ�.
        //�������� ������ ��񽽷��� �������� ���ϰ� �Ѵ�.
        if (equipSlot[_slotNumber].equipItemImage.activeSelf == false)
        {
            Debug.Log("������� x");
            DeSelectAllSlots();
            return;
        }
        

        //���� ���õ� ���԰� ���� ������ ������ ���� ���
        if (slotNum == _slotNumber)
        {
            //���� �̹������ְ� ���õ� ���� �ʱ�ȭ
            equipSlot[slotNum].selectImage.SetActive(false);
            slotNum = -1;
        }
        //�ٸ� ������ ������ ���
        else
        {
            //���� ������ ������ ������
            if (slotNum != -1)
            {
                //���� ������ ������ ���� �̹��� ����
                equipSlot[slotNum].selectImage.SetActive(false);
            }

            //���� ������ ������ ���� �������� ��ȯ
            slotNum = _slotNumber;
            //slotNumber = itemSlot[_slotNumber].slotNum;

            //���� ������ ���� �̹��� Ȱ��ȭ
            equipSlot[slotNum].SelectEquipSlot();
        }
    }

    public void DeSelectAllSlots()
    {
        for (int i = 0; i < equipSlot.Length; i++)
        {
            equipSlot[i].selectImage.SetActive(false);
            slotNum = -1;
        }
    }

    public bool UIOpenCheck()
    {
        return isOpen;
    }

    private void OnDestroy()
    {
        LanguageOption.SortingUI -= SortingUI;
        PlayerStatus.stausUI -= SortingUI;
    }
}
