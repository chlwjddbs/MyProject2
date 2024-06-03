using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICombatable
{
    float AttackDamage { get; }
    float AttackDelay { get; }

    bool MultiAttackAble { get; }

    Collider AttackCollider { get;}

    List<GameObject> AttackedTargets { get;}
}
