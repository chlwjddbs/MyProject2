using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface EnemyStatusDataInterface : DataInterface
{
    float StartHealth { get; }
    float MaxHealth { get; }
    float CurrentHealth { get; }

    float AttackDamage { get; }
    float DefencePoint { get; }

    float AttackRange { get; }
    float ActionRange { get; }
    float DetectRange { get; }

    int Exp { get; }

    Vector3 StartPoint { get; set; }

    public void TakeDamage(float _damage , Transform _attacker , Vector3 _damagedDir);
    public void Die();

}
