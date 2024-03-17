using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Skill", menuName = "Skill")]
public class SkillItem : Item
{
    public string skillName;
    public SkillType skillType;

    public GameObject skill;

    public Sound skillSound;
   
    public override void Use(int slotNum)
    {
        SkillBook.instance.LearnSkill(this);
    }
}

public enum SkillType
{
    Attack,          //���� ��ų 
    Defence = 1000,  //��� ��ų
    Buff = 2000,     //���� ��ų
    Utile = 3000,    //��ƿ ��ų
}
