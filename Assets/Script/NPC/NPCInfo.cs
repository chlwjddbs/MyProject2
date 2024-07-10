using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class NPCInfo
{
    public int npcIndex;
    public string npcName;
    public NpcType npcType;
}
public enum NpcType
{
    Merchant,
    BlackSmith,
    QuestGiver,
}