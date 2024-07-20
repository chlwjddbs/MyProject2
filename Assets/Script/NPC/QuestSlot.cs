using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Localization.Settings;

public class QuestSlot : MonoBehaviour
{
    private QuestManager questManager;
    private Quest quest;
    public TextMeshProUGUI questName;

    public bool isOpen = false;

    public void SetQuestSlot()
    {
        questManager = QuestManager.instance;
        quest = questManager.currentQuest;
        questName.text = LocalizationSettings.StringDatabase.GetLocalizedString("Quest", quest.qName, LocalizationSettings.SelectedLocale);
    }

    public string QuestName()
    {
        return quest.qName;
    }

    public void OpenQuestUI()
    {
        if (!isOpen)
        {
            OtherSlotOpen();

            isOpen = true;
            questManager.currentQuest = quest;
            questManager.selectSlot = this;
            questManager.setQuestUI?.Invoke(true);
        }
    }

    public void OtherSlotOpen()
    {
        if (questManager.selectSlot != null)
        {
            if (questManager.selectSlot != this)
            {
                questManager.selectSlot.isOpen = false;
            }
        }
    }
}
