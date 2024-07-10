using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization.Settings;
public class ItemInformation : MonoBehaviour
{
    private ControllOption controllOption;
    public KeyOption keyOption;

    #region equipType
    public List<RectTransform> equipRect;
    public List<TextMeshProUGUI> equipType;
    public Image equipImage;
    #endregion

    #region IngredientType
    [Space(10)]
    public List<RectTransform> ingredientRect;
    public List<TextMeshProUGUI> ingredientType;
    public Image ingredientImage;
    #endregion

    #region UsedType
    [Space(10)]
    public List<RectTransform> usedRect;
    public List<TextMeshProUGUI> usedType;
    public Image usedImage;
    #endregion

    #region KeyType
    [Space(10)]
    public List<RectTransform> keyRect;
    public List<TextMeshProUGUI> keyType;
    public Image keyImage;
    #endregion

    #region SkillBookType
    [Space(10)]
    public List<RectTransform> skillBookRect;
    public List<TextMeshProUGUI> skillBookType;
    public Image skillBookImage;
    #endregion

    #region EtcType
    [Space(10)]
    public List<RectTransform> etcRect;
    public List<TextMeshProUGUI> etcType;
    public Image etcImage;
    #endregion

   //인벤토리나 장착중인 장비에서 선택된 아이템
    private Item selectItem;

    //아이템 옵션 및 설명을 띄워줄 ItemInformation 오브젝트의 rect
    private RectTransform myRect;

    //옵션 <-> 설명으로 탭 변경 시 변경 될 ItemInformation의 rect
    private Vector2 myVec;

    //현재 선택된 아이템의 타입에 맞는 옵션을 표시해줄 UI : 장비, 소모품 , 재료 등 각자의 타입에 따라 표시하는 옵션이 다름
    private RectTransform selectedItemRect;

    //아이템의 자세한 설명이 써져잇는 UI
    private RectTransform descriptionRect;
    private TextMeshProUGUI description;

    private bool viewDescription = false;

    // Update is called once per frame
    void Update()
    {
        if (gameObject.activeSelf)
        {
            if (Input.GetKeyDown(controllOption.bindKey_Dic[keyOption].bindKey))
            {
                viewDescription = !viewDescription;
                ViewDescription();
            }
        }
    }

    public void SetData()
    {
        controllOption = OptionManager.instance.controllOption;
        controllOption.changeKeyCode += ChangeKeyCode;
        gameObject.SetActive(false);
        myRect = GetComponent<RectTransform>();
        myVec = new Vector2(300, 350);
    }

    public void SetDescription(Item _selectItem, Vector3 _slotPos)
    {
        gameObject.SetActive(true);
        selectItem = _selectItem;
        AllOff();
        myRect.anchoredPosition = _slotPos;

        switch (selectItem.itemType)
        {
            case ItemType.Equip:
                EquipType();
                break;
            case ItemType.Ingredient:
                IngredientType();
                break;
            case ItemType.Used:
                UsedType();
                break;
            case ItemType.SkillBook:
                SkillBookType();
                break;
            case ItemType.Etc:
                Etc();
                break;
        }
    }

