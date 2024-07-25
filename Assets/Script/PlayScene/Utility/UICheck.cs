using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UICheck : MonoBehaviour//, IPointerExitHandler, IPointerEnterHandler
{
    private Player player;
    private void Awake()
    {
        GameObject findPlayer = GameObject.Find("ThePlayer");
        if (findPlayer != null)
        {
            player = findPlayer.GetComponent<Player>();
        }
        else
        {
            Debug.Log("현재 Scene에 Player가 없습니다.");
        }
    }
    
    private void Update()
    {
        if (!GameData.instance.isSet)
        {
            return;
        }

        if(player == null)
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
