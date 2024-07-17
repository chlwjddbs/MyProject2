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
        questManager.currentQuest = quest;
        questManager.selectSlot = this;
        questManager.setQuestUI?.Invoke(true);
    }
}
