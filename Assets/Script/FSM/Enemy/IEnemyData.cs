using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyData : IData
{
    float AttackRange { get; }
    float ActionRange { get; }
    float DetectRange { get; }

    int Exp { get; }

    Vector3 StartPoint { get; set; }


}
