using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Damageable
{
    public bool IsDamageable { get; }
    public void TakeDamage(float _damage, Transform _attacker, Vector3 _damagedDir = new Vector3())
    {

    }
}
