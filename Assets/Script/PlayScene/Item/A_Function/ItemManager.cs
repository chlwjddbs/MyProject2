using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager instance;
    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SetData();
    }

    public List<Item> gameItems;

    public Dictionary<int, Item> itemManage = new Dictionary<int, Item>();

    public void SetData()
    {
        foreach (var item in gameItems)
        {
            itemManage[item.itemNumber] = item;
        }
    }
}
