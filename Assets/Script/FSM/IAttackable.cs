using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttackable
{
    public bool IsAttackable { get; }

    float StartHealth { get; }
    float MaxHealth { get; }
    float RemainHealth { get; }

    float DefencePoint { get; }

    Collider HitBox { get; }


    public void TakeDamage(float _damage, Transform _attacker, Vector3 _damagedDir = new Vector3())
    {

    }


    public void Die();
}
