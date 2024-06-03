using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UICheck : MonoBehaviour//, IPointerExitHandler, IPointerEnterHandler
{
    private Player player;
    private void Awake()
    {
        player = GameObject.Find("ThePlayer").GetComponent<Player>();
    }
    
    private void Update()
    {
        if (!GameData.instance.isSet)
        {
            return;
        }

        if (player.isUI && !EventSystem.current.IsPointerOverGameObject())
        {
            player.isUI = false;
        }
        else if(!player.isUI && EventSystem.current.IsPointerOverGameObject())
        {
            player.isUI = true;
        }
    }

    /*
    private void OnDisable()
    {
        PlayerController.isUI = false;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {         
        PlayerController.isUI = true;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        PlayerController.isUI = false;
    }
    */
    
}
