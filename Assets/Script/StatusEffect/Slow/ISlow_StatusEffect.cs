using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISlow_StatusEffect : IStatusEffect
{
    public Transform StatusEffectPos { get; }
    public void TakeSlowEffect(Slow_StatusEffect _slowEfc);
}
