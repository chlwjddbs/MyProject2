using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupItem : Interaction
{
    public Item item;

    public override void DoAction()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Inventory.instance.GetItem(item);
            
            Debug.Log(item.name + " : ������ ȹ��" ) ;
            Destroy(gameObject);
        }
    }
}
