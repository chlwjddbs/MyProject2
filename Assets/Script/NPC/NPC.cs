using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NPC : MonoBehaviour
{
    private QuestManager questManager;
    private DialogManager dialogManager;
    public NPCInfo npcInfo;

    //플레이어와의 거리
    public float theDistance;

    public List<Quest> npcQuest;

    public string xmlFile;

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
        questManager = QuestManager.instance;
        dialogManager = DialogManager.instance;
        npcQuest = questManager.GetNpcQuest(npcInfo.npcIndex);
        dialogManager.RegistDialogXml(xmlFile);
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
            dialogManager.StartDialog(xmlFile,0);
            return;
        }
    }

    private void OnMouseExit()
    {
       
    }
}
