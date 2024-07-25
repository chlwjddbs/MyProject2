using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "new Ingredient", menuName = "Item/Ingredient")]
public class Ingredient : Item , IOverlapItem
{
    public int ownershipLimit;

    public int quntity = 0;

    public int OwnershipLimit { get { return ownershipLimit; } }

    public int Quntity { get { return quntity; } }

    public override void Use(int slotNum, Player player)
    {
        //
    }
}
