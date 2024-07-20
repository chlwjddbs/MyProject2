using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestList : MonoBehaviour
{
    public RectTransform myRect;

    public Transform questPage;

    public QuestSlot questPrefab;

    public Dictionary<string, QuestSlot> questSlots = new Dictionary<string, QuestSlot>();

    private void Awake()
    {
        QuestManager.instance.loadData += ListAdd;
        QuestManager.instance.removeList += ListRemove;
    }


    public void ListAdd(Quest quest)
    {
        QuestSlot questSlot = Instantiate(questPrefab, questPage);
        questSlot.SetQuestSlot();
        questSlots[quest.qName] = questSlot;
    }

    public void ListRemove(Quest quest)
    {
        if(questSlots.TryGetValue(quest.qName,out QuestSlot slot))
        {
            questSlots.Remove(slot.QuestName());
            Destroy(slot.gameObject);
        }
    }
}
