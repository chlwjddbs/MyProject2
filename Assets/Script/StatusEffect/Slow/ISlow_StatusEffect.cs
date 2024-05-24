using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISlow_StatusEffect : IStatusEffect
{
    public void TakeSlowEffect(Slow_StatusEffect _slowEfc);
}
