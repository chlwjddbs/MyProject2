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
    상태이상을 스크럽터블 오브젝트로 구현 후 상태 이상의 종류는 Item처럼 상속 받아서 만들어 각자 효과가 다른 상태이상들을 추가 할 수 있게 만들어 준다.
    
    Attack,             //공격력 자체에 영향을 줌 ex) 공격력 10% 증가, 공격력 20% 감소 등
    Defence,            //방어력 자체에 영향을 줌
    Slow,               //속도에 영향을 줌 ex) 속도가 느려지거나 빨라짐
    Hold,               //움직임에 영향을 줌 ex) 이동속도가 0이 되는 상태. 이동은 안되지만 행동은 가능함
    Uncontrollable,     //컨트롤 불가 상태 ex) 약하게는 스킬 사용 불가에서 강하게는 빙결 스턴등 조작불가의 상태 이상
    Dotdamage,          //상태 이상 자체에 대미지가 있어 지속적으로 대미지를 줌 ex)출혈, 화상 등
*/