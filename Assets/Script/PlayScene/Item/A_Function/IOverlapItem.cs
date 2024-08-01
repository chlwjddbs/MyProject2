using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IOverlapItem
{
    public int OwnershipLimit { get; }

    public int Quntity { get; }
}
