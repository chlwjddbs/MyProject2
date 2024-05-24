using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDefence_StatusEffect : IStatusEffect
{
    public void TakeDefenceEffect(Defence_StatusEffect _dfcEfc);
}