    public void EquipType()
    {
        EquipItem equip = (EquipItem)selectItem;

        foreach (var rect in equipRect)
        {
            rect.gameObject.SetActive(true);
        }

        selectedItemRect = equipRect[0];
        descriptionRect = equipRect[10];
        description = equipType[10];

        equipImage.sprite = equip.itemImege;
        equipType[0].text = LocalizationSettings.StringDatabase.GetLocalizedString("ItemName", equip.itemName, LocalizationSettings.SelectedLocale);
        equipType[1].text = equip.Level.ToString();
        equipType[2].text = LocalizationSettings.StringDatabase.GetLocalizedString("ItemGrade_Type", equip.equipType.ToString() , LocalizationSettings.SelectedLocale);
        equipType[3].text = LocalizationSettings.StringDatabase.GetLocalizedString("ItemGrade_Type", equip.itemGrade.ToString(), LocalizationSettings.SelectedLocale);
        equipType[4].text = equip.attack.ToString();
        equipType[5].text = equip.defence.ToString();
        equipType[6].text = equip.health.ToString();
        equipType[7].text = equip.mana.ToString();
        equipType[8].text = equip.moveSpeed.ToString();

        equipType[9].text = LocalizationSettings.StringDatabase.GetLocalizedString("ItemDescription", "ViewDescription", LocalizationSettings.SelectedLocale) + $" ({controllOption.bindKey_Dic[keyOption].bindKey})";
        equipType[10].text = LocalizationSettings.StringDatabase.GetLocalizedString("ItemDescription", equip.itemName, LocalizationSettings.SelectedLocale);

        ViewDescription();

        LayoutRebuilder.ForceRebuildLayoutImmediate(selectedItemRect);

        if (equip.attack == 0)
        {
            equipRect[5].gameObject.SetActive(false);
        }
        if(equip.defence == 0)
        {
            equipRect[6].gameObject.SetActive(false);
        }
        if(equip.health == 0)
        {
            equipRect[7].gameObject.SetActive(false);
        }
        if(equip.mana == 0)
        {
            equipRect[8].gameObject.SetActive(false);
        }
        if(equip.moveSpeed == 0)
        {
            equipRect[9].gameObject.SetActive(false);
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(selectedItemRect);

        myVec.y = selectedItemRect.rect.height;
        myRect.sizeDelta = myVec;

        /*
        foreach (var rect in equipRect)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rect);
        }
        */
    }

    public void IngredientType()
    {
        selectedItemRect = ingredientRect[0];
        descriptionRect = ingredientRect[4];
        description = ingredientType[4];

        selectedItemRect.gameObject.SetActive(true);
        descriptionRect.gameObject.SetActive(true);

        ingredientImage.sprite = selectItem.itemImege;
        ingredientType[0].text = LocalizationSettings.StringDatabase.GetLocalizedString("ItemName", selectItem.itemName, LocalizationSettings.SelectedLocale);
        ingredientType[1].text = LocalizationSettings.StringDatabase.GetLocalizedString("ItemGrade_Type", selectItem.itemType.ToString(), LocalizationSettings.SelectedLocale);
        ingredientType[2].text = LocalizationSettings.StringDatabase.GetLocalizedString("ItemGrade_Type", selectItem.itemGrade.ToString(), LocalizationSettings.SelectedLocale);
        ingredientType[3].text = LocalizationSettings.StringDatabase.GetLocalizedString("ItemDescription", "ViewDescription", LocalizationSettings.SelectedLocale) + $" ({controllOption.bindKey_Dic[keyOption].bindKey})";
        ingredientType[4].text = LocalizationSettings.StringDatabase.GetLocalizedString("ItemDescription", selectItem.itemName, LocalizationSettings.SelectedLocale);

        LayoutRebuilder.ForceRebuildLayoutImmediate(selectedItemRect);
        ViewDescription();

        LayoutRebuilder.ForceRebuildLayoutImmediate(selectedItemRect);

        myVec.y = selectedItemRect.rect.height;
        myRect.sizeDelta = myVec;
    }

    public void UsedType()
    {
        selectedItemRect = usedRect[0];
        descriptionRect = usedRect[5];
        description = usedType[5];

        selectedItemRect.gameObject.SetActive(true);
        descriptionRect.gameObject.SetActive(true);

        usedImage.sprite = selectItem.itemImege;
        usedType[0].text = LocalizationSettings.StringDatabase.GetLocalizedString("ItemName", selectItem.itemName, LocalizationSettings.SelectedLocale);
        usedType[1].text = LocalizationSettings.StringDatabase.GetLocalizedString("ItemGrade_Type", selectItem.itemType.ToString(), LocalizationSettings.SelectedLocale);
        usedType[2].text = LocalizationSettings.StringDatabase.GetLocalizedString("ItemGrade_Type", selectItem.itemGrade.ToString(), LocalizationSettings.SelectedLocale);
        usedType[3].text = LocalizationSettings.StringDatabase.GetLocalizedString("UsedItemEffect", selectItem.itemName, LocalizationSettings.SelectedLocale);
        usedType[4].text = LocalizationSettings.StringDatabase.GetLocalizedString("ItemDescription", "ViewDescription", LocalizationSettings.SelectedLocale) + $" ({controllOption.bindKey_Dic[keyOption].bindKey})";
        usedType[5].text = LocalizationSettings.StringDatabase.GetLocalizedString("ItemDescription", selectItem.itemName, LocalizationSettings.SelectedLocale);

        LayoutRebuilder.ForceRebuildLayoutImmediate(usedType[3].rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(selectedItemRect);
        ViewDescription();

        LayoutRebuilder.ForceRebuildLayoutImmediate(selectedItemRect);

        myVec.y = selectedItemRect.rect.height;
        myRect.sizeDelta = myVec;
    }

    public void KeyType()
    {

    }

    public void SkillBookType()
    {
        SkillItem skillItem = (SkillItem)selectItem;

        selectedItemRect = skillBookRect[0];
        descriptionRect = skillBookRect[6];
        description = skillBookType[6];

        selectedItemRect.gameObject.SetActive(true);
        descriptionRect.gameObject.SetActive(true);

        skillBookImage.sprite = skillItem.itemImege;
        skillBookType[0].text = LocalizationSettings.StringDatabase.GetLocalizedString("ItemName", skillItem.itemName, LocalizationSettings.SelectedLocale);
        skillBookType[1].text = skillItem.skill.skillLevel.ToString();
        skillBookType[2].text = LocalizationSettings.StringDatabase.GetLocalizedString("ItemGrade_Type", skillItem.itemType.ToString(), LocalizationSettings.SelectedLocale);
        skillBookType[3].text = LocalizationSettings.StringDatabase.GetLocalizedString("ItemGrade_Type", skillItem.itemGrade.ToString(), LocalizationSettings.SelectedLocale);
        skillBookType[4].text = LocalizationSettings.StringDatabase.GetLocalizedString("SkillEffect", skillItem.skillName, LocalizationSettings.SelectedLocale);
        skillBookType[5].text = LocalizationSettings.StringDatabase.GetLocalizedString("ItemDescription", "ViewDescription", LocalizationSettings.SelectedLocale) + $" ({controllOption.bindKey_Dic[keyOption].bindKey})";
        skillBookType[6].text = LocalizationSettings.StringDatabase.GetLocalizedString("ItemDescription", skillItem.skillName, LocalizationSettings.SelectedLocale);

        LayoutRebuilder.ForceRebuildLayoutImmediate (skillBookType[4].rectTransform);
        LayoutRebuilder.ForceRebuildLayoutImmediate(selectedItemRect);
        ViewDescription();

        LayoutRebuilder.ForceRebuildLayoutImmediate(selectedItemRect);

        myVec.y = selectedItemRect.rect.height;
        myRect.sizeDelta = myVec;
    }

    public void Etc()
    {

    }

    public void AllOff()
    {
        equipRect[0].gameObject.SetActive(false);
        ingredientRect[0].gameObject.SetActive(false);
        usedRect[0].gameObject.SetActive(false);
        //keyRect[0].gameObject.SetActive(false);
        skillBookRect[0].gameObject.SetActive(false);
        //etcRect[0].gameObject.SetActive(false);
    }

    public void ViewDescription()
    {
        if (viewDescription)
        {
            description.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(description.rectTransform);
            LayoutRebuilder.ForceRebuildLayoutImmediate(descriptionRect);
            LayoutRebuilder.ForceRebuildLayoutImmediate(selectedItemRect);

            myVec.y = selectedItemRect.rect.height;
            myRect.sizeDelta = myVec;
        }
        else
        {
            description.gameObject.SetActive(false);
            LayoutRebuilder.ForceRebuildLayoutImmediate(descriptionRect);
            LayoutRebuilder.ForceRebuildLayoutImmediate(selectedItemRect);

            myVec.y = selectedItemRect.rect.height;
            myRect.sizeDelta = myVec;
        }
    }

    public void ChangeKeyCode(KeyOption _option, KeyCode _keyCode)
    {
        if (keyOption == _option)
        {
            if (_keyCode == KeyCode.None)
            {
                //Debug.Log($"{gameObject.name}키는 삭제됐습니다.");
                //keyCodeText.text = null;
            }
            else
            {
                //Debug.Log($"{gameObject.name}키는 {_keyCode}으로 변경되었습니다.");
                //keyCodeText.text = _keyCode.ToString();
            }
        }
    }

        private void OnDisable()
    {
        selectedItemRect = null;
        descriptionRect?.gameObject.SetActive(false);
    }
}
