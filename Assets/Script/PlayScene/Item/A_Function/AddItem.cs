using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization.Settings;

public class AddItem : Interaction
{
    //���� ������ ����
    public Item item;

    //�ʵ忡 �������� ������ ������Ʈ
    public GameObject itemObject;

    //�ʵ忡 ���̴� ������ ������Ʈ �̸� UI
    public GameObject BackGroundUI;

    //�ʵ忡 ���̴� ������ �̸��� �������� TextMeshPro
    public TextMeshProUGUI itemnameText;

    public GameObject ItemShapeBox;

    [HideInInspector]public SetCursorImage cursor;
    private DrawOutline drawOutline;

    //�������� ����
    public int quantity;

    public int test = 0;

    private void Start()
    {
        BackGroundUI.SetActive(false);
        cursor = transform.GetComponent<SetCursorImage>();
        drawOutline = transform.GetComponent<DrawOutline>();
    }

    public override void LateUpdate()
    {
        base.LateUpdate();
        DrawItem();
    }

    private void DrawItem()
    {
        if (cursor.isDraw && ItemShapeBox.activeSelf == false)
        {
            ItemShapeBox.SetActive(true);
        }
        else if(!cursor.isDraw && !cursor.overMouse && ItemShapeBox.activeSelf == true)
        {
            ItemShapeBox.SetActive(false);
            theDistance = Mathf.Infinity;
        }
    }

    

    public override void MouseOver()
    {
        if (cursor.isDraw && cursor.overMouse)
        {
            if (player.isUI | player.isAction)
            {
                DontAction();
                return;
            }
            else
            {
                if (BackGroundUI.activeSelf == false)
                {
                    SetItemName();
                    BackGroundUI.SetActive(true);
                    drawOutline.DrawOutLine();
                }
                DoAction();
            }
        }
    }

    public override void DoAction()
    {
        if (!cursor.overMouse)
        {
            return;
        }
        if (theDistance < actionDis)
        {
            player.isObject = true;

            if (Input.GetMouseButtonDown(0))
            {
                //�κ��丮�� ������ ���������� ��ø ������ �Ҹ�ǰ�� ��� ���� �߰��� �����ϱ� ������ üũ�Ѵ�.
                if (Inventory.instance.isAdd == false)
                {
                    //�������� ���Կ� �� �� �ִ��� ���� ��� ���� ���� Ȯ��.
                    Inventory.instance.CheckUseableSlot?.Invoke(item);

                    //�����ϴٸ� ���� ��ø
                    if (Inventory.instance.UseableSlot)
                    {
                        AddItems();
                    }

                    //�Ұ����ϸ� ������ ȹ�� �Ұ�
                    else
                    {
                        //�κ��丮�� ����á�ٴ� �ȳ� �޽��� ���
                        Debug.Log("�κ��丮�� ���� á���ϴ�.");
                        return;
                    }
                }

                //�κ��丮�� �������� �ʾҴٸ� ������ Ÿ�Կ� ���� ������ ȹ��
                if (item.itemType == ItemType.Used || item.itemType == ItemType.Ingredient)
                {
                    if (item.itemType == ItemType.Used)
                    {
                        AudioManager.instance.PlayeSound("getPotion");
                    }
                    else
                    {
                        AudioManager.instance.PlayeSound("getItem");
                    }
                    AddItems();
                }
                else
                {
                    AudioManager.instance.PlayeSound("getItem");
                    Inventory.instance.GetItem(item);
                    Debug.Log(item.name + " : ������ ȹ��");
                    Destroy(itemObject);
                }
            }
        }
    }

    //��ø ������ �������� ��� �ִ���ø ���� �ѵ��� ������ �ֱ� ������ for���� ���� �������� �ϳ��� ȹ���Ͽ� ���� �ѵ���ŭ�� ȹ�� �� �� �ְ� �Ѵ�.
    public void AddItems()
    {
        //���� �������� ������ üũ�Ѵ�.
        int startQuantity = quantity;

        //�������� ������ŭ for���� ���� ������ ȹ�� ������ ��´�.
        for (int i = 0; i < startQuantity; i++)
        {
            //ȹ���� �������� ������ �Ѱ��־� ��� ������ ������ ã�´�.
            Inventory.instance.CheckUseableSlot?.Invoke(item);

            //�κ��丮�� ����á���� ��� ������ ����(������ ������ ��ĥ ����)�� �ִٸ�
            if (!Inventory.instance.isAdd && Inventory.instance.UseableSlot)
            {
                //������ �߰�
                Inventory.instance.AddItems(item);
                //Inventory.instance.
                //player.SetState(PlayerState.Idle);

                //�������� �߰��Ǿ��� ������ ���� ���� ���� ����
                quantity -= 1;

                //������ ���ҵʿ� ���� ȭ�鿡 �������� ������ �̸�(����ǥ��) ����
                SetItemName();

                //�������� ���� ȹ��Ǿ��� �� ������Ʈ�� �����Ͽ� �� ������Ʈ�� ���°��� ����
                if (quantity <= 0)
                {
                    Destroy(itemObject);
                }
            }
            else if (Inventory.instance.isAdd)
            {
                Inventory.instance.AddItems(item);
                //Debug.Log(item.name + " : ������ ȹ��");
                //player.SetState(PlayerState.Idle);
                quantity -= 1;
                SetItemName();

                if (quantity <= 0)
                {
                    Destroy(itemObject);
                    //player.isItem = false;
                }
            }
            else
            {
                break;
            }
        }
        //CursorManager.instance.ResetCursor();
    }

    public override void DontAction()
    {
        player.isObject = false;
        CursorManager.instance.ResetCursor();
        if (BackGroundUI.activeSelf == true)
        {
            BackGroundUI.SetActive(false);
            drawOutline.DrawOrign();
        }
    }

    public void SetItemName()
    {
        if ((item.itemType == ItemType.Used || item.itemType == ItemType.Ingredient ) && quantity > 1)
        {
            itemnameText.text = LocalizationSettings.StringDatabase.GetLocalizedString("ItemName", item.itemName, LocalizationSettings.SelectedLocale) + " x " + quantity;
        }
        else
        {
            itemnameText.text = LocalizationSettings.StringDatabase.GetLocalizedString("ItemName", item.itemName, LocalizationSettings.SelectedLocale);
        }
    }

    private void OnDestroy()
    {
        player.isObject = false;
        CursorManager.instance.ResetCursor();
    }
}

/*
    public override void OnMouseOver()
    {
        if (cursor.isDraw)
        {
            if (player.isUI | player.isAction)
            {
                DontAction();
                return;
            }
            else
            {
                if (BackGroundUI.activeSelf == false)
                {
                    SetItemName();
                    BackGroundUI.SetActive(true);
                }
                DoAction();
            }
        }
    }
    */