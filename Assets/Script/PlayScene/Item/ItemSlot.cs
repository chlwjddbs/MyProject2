using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour, IPointerClickHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Inventory inven;
    private Equipment equipment;
    private InventoryUI invenUI;
    private DropItemPopupUI dropUI;

    //���Կ� ��ϵ� ������ �̹���
    public Image itemImage;

    //������ ���õǾ��� �� ������ �̹���
    public GameObject selectImage;

    //���� ������ ��ȣ
    public int slotNum;

    [SerializeField]
    private Inventory.InvenItem itemInfo;

    //���� ���Կ� ��ϵ� ������
    private Item item
    {
        get
        {
            if (inven.invenItems.TryGetValue(slotNum, out itemInfo))
            {
                return inven.invenItems[slotNum]?.slotItem;
            }
            else
            {
                return null;
            }
        }
        set { inven.invenItems[slotNum].slotItem = value; }
    }

    private Item tempItem;

    //������ �巡�� ���� ��ġ
    private Transform _startParent;

    //������ �巡�׽� �̹����� sort�� ���̱� ���� �ӽ� �������
    public GameObject Dragtemp;

    private bool isDrag = false;

    public DropItemManager dropItemSlot;

    //�Ҹ�ǰ ������ ��ø����
    private int quantity
    {
        get { return inven.invenItems[slotNum].quantity; }
        set { inven.invenItems[slotNum].quantity = value; }
    }

    public TextMeshProUGUI quantityText;

    //������ ��� ���� ����
    public bool isUseable = true;
    //������ ��ø ���� ���� ����
    public bool isOverlap = false;
   
    public void SetData()
    {
        inven = Inventory.instance;
        equipment = Equipment.instance;
        dropItemSlot = DropItemManager.instance;

        //InventoryUI ��ũ��Ʈ�� ��� UIManager������Ʈ�� �����ǰ� �ִ�. �׷��Ƿ� ItemSlot�� �θ� ������Ʈ�� UIManager�κ��� ã�´�.
        invenUI = GetComponentInParent<InventoryUI>();
        dropUI = GetComponentInParent<DropItemPopupUI>();
        quantityText.text = "";
        CheckUseSlot();
    }

    //�÷��̾ ȹ���Ͽ� �κ��丮�� ��ϵ� ������ ������ ������ ���԰� ������ �ش�.
    public void SetItemSlot()
    {
        //�κ��丮�� �������� �߰� �Ǿ��� �� ����ȴ�.
        //�������� �߰� ������ �����ϰ� ������, ���࿡�� �κ��丮�� �������� ���ٰ� ���� �� �� ������ �̹����� ���� �ʵ��� ��� �ڵ� ����

        //�κ��丮�� �������� �ִٸ�
        if (inven.items[slotNum] != null)
        {
            //������ �ִ� ���� +1
            quantity++;

            //�κ��丮�� �ִ� �������� �����̶��
            if (inven.items[slotNum].itemType == ItemType.Potion)
            {
                //���� ������ �ִ� �������� ������ üũ�Ͽ� textǥ�ù�� ���� Ȯ��
                QuantityItem();

                //������ �ִ� ������ �Ѱ���� �̹��� ���� ��ϵ� �����̱� ������
                if (quantity == 1)
                {
                    //�κ��丮�� ����ִ� �������� ������ ���Կ� ����
                    //item = inven.items[slotNum];
                    //������ �̹��� �����ֱ�
                    itemImage.gameObject.SetActive(true);
                    itemImage.sprite = item.itemImege;

                    isOverlap = true;
                }
                //�������� ������ �������� �ִ� ��ø�ѵ��� �����ϸ� �ش� ���Կ��� �� �̻� ��ø�� �Ұ����ϴٰ� ǥ�����ش�.
                else if (quantity >= (item as Potion).ownershipLimit)
                {
                    //InventoryUI�� CheckSlot �Լ� ȣ�� �� �ݺ����� ���� ��� ������ üũ�ϰ�
                    //invenState�� able �Ͻ� �κ��丮���� ��� ������ ������ �����Ѵٰ� �����ϰ�
                    //enable�Ͻ� ��� ������ ������ ���ٰ� �����Ѵ�.
                    //�׷��� ������ ��ø�ѵ��� ��� ���� ����� �� ���� �����̶�� enable ������ ������ 
                    //�Լ� ȣ�� �� ��� ������ ������ �־ �ݺ����� ���� �۵����� �ʱ� ������ ��� ���·� ��ȯ���ش�.
                    invenUI.invenState = invenState.stay;

                    //quantityText.text = quantity.ToString();

                    isOverlap = false;
                }
            }
            //�κ��丮�� �ִ� �������� ������ ������ �������̶��
            else
            {
                //������ ����
                item = inven.items[slotNum];
                //������ �̹��� �����ֱ�
                itemImage.gameObject.SetActive(true);
                itemImage.sprite = item.itemImege;
            }

            //������ �߰� ���� �κ��丮 ���� üũ
            CheckUseSlot();
        }
    }

    //���Կ��� ������ ����
    public void RemoveItemSlot()
    {
        item = null;
        itemImage.gameObject.SetActive(false);
        itemImage.sprite = null;
        quantity = 0;
        QuantityItem();
        CheckUseSlot();
    }

    //���� �����ϱ�
    public void SelectSlot()
    {
        //������ ���Կ� �������� �ִ� ��쿡�� ���� ������ �����ϰ� ���ش�.
        //�������� ���� ��� �� �����̱� ������ ���� ó��
        if (item == null)
        {
            return;
        }
        selectImage.SetActive(true);
    }

    public void UseItem()
    {
        if (item == null)
        {
            return;
        }

        quantity--;
        //����ϴ� �������� ������ ��
        if (item.itemType == ItemType.Potion)
        {
            Debug.Log(inven.invenItems[slotNum].quantity);
            QuantityItem();
            //��� �� ���� ������ ���ٸ�
            if (quantity <= 0)
            {
                tempItem = item;
                //�κ��丮���� ���� ����
                inven.RemoveItem(slotNum);
                //���Կ��� ���� ����
                RemoveItemSlot();

                //��� ȿ�� �ߵ�
                tempItem.Use(slotNum);
            }
            else
            {
                item.Use(slotNum);
            }
        }
        //����ϴ� �������� ��ų���� ��
        else if(item.itemType == ItemType.SkillBook)
        {
            if(SkillBook.instance.isLearn(item as SkillItem))
            {
                tempItem = item;
                //�κ��丮���� ���� ����
                inven.RemoveItem(slotNum);
                //���Կ��� ���� ����
                RemoveItemSlot();
                tempItem.Use(slotNum);
            }
            else
            {
                Debug.Log("�̹� ������ ��ų �Դϴ�.");
            }
        }

        //�� �� ������ �� ��
        else
        {
            tempItem = item;
            //�κ��丮���� ���� ����
            inven.RemoveItem(slotNum);
            //���Կ��� ���� ����
            RemoveItemSlot();
            tempItem.Use(slotNum);
        }

        //������ ��� �� �κ��丮 ���� üũ
        CheckUseSlot();
    }

    public void QuantityItem()
    {
        if (quantity <= 1)
        {
            quantityText.text = "";
        }
        else
        {
            quantityText.text = quantity.ToString();
        }
        /*
        if (quantity <= 1)
        {
            quantityText.text = "";
        }
        else
        {
            quantityText.text = quantity.ToString();
        }
        */
    }

    public Item HavedItem()
    {
        return item;
    }


    //�κ��丮 ������ ��� ��������, �Ҹ�ǰ ��ø�� ������ �������� üũ.
    public void CheckUseSlot()
    {
        if (item == null)
        {
            isUseable = true;
            isOverlap = false;
            return;
        }
        if (item.itemType == ItemType.Potion)
        {
            if (quantity >= (item as Potion).ownershipLimit)
            {
                isUseable = false;
                isOverlap = false;
            }
            else
            {
                isUseable = true;
                isOverlap = true;
            }
        }
        else
        {
            isUseable = false;
            isOverlap = false;
        }
    }

    //���� ���Կ� �߰��Ǵ� �������� ������ �� ��ø ���� ���� üũ
    public bool PotionOverlapable()
    {
        //���� �ѵ��� ������ ������ true : ��ø ���� ����
        if (isUseable == true && isOverlap == true)
        {
            return true;
        }
        //�ִ� ���� �ѵ��� �����ϸ� false : ��ø �Ұ��� ����
        else if (isUseable == true && isOverlap == false)
        {
            return false;
        }
        else
        {
            return false;
        }
    }

    public void DropItem()
    {
        RaycastHit hit;
        Vector3 hitpos;
        Vector3 DropPos;

        //���콺��ġ���� ����ĳ��Ʈ�� ����
        if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, Mathf.Infinity, ~(1 << 10))) //�÷��̾�� ���� (�÷��̾� 
        {
            //��Ʈ�� ������Ʈ�� water, obstacle , object�� �ƴҰ�� �������� ���� �� �ְ� �Ѵ�.
            if (!hit.transform.CompareTag("Water") && !hit.transform.CompareTag("Obstacle") && !hit.transform.CompareTag("Object"))
            {
                hitpos = hit.point;
                Vector3 PlayerPos = new Vector3(invenUI.Player.transform.position.x, 1f, invenUI.Player.transform.position.z);
                if (invenUI.Player.GetComponentInChildren<PlayerSight>().Dropable(hitpos))
                {
                    //�������� �����̰� 2�� �̻� ������ �ִٸ�
                    if (item.itemType == ItemType.Potion && quantity > 1)
                    {
                        //��� ������ �����.
                        Debug.Log("��� �����ðڽ��ϱ�?");
                        dropUI.SetDropItemUI(slotNum, quantity, Input.mousePosition);
                    }
                    //�������� 2�� �̸��� �����̰ų� ������ �ƴҶ�
                    else
                    {
                        //���� �ÿ��̾��� ��ġ�� �������� ���� ��ġ�� �Ÿ��� 10f �̸��� ��
                        if ((PlayerPos - hitpos).magnitude < 10f)
                        {
                            hitpos.y = 0.5f;
                            //���� ��ġ�� ���� �������� �����ϰ�    
                            Instantiate(item.FieldObject, hitpos, item.FieldObject.transform.rotation, dropItemSlot.transform);
                            //�κ��丮���� �������� �����Ѵ�.
                            inven.RemoveItem(slotNum);
                            RemoveItemSlot();
                        }
                        //�Ÿ��� 10f �̻��� ��
                        else
                        {
                            //10�Ÿ� ���� �ֶ� DropPos�� �ִ�Ÿ��� 10���� �����Ͽ� 10��ġ�� �������� ������.
                            DropPos = PlayerPos + ((hitpos - PlayerPos).normalized * 10);
                            DropPos.y = 0.5f;
                            Instantiate(item.FieldObject, DropPos, item.FieldObject.transform.rotation,dropItemSlot.transform);
                            //�κ��丮���� �������� �����Ѵ�.
                            inven.RemoveItem(slotNum);
                            RemoveItemSlot();
                        }

                        AudioManager.instance.PlayeSound("dropItem");
                    }
                }
            }
        }
    }

    public void DropPotion(int _quantity, Vector2 _DropPos)
    {
        if (_quantity <= 0)
        {
            return;
        }

        RaycastHit hit;
        Vector3 hitpos;
        Vector3 DropPos;

        if (Physics.Raycast(Camera.main.ScreenPointToRay(_DropPos), out hit))
        {
            hitpos = hit.point;
            Vector3 PlayerPos = new Vector3(invenUI.Player.transform.position.x, 1f, invenUI.Player.transform.position.z);
            if ((PlayerPos - hitpos).magnitude < 10f)
            {
                hitpos.y = 0f;
                if (quantity - _quantity <= 0)
                {
                    Debug.Log("���� ����");
                    GameObject _potion = Instantiate(item.FieldObject, hitpos, item.FieldObject.transform.rotation,dropItemSlot.transform);
                    _potion.GetComponentInChildren<AddItem>().quantity = _quantity;
                    inven.RemoveItem(slotNum);
                    RemoveItemSlot();
                }
                else
                {
                    GameObject _potion = Instantiate(item.FieldObject, hitpos, item.FieldObject.transform.rotation,dropItemSlot.transform);
                    _potion.GetComponentInChildren<AddItem>().quantity = _quantity;
                    quantity -= _quantity;
                    QuantityItem();
                    CheckUseSlot();
                }
            }
            else
            {
                DropPos = PlayerPos + ((hitpos - PlayerPos).normalized * 10);
                DropPos.y = 0;
                if (quantity - _quantity <= 0)
                {
                    GameObject _potion = Instantiate(item.FieldObject, DropPos, item.FieldObject.transform.rotation,dropItemSlot.transform);
                    _potion.GetComponentInChildren<AddItem>().quantity = _quantity;
                    inven.RemoveItem(slotNum);
                    RemoveItemSlot();
                }
                else
                {
                    GameObject _potion = Instantiate(item.FieldObject, DropPos, item.FieldObject.transform.rotation,dropItemSlot.transform);
                    _potion.GetComponentInChildren<AddItem>().quantity = _quantity;
                    quantity -= _quantity;
                    QuantityItem();
                    CheckUseSlot();
                }
            }


        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            UseItem();
        }
    }


    public void OnBeginDrag(PointerEventData eventData)
    {
        if (item == null)
        {
            return;
        }
        isDrag = true;
        //Drag & Drop ������ Drop�� ���԰� Drag�� ������ �������� �ٲ� �־�� �Ѵ�.
        //Drop�� ������ �� ��ġ(Drag ������)�� ���Ѵ�.
        _startParent = transform.GetChild(0).GetChild(0);

        //Drag�� ������ ������ ǥ�� ���� �ʰ���
        quantityText.enabled = false;

        //Drag�� sort������ Drag���� �������� �������� ���� ����.
        //ItemSlot�� InventoryCanvas �ȿ� �׷����� �ִ�.
        //Drag �� itemImage�� ��ġ�� InventoryCanvas �� Dragtemp������ �Ű��־� sort�� ���� �̹����� �߸��� �ʵ��� �Ѵ�.       
        itemImage.transform.SetParent(Dragtemp.transform); // Dragtemp = InventoryCanvas�� �������� ��ġ�� Object�� sort�� ���� ����.
                                                           //InventoryCanvas�� ���� ������ UIManager�� ���� �Ǹ� UIManager�� Canvas�� �ƴϱ� ������ sort�� ���� �ʾ� �̹����� ������ �ʰ� �ȴ�.


        //sort�� �������� �ö���� �ڿ� �ִ� ������Ʈ�� ���� ������ DropSlot�� ã�� �� ���� �ȴ�.
        //�̹����� ���̸� ���־� Drop�� ������Ʈ�� �Ǻ� �����ϰ� �Ѵ�. 
        itemImage.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (item == null)
        {
            return;
        }
        //������ �̹����� �̵��ϴ°� �����ش�.
        itemImage.transform.position = eventData.position;

    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (item == null)
        {
            return;
        }

        isDrag = false;
        //Drag�� ǥ�õ��� �ʰ� �ߴ� ������ ���� ǥ�� ������
        quantityText.enabled = true;

        //�巡�װ� ������ ������ ������Ʈ���� hoveredObject�� ����
        List<GameObject> hoveredObject = eventData.hovered;

        //EventSystem.current.IsPointerOverGameObject() : ������Ʈ�� UI�� ��ġ�� false �׷��� ������ true
        //false �� �� UI�� �ƴ� ���ӻ� ������Ʈ ���� ���
        //Drag�� ���� UI�� �ƴ� ��� �������� ������.
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            DropItem();
        }
        else
        {
            for (int i = 0; i < hoveredObject.Count; i++)
            {
                //hoveredObject���� �ڽ��� �ƴ� Slot ������Ʈ�� ã�´�. 
                if (hoveredObject[i] != gameObject && hoveredObject[i].CompareTag("Slot"))
                {
                    //Debug.Log(hoveredObject[i]);
                    ItemSlot dropSlot = hoveredObject[i].GetComponent<ItemSlot>();
                    //Drag������ �̹����� ������(temp ����)
                    Image dragItemImage = itemImage;
                    TextMeshProUGUI dragQuantityText = quantityText;
                    Item dragItem = item;
                    //Drag���Կ� �ִ� �������� ����
                    int dragquntity = quantity;

                    Inventory.InvenItem _dragItem = new Inventory.InvenItem(inven.invenItems[slotNum].slotItem, inven.invenItems[slotNum].quantity,true);


                    //���� �������� �ܷ��� ��ġ�� ���
                    //��Ȳ> �ִ������ 20���� ������ A�� B���Կ� ���� �� B������ Drag�Ͽ� A���Կ� Drop ���� ��.
                    //��) A���Կ� 6��, B���Կ� 7���� ���� �� A������ 13���� �ǰ� B������ �� ������ �Ǿ�� �Ѵ�.
                    //��2) A���Կ� 19��, B���Կ� 3�� ���� �� A������ 20���� �Ǿ���ϰ�, B������ 2���� �Ǿ�� �Ѵ�.
                    //�������� �����̰�, Drop���Կ� �������� �����ϸ�, A���԰� B���� ��� ������ ��ø�� ������ �� �� ������ ���డ���ϴ�. + ���� ������ �����̿��� �Ѵ�.
                    if (item.itemType == ItemType.Potion && dropSlot.item != null && isOverlap == true && dropSlot.isOverlap == true && item.itemNumber == dropSlot.item.itemNumber)
                    {
                        //���� ������ ������ ��
                        if (item.itemName == dropSlot.item.itemName)
                        {
                            //Drag���԰� Drop������ ������ �ִ� ���� �ѵ����� �۰ų� ���ٸ�
                            if ((quantity + dropSlot.quantity) <= (dropSlot.item as Potion).ownershipLimit)
                            {
                                //Drop ���� ���� ����
                                dropSlot.quantity = quantity + dropSlot.quantity;
                                //Drag ���� �ʱ�ȭ
                                quantity = 0;
                                RemoveItemSlot();
                                inven.RemoveItem(slotNum);
                            }
                            //Drag���԰� Drop������ ������ �ִ� ���� �ѵ����� ���ٸ�
                            else
                            {
                                //Drop ���Կ� �Ѱ��ְ� ���� ������ ����
                                quantity = quantity - ((dropSlot.item as Potion).ownershipLimit - dropSlot.quantity);

                                //Drop ������ �ִ�ġ�� ä��
                                dropSlot.quantity = (dropSlot.item as Potion).ownershipLimit;
                            }
                            QuantityItem();
                            dropSlot.QuantityItem();
                            CheckUseSlot();
                            dropSlot.CheckUseSlot();
                        }
                    }
                    else
                    {
                        //Drop������ ������ �̹����� Drag���Կ� �־��ش�. (��ġ ��ȯ)
                        dropSlot.itemImage.transform.SetParent(_startParent);
                        //�̹����� ��ġ ����
                        dropSlot.itemImage.transform.localPosition = Vector3.zero;

                        //Drag������ ������ �̹����� Drop�� ���Կ� �־��ش�. (��ġ ��ȯ)
                        itemImage.transform.SetParent(hoveredObject[i].transform.GetChild(0).GetChild(0));
                        //�̹����� ��ġ ����
                        itemImage.transform.localPosition = Vector3.zero;

                        //Drag���۽� ���ξ��� ���̸� ���־� �ٽ� �巡�׽� ���� �� �� �ֵ��� �Ѵ�.
                        itemImage.raycastTarget = true;
                        dropSlot.itemImage.raycastTarget = true;

                        //Drag ������ �����ϸ� Drop������ �������� �޾� ���� ������ item�� �����Ѵ�.
                        //Drop������ �������� �޾ƿ´�. (������ ��ȯ)
                        item = dropSlot.item;
                        //Drop������ ������ �̹����� �޾ƿ´�. (�̹��� ��ȯ)
                        itemImage = dropSlot.itemImage;
                        
                        //Drop���Կ� �ִ� �������� ������ �޾ƿ´�. (���� ��ȯ)
                        quantity = dropSlot.quantity;
                        QuantityItem();

                        //Drag ���Կ� ����� item�� �������� ������ ���Կ� ����� �������� Drop���Կ� Drag �������� �����ϰ� �ȴ�
                        //Drop���� ������Ʈ�� Drag������ ������Ʈ���� �־��ش�.
                        dropSlot.itemImage = dragItemImage;
                        dropSlot.item = _dragItem.slotItem;
                        dropSlot.quantity = dragquntity;
                        dropSlot.QuantityItem();
                        //dropSlot.inven.invenItems[dropSlot.slotNum] = _dragItem;

                        //swap �Ϸ�� ������ �������� Inventory�� �Ѱ��ش�.
                        inven.items[slotNum] = item;
                        inven.items[dropSlot.slotNum] = dropSlot.item;

                        //�κ��丮 ��밡�� �� ��ȭ���� üũ               
                        CheckUseSlot();
                        dropSlot.CheckUseSlot();
                        AudioManager.instance.PlayeSound("dragEnd");
                    }
                }
                else if (hoveredObject[i].CompareTag("Equipslot") && item.itemType == ItemType.Equip)
                {

                    if (hoveredObject[i].GetComponent<EquipItemSlot>().slotTypeNum == (int)(((EquipItem)item).equipType))
                    {
                        Debug.Log("������ ����");
                        UseItem();
                    }
                }
            }
        }

        itemImage.raycastTarget = true;
        itemImage.transform.SetParent(_startParent);
        itemImage.transform.localPosition = Vector3.zero;

        //GetComponentInParent<UICheck>().enabled = false;
        //GetComponentInParent<UICheck>().enabled = true;

        invenUI.DeSelectAllSlots();
        //����Ʈ ������ ������ ������ hoveredObject�� ��� �׿� ���� �߻�
        
        hoveredObject.Clear();
    }

    //������ �巡�׸� �����ϸ鼭 �κ��丮�� �ݾ��� �� �̹����� ���� ��ġ�� ���ư��� �ʰ� 
    //�κ��丮�� ���� �� �ִ� ��ġ�� �̹����� �״�� ���� ������ ������
    public void OnDisable()
    {
        if (isDrag)
        {
            itemImage.transform.SetParent(_startParent);
            itemImage.transform.localPosition = Vector3.zero;
            QuantityItem();
            //isDrag = false; //������ ����
        }
    }

}
