using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    private Inventory inven;
    private ControllOption controllOption;

    //���� �κ��丮 â���� ���� ������Ʈ
    public GameObject InvenUI;

    //�κ��丮UI On,Off ����
    private RectTransform InvenUIrect;
    private bool isOpen = false;

    private EquipmentUI equipUI;

    //������ ���Ե��� ������ �ִ� �θ������Ʈ Items�� �޾ƿ�
    public Transform Items;

    public TextMeshProUGUI goldText;

    //Items���� ������ ���Ե��� ������ �迭 ����
    [HideInInspector] public ItemSlot[] itemSlot;
    public ItemInformation itemInformation;

    //���� ���õ� ������ ��ȣ
    public int slotNum = -1; //-1 : ���õ� ������ ��ȣ�� ���� ���

    public invenState invenState = invenState.stay;

    public GameObject Player;

    public Sound[] InvenSound;

    public KeyOption keyOption;

    private void Awake()
    { 
        Inventory.instance.SetUIData += SetUIData;
    }

    private void Start()
    {
        //CloseUI();
        foreach (var s in InvenSound)
        {
            AudioManager.instance.AddExternalSound(s);
        }
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        if (!GameData.instance.isSet)
        {
            return;
        }
        if (GameSceneMenu.isMenuOpen)
        {
            return;
        }

        ToggleUI();
    }

    /*
    //�������� ȹ���ϴ� �� �κ��丮�� ��ȭ�� ���� �� �κ��丮 ������Ʈ
    public void UpdateInven()
    {
        //�������� ���Ž� ������ ī��Ʈ ��ŭ ������ ���� ������ ������ ���Ա��� �������� ���Ѵ�.
        //������� 8���� ������ �� �ϳ��� �������� �����ϸ� ������ ī��Ʈ�� 7�̵ȴ�.
        //�׷��� ������ ī��Ʈ ��ŭ ������ ������ �Ǹ� 7��° ���Ա��� ������ ����
        //���ŵ� 8��° ������ ���ŵ��� ���� UI�󿡼��� ���� �ϴ°ɷ� �ν��ϰ� �ȴ�.
        //�׷��Ƿ� ��ü �ʱ�ȭ(itemSlot.Length)�� �����ϰų�
        //������ ī��Ʈ +1(inven.items.Count+1)��ŭ �ʱ�ȭ�� ���� ������� �Ѵ�.
        for (int i = 0; i < inven.items.Count + 1; i++)
        {
            itemSlot[i].RemoveItemSlot();
            AllSlotsReset();
        }

        //�κ��丮�� �ִ� ������ ��ŭ �ٽ� ���
        for (int i = 0; i < inven.items.Count; i++)
        {
            itemSlot[i].SetItemSlot();
        }
    }
    */

    public void SetUIData()
    {
        inven = Inventory.instance;
        controllOption = OptionManager.instance.controllOption;
        equipUI = GetComponent<EquipmentUI>();
        InvenUIrect = InvenUI.GetComponent<RectTransform>();
        itemInformation.SetData();
        CloseUI();

        //Items�� �ڽĵ� = itemSlot�� ����
        //Items�� �ڽĵ� �� ItemSlot�� ã�� �迭�� ���� 
        itemSlot = Items.GetComponentsInChildren<ItemSlot>();

        //�κ��丮 ������� �κ��丮 ������ ����
        inven.invenSize = itemSlot.Length;

        //���� ��ȣ ����
        for (int i = 0; i < itemSlot.Length; i++)
        {
            //�Ű������� ���� OnClick �̺�Ʈ 
            //itemSlot[i].GetComponent<Button>().onClick.AddListener(() => SelectSlot(i));
            int slotNum = i;
            //�Ű� ������ �ִ� �޼��� ����� ���� �Լ��� �̿��Ͽ� ����
            //itemSlot[i].GetComponent<Button>().onClick.AddListener(() =>SelectSlot(index));
            //delegate �Լ� �̿�

            itemSlot[i].GetComponent<Button>().onClick.AddListener(delegate { SelectSlot(slotNum); });
            itemSlot[i].slotNum = i;
            itemSlot[i].SetData(itemInformation);
        }

        //�κ��丮 ������Ʈ �׼��Լ��� ���
        inven.AddUpdateUI += AddItemUI;
        inven.UpdateGoldUI += UpdateGoldUI;
        inven.RemoveUpdateUI += DeSelectSlot;
        inven.CheckUseableSlot += CheckSlot;
        inven.GoodsOverlap += GoodsOverlap;
        inven.ConsumeQuestItems += ConsumeQuestItem;

        inven.LoadSlot += LoadSlot;
    }

    public void LoadSlot()
    {
        for (int i = 0; i < inven.invenItems.Count; i++)
        {
            if(inven.invenItems[i].slotItem != -1)
            {
                itemSlot[i].LoadData(GameData.instance.userData.invenItem[i].quantity);
            }
        }
    }

    public void ToggleUI()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isOpen)
            {
                //InvenUI.SetActive(false);
                DeSelectAllSlots();
                CloseUI();
                AudioManager.instance.PlayExternalSound("InvenClose");
            }
        }
        if (Input.GetKeyDown(controllOption.bindKey_Dic[keyOption].bindKey))
        {
            //�κ��丮�� ���� ������ �پ��� ���װ� �߻��Ͽ� ��ġ�� �ű�� ������� ����
            /*InvenUI.SetActive(!InvenUI.activeSelf);          
            if (InvenUI.activeSelf == false)
            {
                DeSelectAllSlots();
            }
            */

            isOpen = !isOpen;
            if (isOpen)
            {
                InvenUIrect.anchoredPosition = new Vector3(0, 0, 0);
                AudioManager.instance.PlayExternalSound("InvenOpen");
            }
            else
            {
                DeSelectAllSlots();
                InvenUIrect.anchoredPosition = new Vector3(1920f, 0, 0);
                AudioManager.instance.PlayExternalSound("InvenClose");
            }
        }
    }

    public void AddItemUI(int _slotNum)
    {
        itemSlot[_slotNum].SetItemSlot();
    }

    public void UpdateGoldUI(int _gold)
    {
        goldText.text = string.Format("{0 : #,###}", _gold);
    }

    //�κ��丮�� �߰��Ǵ� �������� ��ø ������ �������� �� ��ø �������� üũ
    public void GoodsOverlap(int _slotNum, Item _item)
    {
        if (_item.itemType == ItemType.Used || _item.itemType == ItemType.Ingredient)
        {
            inven.isOverlap = itemSlot[_slotNum].PotionOverlapable();
        }
    }

    //�������� �� ������ ���� üũ
    public void CheckSlot(Item _receiveItem)
    {
        for (int i = 0; i < inven.invenSize; i++)
        {
            //�������� ����ִ� ���� �߰� ��
            if (itemSlot[i].HavedItem() != null)
            {
                //���Կ� ��� �ִ� �������� ȹ���� �����۰� ���� ��ø ������ �����̸�
                if (_receiveItem == itemSlot[i].HavedItem() && itemSlot[i].PotionOverlapable() == true)
                {
                    //���� ��ø ����
                    invenState = invenState.able;
                    inven.UseableSlot = true;
                    break;
                }

                //���Կ� ��� �ִ� �������� ���� �����۰� ������ ��ø�� �Ұ����� ���¶��
                else if (_receiveItem == itemSlot[i].HavedItem() && itemSlot[i].PotionOverlapable() == false)
                {
                    //for���� ���� ������ ��� ������ �˻��ϰ� �ȴ�.
                    //�׶� ���� ������ true ������ ���� �ڿ� ������ false�� �Ǹ�
                    //�κ��丮�� �ش� �������� ��ø ������ ���������� false�� ��ȯ�Ͽ� ��ø�� �Ұ����ϴٰ� �Ǵ� �� �� �ִ�.
                    //�׷��� ������ �ռ� �������� true ó���� �޴´ٸ� continue�� ���� �ǳʶپ�
                    //�ѹ� ���� true ������ false�� �ٲ��� �ʵ��� ���ش�.
                    if (invenState == invenState.able)
                    {
                        continue;
                    }
                    //�κ��丮 ������ ���ص� ��ø ������ �κ��丮 ������ ������ ��ø �Ұ���(false) ����
                    else
                    {
                        invenState = invenState.enable;
                        inven.UseableSlot = false;
                    }
                }
            }
            //���� �̸��� �������� ���ٸ� ó�� ȹ���ϴ� ���������� inventory���� �� ������ ã���� ��.
            else
            {
                inven.UseableSlot = false;
            }
        }
    }

    public void ConsumeQuestItem(int _slotNum, int _quantity)
    {
        itemSlot[_slotNum].ConsumeQuestItem(_quantity);
    }

    /*
    public void RemoveItemUI(int _slotNum, Item _item)
    {
        itemSlot[_slotNum].RemoveItemSlot();
    }
    */


    public void CloseUI()
    {
        //InvenUI.SetActive(false);
        DeSelectAllSlots();     
        isOpen = false;
        InvenUIrect.anchoredPosition = new Vector3(1920f, 0, 0);
    }


    //���� ����
    public void SelectSlot(int _slotNumber)
    {
        equipUI.DeSelectAllSlots();

        //�������� ���� ������ �ѹ��� -1�̴�.
        //�׷���� _slotNumber�� �޾ƿ� ��� ���� ������ ������ ���� ������ �� �������� ����Ǿ� �������� ��� ������ �����ߴٰ� �����Ѵ�.
        //�� ������ �������� �� �ڸ��� �������� �޾ƿ��� �������� �����ϸ� �����̹����� �ٷ� �������� �ʴ´�.

        //�������� ���� ���� ���ý� ���� ���� ������ ������ �ʴ´�.
        if (itemSlot[_slotNumber].HavedItem() == null)
        {
            DeSelectAllSlots();
            return;
        }

        //���� ���õ� ���԰� ���� ������ ������ ���� ���
        if (slotNum == _slotNumber)
        {
            DeSelectSlot(slotNum);
            //���� �̹������ְ� ���õ� ���� �ʱ�ȭ
            //itemSlot[slotNum].selectImage.SetActive(false);
            //slotNum = -1;
        }
        //�ٸ� ������ ������ ���
        else
        {
            //���� ������ ������ ������
            if (slotNum != -1)
            {
                //���� ������ ������ ���� �̹��� ����
                itemSlot[slotNum].selectImage.SetActive(false);
            }

            //���� ������ ������ ���� �������� ��ȯ
            slotNum = _slotNumber;
            //slotNumber = itemSlot[_slotNumber].slotNum;

            //���� ������ ���� �̹��� Ȱ��ȭ
            itemSlot[slotNum].SelectSlot();
        }
    }

    public void DeSelectSlot(int _slotNumber)
    {
        if(slotNum != -1)
        {
            itemSlot[slotNum].selectImage.SetActive(false);
        }

        itemSlot[_slotNumber].selectImage.SetActive(false);
        slotNum = -1;
        itemInformation.gameObject.SetActive(false);
    }

    //��� ���� ����
    public void DeSelectAllSlots()
    {
        if (slotNum != -1)
        {
            itemInformation.gameObject.SetActive(false);
        }

        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i].selectImage.SetActive(false);
            slotNum = -1;
        }
    }

    public bool UIOpenCheck()
    {
        return isOpen;
    }
}


public enum invenState
{
    stay,   //�ݺ����� ���� able�� enable ���¸� �����ϱ� ������ �ݺ��� �� ������� ����
    able,   //�κ��丮 ���� ȹ�� �� �������� �� ������ �ִٰ� ����.
    enable, //�κ��丮 ���� ȹ�� �� �������� �� ������ ���ٰ� ����.
}
