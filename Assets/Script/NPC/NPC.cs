using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPC : MonoBehaviour
{
    private Player player;
    private QuestManager questManager;
    private DialogManager dialogManager;
    public NPCInfo npcInfo;

    //플레이어와의 거리
    public float theDistance;

    public List<Quest> npcQuest;

    public string dialogXml;
    public string questXml;

    public Quest startAbleQuest;

    public int randomDialog;

    private void Start()
    {
        SetData();
    }

    // Update is called once per frame
    void Update()
    {
        //theDistance = Player.checkObjectDis;
        theDistance = 1;
    }

    public void SetData()
    {
        player = GameData.instance.player;

        dialogManager = DialogManager.instance;
        questManager = QuestManager.instance;
        
        dialogManager.RegistDialogXml(dialogXml);
        questManager.RegistQuestXml(npcInfo.npcName, questXml);

        //questManager.SetNpcQuest(npcInfo.npcName);
        foreach (Quest item in questManager.SetNpcQuest(npcInfo.npcName))
        {
            npcQuest.Add(item);
        }
    }

    private void OnMouseOver()
    {
        if (theDistance < 2.0f)
        {
            
        }
        else
        {
            
        }

        if (Input.GetButtonDown("Action"))
        {
            if (theDistance < 2.0f)
            {
                DoEvent();
            }
        }

    }

    private void DoEvent()
    {
        if(npcQuest.Count == 0)
        {
            questManager.cuurentState = QuestState.None;
            dialogManager.StartDialog(dialogXml, 0);
            return;
        }

        foreach (Quest _quest in npcQuest)
        {
            if(player.PlayerLv >= _quest.level)
            {
                startAbleQuest = _quest;
                startAbleQuest.questState = questManager.GetQuestState(_quest);
                break;
            }
        }

        switch (startAbleQuest.questState)
        {
            case QuestState.None:
                dialogManager.StartDialog(dialogXml, 0);
                break;
            case QuestState.Ready:
                dialogManager.StartDialog(dialogXml, startAbleQuest.dialogIndex);
                break;
            case QuestState.Accept:
                dialogManager.StartDialog(dialogXml, startAbleQuest.dialogIndex + Random.Range(1, startAbleQuest.randomIndex+1));
                break;
            case QuestState.Complete:
                dialogManager.StartDialog(dialogXml, startAbleQuest.completeIndex);
                break;
            default:
                break;
        }

    }

    private void OnMouseExit()
    {
       
    }
}
