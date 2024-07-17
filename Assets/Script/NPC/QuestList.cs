using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestList : MonoBehaviour
{
    public RectTransform myRect;

    public Transform questPage;

    public QuestSlot questPrefab;

    public List<QuestSlot> questSlots;


    public void ListAdd(Quest quset)
    {
        QuestSlot questSlot = Instantiate(questPrefab, questPage);
        questSlot.SetQuestSlot();
        questSlots.Add(questSlot);
    }

    public void ListRemove(Quest quest)
    {
        foreach (var slot in questSlots)
        {
            if(slot.QuestName() == quest.qName)
            {
                questSlots.Remove(slot);
                Destroy(slot);
            }
        }
    }
}
