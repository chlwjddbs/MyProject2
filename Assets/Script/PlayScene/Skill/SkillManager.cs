using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    public int skillLevel;

    //��ų�� ���� ���� ��ų�� ��Ÿ��
    public float remainingTime;

    //��ų ���� ������� �ɸ��� �ð�
    public float coolTime;

    //�Ҹ𸶳�
    public float cunsumeMana;
    [SerializeField]protected Player player;
    public bool isUse;
    public AnimationClip skillMotion;
    public float actionSpeed;

    protected bool useSequenceText;

    protected virtual void Update()
    {
        if (!isUse)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0 && player.RemainMana>= cunsumeMana)
            {
                isUse = true;
            }
        }
    }

    public virtual void SetSkill(Player _player)
    {
        player = _player;
        isUse = true;
        remainingTime = 0;
    }

    public void LoadPlayer(Player _player)
    {
        player = _player;
    }

    public virtual void UseSkill()
    {
        if (player.RemainMana < cunsumeMana)
        {
            isUse = false;
            useSequenceText = false;
            SequenceText.instance.SetSequenceText(null, "NotEnoughMana");
            Invoke("SequenceTextAble", .5f);
            return;
        }
    }

    public void SequenceTextAble()
    {
        useSequenceText = true;
    }

    public void SwapSkill(float _remainingTIme)
    {
        remainingTime = _remainingTIme;
        if (remainingTime <= 0)
        {
            isUse = true;
        }
        else
        {
            isUse = false;
        }
    }
}

