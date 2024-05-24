using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttack_StatusEffect : IStatusEffect
{
    public void TakeAttackEffect(Attack_StatusEffect _atkEfc);
}
