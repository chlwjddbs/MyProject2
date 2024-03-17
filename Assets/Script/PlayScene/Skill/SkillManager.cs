using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
    //��ų�� ���� ���� ��ų�� ��Ÿ��
    public float remainingTime;

    //��ų ���� ������� �ɸ��� �ð�
    public float coolTime;

    //�Ҹ𸶳�
    public float cunsumeMana;
    protected PlayerController player;
    public bool isUse;
    public AnimationClip skillMotion;
    public float actionSpeed;

    private void Update()
    {
        if (!isUse)
        {
            remainingTime -= Time.deltaTime;
            if (remainingTime <= 0)
            {
                isUse = true;
            }
        }
    }

    protected virtual void SetSkill()
    {
        isUse = true;
        remainingTime = coolTime;
    }

    public virtual void UseSkill(PlayerController _player)
    {

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

