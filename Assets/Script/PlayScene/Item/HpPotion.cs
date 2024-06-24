using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "new Posion",menuName = "Item/Posion/Hp_Posion")]
public class HpPotion : Potion
{
    public float recoveryPoint;

    public override void Use(int slotNum, Player player)
    {
        player.RecoveryHP(recoveryPoint);
    }
}