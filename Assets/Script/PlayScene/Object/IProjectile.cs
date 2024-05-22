using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IProjectile
{
    public void SetTarget(Transform _target, Vector3 _startPos, float _attackDmg) { }
}
