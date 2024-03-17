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

    private SetCursorImage cursor;

    //�������� ����
    public int quantity;

    private void Start()
    {
        BackGroundUI.SetActive(false);
        cursor = transform.GetComponent<SetCursorImage>();
    }

    public override void Update()
    {
        base.Update();
        DrawItem();
    }

    private void DrawItem()
    {
        if (cursor.isDraw && ItemShapeBox.activeSelf == false)
        {
            ItemShapeBox.SetActive(true);
        }
        else if(!cursor.isDraw && ItemShapeBox.activeSelf == true)
        {
            ItemShapeBox.SetActive(false);
        }
    }

    public override void OnMouseOver()
    {
        if (cursor.isDraw)
        {
            if (PlayerController.isUI | PlayerController.isAction)
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

    public override void DoAction()
    {
        player.isObject = true;
        if (Input.GetMouseButtonDown(0))
        {
            if (theDistance < actionDis)
            {
                //�κ��丮�� ������ ���������� ��ø ������ �Ҹ�ǰ�� ��� ���� �߰��� �����ϱ� ������ üũ�Ѵ�.
                if (Inventory.instance.isAdd == false)
                {
                    //�������� ���Կ� �� �� �ִ��� ���� ��� ���� ���� Ȯ��.
                    Inventory.instance.CheckUseableSlot?.Invoke(item);

                    //�����ϴٸ� ���� ��ø
                    if (Inventory.instance.UseableSlot)
                    {
                        AddPotion();
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
                if (item.itemType == ItemType.Potion)
                {
                    AudioManager.instance.PlayeSound("getPotion");
                    AddPotion();
                }
                else
                {
                    /*
                    switch (item.itemType)
                    {
                        case ItemType.Equip:
                            AudioManager.instance.PlayeSound("addEquip");
                            break;
                        case ItemType.Ingredient:
                            break;
                        case ItemType.Potion:
                            break;
                        case ItemType.Puzzle:
                            break;
                        case ItemType.SkillBook:
                            break;
                        default:
                            break;
                    }
                    */

                    AudioManager.instance.PlayeSound("getItem");
                    Inventory.instance.AddItem(item);
                    Debug.Log(item.name + " : ������ ȹ��");
                    player.SetState(PlayerState.Idle);
                    Destroy(itemObject);
                    //player.isItem = false;
                }

                //Cursor.SetCursor(CursorManager.instance.orginCursor, CursorManager.instance.hotSpot, CursorMode.Auto);
                //CursorManager.instance.ResetCursor();
            }
        }
    }

    //��ø ������ �������� ��� �ִ���ø ���� �ѵ��� ������ �ֱ� ������ for���� ���� �������� �ϳ��� ȹ���Ͽ� ���� �ѵ���ŭ�� ȹ�� �� �� �ְ� �Ѵ�.
    public void AddPotion()
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
                Inventory.instance.AddPotion(item);
                //Inventory.instance.
                player.SetState(PlayerState.Idle);

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
                Inventory.instance.AddPotion(item);
                //Debug.Log(item.name + " : ������ ȹ��");
                player.SetState(PlayerState.Idle);
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
        }
    }

    public void SetItemName()
    {
        if (item.itemType == ItemType.Potion && quantity > 1)
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
