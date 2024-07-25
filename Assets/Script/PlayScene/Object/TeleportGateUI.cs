using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportGateUI : MonoBehaviour
{
    public RectTransform teleportGateUI;
    public bool isOpen;

    private void Awake()
    {
        GateManager.instence.getUI += SetGate;
    }

    void LateUpdate()
    {
        if (!GameData.instance.isSet)
        {
            return;
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            CloseUI();
            //player.GetComponent<PlayerController>().SetState(PlayerState.Idle);
        }

        /*
        if(Input.GetKey(KeyCode.O) && Input.GetKeyDown(KeyCode.P) && !isAllgate)
        {
            isAllgate = true;

            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.activeSelf == true)
                {
                    transform.GetChild(i).gameObject.GetComponent<TeleportGate>().OpenGate();
                }
            }
        }
        */
    }

    public void SetGate()
    {
        GateManager.instence.teleportGateUI = this;
    }

    public void ToggleUI()
    {
        isOpen = !isOpen;
        if (isOpen)
        {
            teleportGateUI.anchoredPosition = new Vector3(0, 0, 0);
        }
        else
        {
            teleportGateUI.anchoredPosition = new Vector3(-800f, 0, 0);
        }

    }

    public void CloseUI()
    {
        isOpen = false;
        teleportGateUI.anchoredPosition = new Vector3(-2000f, 0, 0);
    }

    public void OpenUI()
    {
        isOpen = true;
        teleportGateUI.anchoredPosition = new Vector3(0, 0, 0);
        //player.GetComponent<PlayerController>().SetState(PlayerState.Action);
        //PlayerController.isAction = true;
    }

    public bool UIOpenCheck()
    {
        return isOpen;
    }
}
