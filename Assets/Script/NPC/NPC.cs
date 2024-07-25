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

    private SetCursorImage setCursor;

    //플레이어와의 거리
    public float theDistance;

    public List<Quest> npcQuest;
    public Dictionary<string, Quest> questDic = new Dictionary<string, Quest>();

    public string dialogXml;
    public string questXml;

    public Quest startAbleQuest;

    public int randomDialog;

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
        setCursor = GetComponent<SetCursorImage>();


        dialogManager.RegistDialogXml(dialogXml);
        questManager.RegistQuestXml(npcInfo.npcName, questXml);

        //questManager.SetNpcQuest(npcInfo.npcName);
        foreach (Quest _quest in questManager.SetNpcQuest(npcInfo.npcName))
        {
            npcQuest.Add(_quest);
            questDic[_quest.qName] = _quest;
        }
    }

    public void LoadData()
    {
        foreach (var completeQuest in questManager.completeQuest)
        {
            if (questDic.TryGetValue(completeQuest.qName, out Quest value))
            {
                for (int i = 0; i < npcQuest.Count; i++)
                {
                    if(npcQuest[i].qName == value.qName)
                    {
                        npcQuest.Remove(npcQuest[i]);
                    }
                }
                questDic.Remove(value.qName);
            }
        }

        foreach (var performingQuest in questManager.performingQuest)
        {
            if (questDic.TryGetValue(performingQuest.qName, out Quest value))
            {
                value.questState = performingQuest.questState;
                value.questProgress = performingQuest.questProgress;
            }
        }
    }

    private void OnMouseOver()
    {
        if (Input.GetButtonDown("Action"))
        {
            if (setCursor.theDistance < setCursor.actionDis)
            {
                DoEvent();
            }
        }
    }

    private void DoEvent()
    {
        if(questDic.Count == 0)
        {
            questManager.cuurentState = QuestState.None;
            dialogManager.StartDialog(dialogXml, 0);
            return;
        }

        foreach (Quest _quest in questDic.Values)
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
                npcQuest.Remove(startAbleQuest);
                questDic.Remove(startAbleQuest.qName);
                dialogManager.StartDialog(dialogXml, startAbleQuest.completeIndex);

                //questManager.CompleteQuest(); 퀘스트 완료 대화 이후에 보상을 지급한다. DialogUI가 Close 될때 실행한다.
                break;
            default:
                break;
        }

    }

    private void OnMouseExit()
    {
       
    }
}
