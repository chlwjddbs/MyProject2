using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusEffect : MonoBehaviour
{
    public EffectType effectType;
    public string effctName;
    public float duration;
    public IStatusEffect target;


    public virtual void UseEffect()
    {

    }
}
public enum EffectType
{
    Buff,
    Debuff,
}


/*
    �����̻��� ��ũ���ͺ� ������Ʈ�� ���� �� ���� �̻��� ������ Itemó�� ��� �޾Ƽ� ����� ���� ȿ���� �ٸ� �����̻���� �߰� �� �� �ְ� ����� �ش�.
    
    Attack,             //���ݷ� ��ü�� ������ �� ex) ���ݷ� 10% ����, ���ݷ� 20% ���� ��
    Defence,            //���� ��ü�� ������ ��
    Slow,               //�ӵ��� ������ �� ex) �ӵ��� �������ų� ������
    Hold,               //�����ӿ� ������ �� ex) �̵��ӵ��� 0�� �Ǵ� ����. �̵��� �ȵ����� �ൿ�� ������
    Uncontrollable,     //��Ʈ�� �Ұ� ���� ex) ���ϰԴ� ��ų ��� �Ұ����� ���ϰԴ� ���� ���ϵ� ���ۺҰ��� ���� �̻�
    Dotdamage,          //���� �̻� ��ü�� ������� �־� ���������� ������� �� ex)����, ȭ�� ��
*/