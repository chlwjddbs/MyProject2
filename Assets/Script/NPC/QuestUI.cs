using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Pool;

public class QuestUI : MonoBehaviour
{
    private QuestManager questManager;

    public RectTransform questUI;

    public TextMeshProUGUI questName;

    public TextMeshProUGUI descriptionArea;
    public TextMeshProUGUI questGoal;

    public TextMeshProUGUI gold;
    public TextMeshProUGUI goldText;

    public TextMeshProUGUI exp;
    public TextMeshProUGUI expText;

    public TextMeshProUGUI itemText;

    public Transform goldReward;
    public Transform expReward;
    public Transform ItemReward;

    public Image rewardItemImagePrefab;
    public List<Image> rewardImages = new List<Image>();

    private ObjectPoolingManager poolingManager;
    public IObjectPool<Image> connectPool;

    private Quest curQuset;
    private Vector3 ClosePos = new Vector3(0, 1080f, 0);

    public void Start()
    {
        questManager = QuestManager.instance;
        questManager.setQuestUI += SetQuestUI;

        poolingManager = ObjectPoolingManager.instance;
        poolingManager.RegisetPoolImageObj(rewardItemImagePrefab, new ObjectPool<Image>(CreatePool, OnGet, OnRelease, OnDes, maxSize: 3));
        connectPool = poolingManager.imagePool;

        for (int i = 0; i < 3; i++)
        {
            Image rewardImage = CreatePool();
            connectPool.Release(rewardImage);
        }
    }

    public void SetQuestUI(Quest _quest)
    {
        curQuset = _quest;

        questName.text = _quest.qName;

        descriptionArea.text = _quest.description;
        questGoal.text = $"[{_quest.questProgress.currentAmount}/{_quest.questProgress.reachedAmount}]";

        if (_quest.goldReward == 0)
        {
            goldReward.gameObject.SetActive(false);
        }
        else
        {
            goldReward.gameObject.SetActive(true);
            gold.text = _quest.goldReward.ToString();
        }

        if(_quest.expReward == 0)
        {
            expReward.gameObject.SetActive(false);
        }
        else
        {
            expReward.gameObject.SetActive(true);
            exp.text = _quest.expReward.ToString();
        }

        if (_quest.itemReward.Count == 0)
        {
            ItemReward.gameObject.SetActive(false);
        }
        else
        {
            ItemReward.gameObject.SetActive(true);

            for (int i = 0; i < _quest.itemReward.Count; i++)
            {
                rewardImages.Add(connectPool.Get());
                rewardImages[i].sprite = _quest.itemReward[i].itemImege;
            }
        }

        OpenUI();
    }

    public void ReSetQuestUI()
    {
        CloseUI();

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

        questManager.ResetQuest();
    }

    public void OpenUI()
    {
        questUI.anchoredPosition = Vector3.zero;
    }

    public void CloseUI()
    {
        questUI.anchoredPosition = ClosePos;
    }

    public void Accecpt()
    {
        CloseUI();
        questManager.performingQuest.Add(curQuset);
    }

    public void Cancel()
    {
        ReSetQuestUI();
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


