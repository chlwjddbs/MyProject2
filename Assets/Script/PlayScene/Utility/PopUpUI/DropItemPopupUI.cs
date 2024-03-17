using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DropItemPopupUI : MonoBehaviour
{ 
    public GameObject DropItemPopUpUI;
    private RectTransform popupUIrect;
    public TMP_InputField inputField;
    public bool isOpen;
    private InventoryUI invenUI;

    //inputfield�� �����Ǵ� ��
    public int value;

    //�������� ���� ������ ��ȣ
    private int slotNum;

    //������ ������ �ִ� �������� ����
    private int slotQuantity;

    //�������� ���� ��ġ
    private Vector2 DropPos;

    private void Start()
    {
        popupUIrect = DropItemPopUpUI.GetComponent<RectTransform>();
        SetInputField();
        CloseUI();
        isOpen = false;
        invenUI = GetComponent<InventoryUI>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!DataManager.instance.isSet)
        {
            return;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CancelButton();
        }

        if (isOpen)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
            {
                //inputfield�� Ȱ��ȭ ��Ų��. (��Ŀ����)
                inputField.ActivateInputField();

                //caret�� �ؽ�Ʈ ������ �̵���Ų��. true �̸� ������ �ؽ�Ʈ ������ �ǰ� false�̸� caret�� text������ �̵��Ѵ�.
                inputField.MoveTextEnd(false);
            }
        }
    }

    private void SetInputField()
    {
        inputField.ActivateInputField();
        inputField.text = "0";
        inputField.MoveTextEnd(false);
    }

    public void SetDropItemUI(int _slotNum, int _slotQuantity, Vector2 openUIPos)
    {
        isOpen = true;
        DropItemPopUpUI.SetActive(true);
        DropItemPopUpUI.transform.position = openUIPos;
        slotNum = _slotNum;
        slotQuantity = _slotQuantity;
        DropPos = openUIPos;
        SetInputField();
    }

    public void UpButton()
    {
        value = int.Parse(inputField.text);
        value += 1;
        value = Mathf.Clamp(value, 0, slotQuantity);
        inputField.text = value.ToString();
    }

    public void DownButton()
    {
        value = int.Parse(inputField.text);
        value -= 1;
        value = Mathf.Clamp(value, 0, slotQuantity);
        inputField.text = value.ToString();
    }

    public void UpupButton()
    {
        value = int.Parse(inputField.text);
        value += 5;
        value = Mathf.Clamp(value, 0, slotQuantity);
        inputField.text = value.ToString();
    }

    public void DowndownButton()
    {
        value = int.Parse(inputField.text);
        value -= 5;
        value = Mathf.Clamp(value, 0, slotQuantity);
        inputField.text = value.ToString();
    }

    public void OkButton()
    {
        AudioManager.instance.PlayeSound("dropItem");
        invenUI.itemSlot[slotNum].GetComponentInChildren<ItemSlot>().DropPotion(value,DropPos);
        CancelButton();
    }

    public void CancelButton()
    {
        isOpen = false;
        inputField.text = "0";
        CloseUI();
        slotNum = -1;
        slotQuantity = 0;
        DropPos = Vector2.zero;

    }

    public void CloseUI()
    {
        popupUIrect.anchoredPosition = new Vector3(0, 1080f, 0);
    }

    public void GetValue()
    {
        if (inputField.text == "")
        {
            value = 0;
            inputField.text = value.ToString();
            inputField.MoveTextEnd(false);
            return;
        }

        value = int.Parse(inputField.text);
        if (value > slotQuantity)
        {
            value = Mathf.Clamp(value, 0, slotQuantity);
        }
        inputField.text = value.ToString();
    }
}
