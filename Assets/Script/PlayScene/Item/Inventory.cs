using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Inventory : MonoBehaviour
{
    [System.Serializable]
    public class InvenItem
    {
        //Item�� ScriptableObject�� ������� �ִ�.
        //�׷��� ������ ���������� �������� ���� �� �� ���� ������ �κ��丮 ���������� ���� ������ ������ش�.
        public Item slotItem;
        public int quantity;

        public InvenItem(Item _item, int _quantity = 0, bool _isTemp = false)
        {
            slotItem = _item;

            if (slotItem == null)
            {
                quantity = 0;
            }

            if (_isTemp)
            {
                quantity = _quantity;
            }
        }
    }

    public static Inventory instance;

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(this);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private GameData gameData;

    //�κ��丮 ������Ʈ �� ����� �׼� �Լ� ���
    //public UnityAction<int, Item> UpdateInventory;
    public UnityAction<int> AddUpdateUI;

    public UnityAction<int> RemoveUpdateUI;

    public UnityAction<int> UpdateGoldUI;

    //public List<Item> items = new List<Item>();
    public int invenSize;
    //�������� ���� ���Թ�ȣ
    public int slotNum = -1;

    //��ųʸ��� ���̾��Ű���� �� �� �����Ƿ� ���� ������ ����� �� �� �ְ� �迭�� public���� ������ش�.
    //��ųʸ����� ���ϰ� ������ �� �ֱ� ������ ����� �Ÿ� ���������.  ex)items[slotNum] == inventiems[slotNum].slotItem;
    public Item[] items;
    //�ٷ� �������� ���� �迭
    //public int[] quantityCheck;

    //�÷��̰� ����� �� �ִ� ȭ��;
    public int gold;

    //������� �κ��丮 ������ ����
    private int useSlot = 0;
    //�κ��丮�� �������� �߰� �� �� �ֳ���?
    public bool isAdd = true;

    //��ø ������ ������ ȹ�� �� ������ �����۰� ���� �������� �ִ� ���԰� ��ø ���� ������ ���� ���� üũ
    public UnityAction<Item> CheckUseableSlot;
    public bool UseableSlot = false;

    //��ø ������ �������� ��ø�Ǿ�� �ϴ���, ��ø�� �Ұ��� �Ͽ� ���ο� ������ �����ؾ� �ϴ��� üũ
    public UnityAction<int, Item> GoodsOverlap;
    public bool isOverlap = false;

    //inventoryUI�� set�Լ��� ����
    public UnityAction SetUIData;
    //Inventory load data�� ItemSlot�� ����
    public UnityAction LoadSlot; 

    public Dictionary<int, InvenItem> invenItems = new Dictionary<int, InvenItem>();

    /*
    private void Start()
    {
        if (DataManager.instance.newGame)
        {
            Debug.Log("new Game");
            //invenSize = 25;
            spareSlot = 0;
            items = new Item[invenSize];
            InventorySpace();
        }
    }
    */

    /*
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
        {
            foreach (KeyValuePair<int,InvenItem> item in invenItems)
            {
                Debug.Log($"{item.Key}�� ���� : {item.Value.slotItem.itemName}�� ���� = {item.Value.quantity}��");
            }
        }
    }
    */

    public void SetData()
    {
        if (instance == null)
        {
            SetData();
            Debug.Log("�κ��丮 �̱��� ���� �����");
            return;
        }

        if (SetUIData == null)
        {
            SetData();
            Debug.Log("�κ��丮 UI ������ �غ���");
            return;
        }

        gameData = GameData.instance;

        SetUIData.Invoke();

        if (gameData.newGame)
        {
            invenSize = 25;
            useSlot = 0;
            items = new Item[invenSize];
            gold = 150;
            UpdateGoldUI?.Invoke(gold);

            for (int i = 0; i < invenSize; i++)
            {
                invenItems.Add(i, new InvenItem(null));
            }

            InventorySpace();
        }
    }

    public void LoadData()
    {
        invenSize = gameData.userData.invenSize;
        useSlot = gameData.userData.useSlot;
        items = new Item[gameData.userData.invenItem.Count];
        gold = gameData.userData.gold;
        UpdateGoldUI?.Invoke(gold);
        for (int i = 0; i < invenSize; i++)
        {
            invenItems.Add(i, new InvenItem(gameData.userData.invenItem[i].slotItem));
            items[i] = invenItems[i].slotItem;
        }
        LoadSlot.Invoke();

        InventorySpace();
    }
    public void SaveData()
    {
        //GameData.instance.userData.inventoryItem = items;
        GameData.instance.userData.invenSize = invenSize;
        GameData.instance.userData.useSlot = useSlot;
        GameData.instance.userData.gold = gold;

        for (int i = 0; i < invenSize; i++)
        {
            if (GameData.instance.userData.slotNum.Contains(i))
            {
                GameData.instance.userData.invenItem[i] = invenItems[i];
            }
            else
            {
                GameData.instance.userData.slotNum.Add(i);
                GameData.instance.userData.invenItem.Add(invenItems[i]);
            }
        }

        /*
        DataManager.instance.userData.inventoryItem = new Item[invenSize];
        for (int i = 0; i < invenSize; i++)
        {
            DataManager.instance.userData.inventoryItem[i] = items[i];
        }
        */
    }

    public void AddItem(Item _item)
    {
        for (int i = 0; i < items.Length; i++)
        {
            if(items[i] == null)
            {
                //items[i] = _item;
                slotNum = i;
                invenItems[slotNum].slotItem = _item;
                items[slotNum] = invenItems[slotNum].slotItem;
                AddUpdateUI?.Invoke(slotNum);

                //�������� ������ �κ��丮�� �Ҹ��� ����
                useSlot += 1;
                InventorySpace();
                break;
            }
        }
        //items.Add(_item);
        //�׼��Լ��� ��ϵǾ� ���� �� ����
        //UpdateInventory?.Invoke(slotNum, items[slotNum]);
    }

    //���� ������ �����
    public void AddPotion(Item _potion)
    {
        //������ �������̶� ������ ���� �ְ�, ��ø ������ ���� ������ ���� ���� üũ
        CheckUseableSlot?.Invoke(_potion);
        //Debug.Log(UseableSlot);
        //������ ���� �ִٸ�
        if (UseableSlot)
        {
            for (int i = 0; i < items.Length; i++)
            {
                //�������� ���� ������ �н�
                if(items[i] == null)
                {
                    continue;
                }

                //���� �������� �ִ� ���� ã��
                else if (items[i].itemName == _potion.itemName)
                {
                    //���� ������ �������� ���� ���Կ� ��ø �������� üũ
                    GoodsOverlap?.Invoke(i,_potion);

                    //�����ϸ� ������ ��ø
                    if (isOverlap)
                    {
                        AddUpdateUI?.Invoke(i);
                        //Debug.Log("��ø");
                        break;
                    }

                    //�Ұ��� ��ø ������ ���� ������ ã�´�.
                    //��ü ���Կ��� ��ø ������ ������ ���� �Ҽ� ����.
                    //CheckUseableSlot?.Invoke(_potion.itemName); ���� ��ø ������ ������ ���� ������ �����߱� ����.
                }
            }
        }
        //������ ���� ���ų�, ������ ���� �����Ͽ��� ��ø���� ������ ���� ���� �ʾҴٸ�
        else
        {
            //�� ���Կ� ������ �߰�
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                {
                    //items[i] = _potion;
                    slotNum = i;
                    invenItems[slotNum].slotItem = _potion;
                    items[slotNum] = invenItems[slotNum].slotItem;
                    AddUpdateUI?.Invoke(slotNum);

                    //�������� ������ �κ��丮�� �Ҹ��� ����
                    useSlot += 1;
                    InventorySpace();

                    Debug.Log("������ �߰�");
                    break;
                }
            }
        }

        //��� ������ �κ��丮�� �ݵ��� �����Ѵ�. �ֳ��ϸ� �������� �κ��丮�� ����ϱ���, ����ܰ迡�� ȹ�� ���� ���θ� üũ�ϱ� ����
    }

    public void AddGold(int _gold)
    {
        gold += _gold;
        UpdateGoldUI?.Invoke(gold);
    }

    public void UseGold(int _gold)
    {
        if(gold < _gold)
        {
            Debug.Log("�������� �����մϴ�.");
            return;
        }

        gold -= _gold;
        UpdateGoldUI?.Invoke(gold);
    }

    public void SwapItem(Item _item, int _slotNum)
    {
        items[_slotNum] = _item;
        slotNum = _slotNum;
        AddUpdateUI?.Invoke(slotNum);
    }

    public void RemoveItem(int _slotNum)
    {
        RemoveUpdateUI?.Invoke(_slotNum);
        items[_slotNum] = null;
        invenItems[_slotNum].slotItem = null;

        slotNum = -1;

        //�������� �����ϸ� �κ��丮 �Ҹ��� ����
        useSlot -= 1;
        InventorySpace();
        //items.Remove(_item);
        //UpdateInventory?.Invoke(slotNum, items[slotNum]);
    }
    
    public void InventorySpace()
    {
        //�κ��丮�� invenSize(�κ��丮 �ѵ�)�� �����ߴ��� üũ
        if(useSlot == invenSize)
        {
            isAdd = false;
        }
        else
        {
            isAdd = true;
        }
    }

    /*
    public void RemoveItem(Item _item)
    {
        for (int i = 0; i < invenSize; i++)
        {
            if(items[i].itemNumber == _item.itemNumber)
            {
                items[i] = null;
                slotNum = i;
                RemoveUpdateInventory?.Invoke(slotNum, items[slotNum]);
                break;
            }
        }
        //items.Remove(_item);
        //UpdateInventory?.Invoke(slotNum, items[slotNum]);
    }
    */
}

/*
            if(DataManager.instance.userData.slotNum.Contains(i))
            {
                Debug.Log(i);
                int test1= 0;
                for (int j = 0; j < DataManager.instance.userData.slotNum.Count; j++)
                {
                    if (DataManager.instance.userData.slotNum.Contains(i))
                    {
                        test1 = j;
                        break;
                    }    
                }
                invenItems.Add(i, DataManager.instance.userData.invenItem[test1]);
            }
            else
            {
                invenItems.Add(i, null);
            }

        for (int i = 0; i < invenSize; i++)
        {
            if (DataManager.instance.userData.invenItem != null) 
            {              
                invenItems.Add(i, DataManager.instance.userData.invenItem[i]);
            }
            else
            {
                invenItems.Add(i, null);
            }
        }
        */

    //Debug.Log(DataManager.instance.userData.invenItems[0].slotItem.itemName);
    //invenItems = new Dictionary<int, InvenItem>(DataManager.instance.userData.invenItems);

    /*
    for (int i = 0; i < items.Length; i++)
    {
        if (items[i] != null)
         {
         AddUpdateUI?.Invoke(i);
         }
    }
    */