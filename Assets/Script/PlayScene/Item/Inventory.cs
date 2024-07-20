using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class Inventory : MonoBehaviour
{
    [System.Serializable]
    public class InvenItem
    {
        //Item을 ScriptableObject로 만들어져 있다.
        //그렇기 때문에 직접적으로 수량등을 변경 할 수 없기 때문에 인벤토리 아이템으로 따로 정보를 만들어준다.
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

    //인벤토리 업데이트 시 실행될 액션 함수 등록
    //public UnityAction<int, Item> UpdateInventory;
    public UnityAction<int> AddUpdateUI;

    public UnityAction<int> RemoveUpdateUI;

    public UnityAction<int> UpdateGoldUI;

    //public List<Item> items = new List<Item>();
    public int invenSize;
    //아이템이 들어온 슬롯번호
    public int slotNum = -1;

    //딕셔너리는 하이어라키에서 볼 수 없으므로 현재 아이템 목록을 볼 수 있게 배열을 public으로 만들어준다.
    //딕셔너리보다 편하게 접근할 수 있기 때문에 사용할 거면 사용해주자.  ex)items[slotNum] == inventiems[slotNum].slotItem;
    public Item[] items;
    //다량 아이템의 수량 배열
    //public int[] quantityCheck;

    //플레이가 사용할 수 있는 화폐;
    public int gold;

    //사용중인 인벤토리 슬롯의 갯수
    private int useSlot = 0;
    //인벤토리에 아이템을 추가 할 수 있나요?
    public bool isAdd = true;

    //중첩 가능한 아이템 획득 시 습득한 아이템과 같은 아이템이 있는 슬롯과 중첩 가능 공간이 남은 슬롯 체크
    public UnityAction<Item> CheckUseableSlot;
    public bool UseableSlot = false;

    //중첩 가능한 아이템이 중첩되어야 하는지, 중첩이 불가능 하여 새로운 슬롯을 차지해야 하는지 체크
    public UnityAction<int, Item> GoodsOverlap;
    public bool isOverlap = false;

    //inventoryUI의 set함수를 실행
    public UnityAction SetUIData;
    //Inventory load data를 ItemSlot과 연동
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
                Debug.Log($"{item.Key}번 슬롯 : {item.Value.slotItem.itemName}의 갯수 = {item.Value.quantity}개");
            }
        }
    }
    */

    public void SetData()
    {
        if (instance == null)
        {
            SetData();
            Debug.Log("인벤토리 싱글톤 생성 대기중");
            return;
        }

        if (SetUIData == null)
        {
            SetData();
            Debug.Log("인벤토리 UI 데이터 준비중");
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

                //아이템이 들어오면 인벤토리의 소모율 증가
                useSlot += 1;
                InventorySpace();
                break;
            }
        }
        //items.Add(_item);
        //액션함수가 등록되어 있을 시 실행
        //UpdateInventory?.Invoke(slotNum, items[slotNum]);
    }

    //포션 아이템 습득시
    public void AddPotion(Item _potion)
    {
        //습득한 아이템이랑 동일한 템이 있고, 중첩 가능한 여유 공간이 남은 슬롯 체크
        CheckUseableSlot?.Invoke(_potion);
        //Debug.Log(UseableSlot);
        //동일한 템이 있다면
        if (UseableSlot)
        {
            for (int i = 0; i < items.Length; i++)
            {
                //아이템이 없는 슬롯은 패스
                if(items[i] == null)
                {
                    continue;
                }

                //같은 아이템이 있는 슬롯 찾기
                else if (items[i].itemName == _potion.itemName)
                {
                    //새로 습득한 아이템이 현재 슬롯에 중첩 가능한지 체크
                    GoodsOverlap?.Invoke(i,_potion);

                    //가능하면 아이템 중첩
                    if (isOverlap)
                    {
                        AddUpdateUI?.Invoke(i);
                        //Debug.Log("중첩");
                        break;
                    }

                    //불가능 중첩 가능한 다음 슬롯을 찾는다.
                    //전체 슬롯에서 중첩 가능한 슬롯은 존재 할수 없다.
                    //CheckUseableSlot?.Invoke(_potion.itemName); 으로 중첩 가능한 공간이 남은 슬롯을 판정했기 때문.
                }
            }
        }
        //동일한 템이 없거나, 동일한 템이 존재하여도 중첩가능 공간이 남아 있지 않았다면
        else
        {
            //빈 슬롯에 아이템 추가
            for (int i = 0; i < items.Length; i++)
            {
                if (items[i] == null)
                {
                    //items[i] = _potion;
                    slotNum = i;
                    invenItems[slotNum].slotItem = _potion;
                    items[slotNum] = invenItems[slotNum].slotItem;
                    AddUpdateUI?.Invoke(slotNum);

                    //아이템이 들어오면 인벤토리의 소모율 증가
                    useSlot += 1;
                    InventorySpace();

                    Debug.Log("아이템 추가");
                    break;
                }
            }
        }

        //사용 가능한 인벤토리는 반듯이 존재한다. 왜냐하면 아이템을 인벤토리에 등록하기전, 습득단계에서 획득 가능 여부를 체크하기 때문
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
            Debug.Log("소지금이 부족합니다.");
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

        //아이템을 제거하면 인벤토리 소모율 감소
        useSlot -= 1;
        InventorySpace();
        //items.Remove(_item);
        //UpdateInventory?.Invoke(slotNum, items[slotNum]);
    }
    
    public void InventorySpace()
    {
        //인벤토리가 invenSize(인벤토리 한도)에 도달했는지 체크
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