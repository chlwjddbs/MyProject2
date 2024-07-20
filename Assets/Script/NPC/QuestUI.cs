using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Settings;
using TMPro;
using UnityEngine.Pool;

public class QuestUI : MonoBehaviour
{
    private QuestManager questManager;
    private ControllOption controllOption;
    private QuestList questList;

    public RectTransform questUI;
    public RectTransform questListUI;

    public TextMeshProUGUI questName;

    public TextMeshProUGUI descriptionArea;
    public TextMeshProUGUI questGoal;

    public TextMeshProUGUI gold;
    public TextMeshProUGUI goldText;

    public TextMeshProUGUI exp;
    public TextMeshProUGUI expText;

    public TextMeshProUGUI itemText;

    public RectTransform goldReward;
    public RectTransform expReward;
    public RectTransform ItemReward;

    public GameObject AcceptButton;
    public GameObject GiveupButton;

    public Image rewardItemImagePrefab;
    public List<Image> rewardImages = new List<Image>();

    private ObjectPoolingManager poolingManager;
    public IObjectPool<Image> connectPool;

    private Vector3 ClosePos = new Vector3(0, 1080f, 0);

    private bool isOpen = false;
    private bool isQuestUIOpen = false;

    public KeyOption keyOption;

    public ConfirmedPopup confirmedPopup;

    public void Start()
    {
        questManager = QuestManager.instance;
        questManager.setQuestUI += SetQuestUI;

        controllOption = OptionManager.instance.controllOption;

        questList = GetComponentInChildren<QuestList>();

        poolingManager = ObjectPoolingManager.instance;
        poolingManager.RegisetPoolImageObj(rewardItemImagePrefab, new ObjectPool<Image>(CreatePool, OnGet, OnRelease, OnDes, maxSize: 3));
        connectPool = poolingManager.imagePool;

        for (int i = 0; i < 3; i++)
        {
            Image rewardImage = CreatePool();
            connectPool.Release(rewardImage);
        }
    }

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

    public void SetQuestUI(bool isListUI)
    {
        ReSetQuestUI();

        questName.text = LocalizationSettings.StringDatabase.GetLocalizedString("Quest", questManager.currentQuest.qName, LocalizationSettings.SelectedLocale);

        descriptionArea.text = LocalizationSettings.StringDatabase.GetLocalizedString("Quest", questManager.currentQuest.description , LocalizationSettings.SelectedLocale);
        questGoal.text = $"[{questManager.currentQuest.questProgress.currentAmount}/{questManager.currentQuest.questProgress.reachedAmount}]";

        if (questManager.currentQuest.goldReward == 0)
        {
            goldReward.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log(questManager.currentQuest.goldReward);
            goldReward.gameObject.SetActive(true);
            gold.text = questManager.currentQuest.goldReward.ToString();
            LayoutRebuilder.ForceRebuildLayoutImmediate(goldReward);
        }

        if(questManager.currentQuest.expReward == 0)
        {
            expReward.gameObject.SetActive(false);
        }
        else
        {
            expReward.gameObject.SetActive(true);
            exp.text = questManager.currentQuest.expReward.ToString();
            LayoutRebuilder.ForceRebuildLayoutImmediate(expReward);
        }

        if (questManager.currentQuest.itemReward.Count == 0)
        {
            ItemReward.gameObject.SetActive(false);
        }
        else
        {
            ItemReward.gameObject.SetActive(true);

            for (int i = 0; i < questManager.currentQuest.itemReward.Count; i++)
            {
                rewardImages.Add(connectPool.Get());
                rewardImages[i].sprite = questManager.currentQuest.itemReward[i].itemImege;
            }
        }

        LayoutRebuilder.ForceRebuildLayoutImmediate(questUI);
       
        AcceptButton.SetActive(!isListUI);
        GiveupButton.SetActive(isListUI);

        OpenUI();
    }

    public void ReSetQuestUI()
    {
        questName.text = null;

        descriptionArea.text = null;
        questGoal.text = null;

        gold.text = null;
        exp.text = null;

        if (rewardImages.Count > 0)
        {

            foreach (var item in rewardImages)
            {
                item.sprite = null;
                connectPool.Release(item);
            }

            rewardImages.Clear();
        }
    }

    public void OpenUI()
    {
        isQuestUIOpen = true;
        questUI.anchoredPosition = Vector3.zero;       
    }

    public void CloseUI()
    {
        isQuestUIOpen = false;
        questUI.anchoredPosition = ClosePos;
        if (questManager.selectSlot != null)
        {
            questManager.selectSlot.isOpen = false;
        }
    }

    private void ToggleUI()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseListUI();
            CloseUI();
        }

        if (Input.GetKeyDown(controllOption.bindKey_Dic[keyOption].bindKey))
        {
            isOpen = !isOpen;
            if (isOpen)
            {
                questListUI.anchoredPosition = new Vector3(0, 0, 0);
            }
            else
            {
                questListUI.anchoredPosition = new Vector3(0, 1080f, 0);
            }
        }
    }

    public void CloseListUI()
    {
        isOpen = false;
        questListUI.anchoredPosition = new Vector3(0, 1080f, 0);
    }

    public void Accecpt()
    {
        CloseUI();
        questManager.cuurentState = QuestState.Accept;
        questManager.currentQuest.questState = QuestState.Accept;
        questManager.performingQuest.Add(questManager.currentQuest);
        questList.ListAdd(questManager.currentQuest);
    }

    public void GiveUpButton()
    {
        confirmedPopup.myRect.anchoredPosition = Vector3.zero;
    }

    public void GiveupQuest()
    {
        CloseUI();
        confirmedPopup.CloseUI();
        questManager.GiveupQuest();
    }

    public void Cancel()
    {
        CloseUI();
        ReSetQuestUI();
        questManager.ResetQuest();
    }

    public bool UIOpenCheck()
    {
        return isOpen || isQuestUIOpen;
    }

    #region 오브젝트 풀링 메서드
    private Image CreatePool()
    {
        Image rewardImage = Instantiate(rewardItemImagePrefab, ItemReward);
        return rewardImage;
    }
    private void OnGet(Image _poolObj)
    {
        _poolObj.gameObject.SetActive(true);
    }
    private void OnRelease(Image _poolObj)
    {
        _poolObj.gameObject.SetActive(false);
    }

    private void OnDes(Image _poolObj)
    {
        Destroy(_poolObj.gameObject);
    }
    #endregion
}


