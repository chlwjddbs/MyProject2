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
    float DetectRange { get; }
    float ReactionRange { get; }

    int Exp { get; }

    Vector3 StartPoint { get; }

    public void TakeDamage(float _damage);
    public void Die();

}
