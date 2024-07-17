using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillBookUI : MonoBehaviour
{
    private ControllOption controllOption;

    public GameObject skillBookUI;
    private RectTransform skillBookUIrect;
    private bool isOpen;

    public Sound[] skillboolSound;

    public KeyOption keyOption;

    private void Start()
    {
        foreach (var s in skillboolSound)
        {
            AudioManager.instance.AddExternalSound(s);
        }
    }

    public void SetData()
    {
        skillBookUIrect = skillBookUI.GetComponent<RectTransform>();
        controllOption = OptionManager.instance.controllOption;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!GameData.instance.isSet)
        {
            return;
        }

        if (GameSceneMenu.isMenuOpen)
        {
            return;
        }

        ToggleUI();
        /*
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            skillBookUI.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            skillBookUI.SetActive(!skillBookUI.activeSelf);
        }
        */
    }

    public void ToggleUI()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (isOpen)
            {
                CloseUI();
                AudioManager.instance.PlayExternalSound("closeSkillbook");
            }
        }
        

        if (Input.GetKeyDown(controllOption.bindKey_Dic[keyOption].bindKey))
        {
            isOpen = !isOpen;
            if (isOpen)
            {
                skillBookUIrect.anchoredPosition = new Vector3(0, 0, 0);
                AudioManager.instance.PlayExternalSound("openSkillbook");
            }
            else
            {
                skillBookUIrect.anchoredPosition = new Vector3(-1920f, 0, 0);
                AudioManager.instance.PlayExternalSound("closeSkillbook");
            }
        }
    }

    public void CloseUI()
    {
        isOpen = false;
        skillBookUIrect.anchoredPosition = new Vector3(-1920f, 0, 0);
    }

    public void OpenUI()
    {
        isOpen = true;
        skillBookUIrect.anchoredPosition = new Vector3(0, 0, 0);
    }

    public bool UIOpenCheck()
    {
        return isOpen;
    }
}
