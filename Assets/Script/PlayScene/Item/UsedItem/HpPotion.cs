using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "new Used",menuName = "Item/Used/Hp_Posion")]
public class HpPotion : Used
{
    public float recoveryPoint;

    public override void Use(int slotNum, Player player)
    {
        player.RecoveryHP(recoveryPoint);
    }
}