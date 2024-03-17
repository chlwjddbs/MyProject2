using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using TMPro;
using System.IO;
using UnityEngine.UI;

public class MainSceneMenu : MainMenu
{
    //���ο� ���� ���۽� �̵��� �� �̸�
    public string newGame = "IntroScene";
    public GameObject continueButton;
    public GameObject ectMenu;
    public NewGamePopupUI newGamePopupUI;
    public GameObject noticePopupUI;
    public GameObject nameConfirmedPopupUI;
    public GameObject saveFileObj;


    public override void Start()
    {
        base.Start();
        if (Directory.Exists(Application.persistentDataPath + "/Save/"))
        {
            if (dataManager.direcInfo.GetFiles().Length == 0)
            {
                continueButton.GetComponent<Button>().interactable = false;
                continueButton.GetComponentInChildren<TextMeshProUGUI>().color = disableColor;
                //Debug.Log("����.");
            }
            else
            {
                continueButton.GetComponent<Button>().interactable = true;
                continueButton.GetComponentInChildren<TextMeshProUGUI>().color = activeColor;
                //Debug.Log("�ִ�.");
            }
        }
        else
        {
            continueButton.GetComponent<Button>().interactable = false;
            continueButton.GetComponentInChildren<TextMeshProUGUI>().color = disableColor;
            Debug.Log("���� ����.");
        }
        //���� �޴��� ���۽� MainMenuBGM ���
        //fader.FadeOut();
        //AudioManager.instance.PlayBGM("MainMenuBGM");
    }

    public void StartGameButton()
    {
        Debug.Log("�����ϱ�");
        MenuOpen(newGamePopupUI.gameObject);
        newGamePopupUI.SetPopup();
        AudioManager.instance.PlayeSound(selectMenu);
        //fader.FadeIn(newGame);
        //SceneManager.LoadScene(newGame);
    }

    public void NewGame()
    {
        dataManager.userData.userName = newGamePopupUI.userName;
        dataManager.userData.saveTime = DateTime.Now.ToString("yyyy-MM-dd [HH:mm]");
        dataManager.userData.currentArea = "ž �Ա�";
        dataManager.userData.currentStage = "1st_Floor";
        dataManager.userData.sceneName = "PlayScene_Floor_1";
        //DataManager.instance.AutoSaveData();       
        AudioManager.instance.PlayeSound(selectMenu);
        dataManager.fader.SceneLoad(newGame);
        dataManager.newGame = true;
        dataManager.isSet = false;
        //SceneManager.LoadScene(newGame);
    }

    public void ContinueGame()
    {
        Debug.Log("�̾��ϱ�");
        MenuOpen(saveFileObj);
        AudioManager.instance.PlayeSound(selectMenu);
    }

    public void EtcButton()
    {
        Debug.Log("���� �Ұ�");
        AudioManager.instance.PlayeSound(selectMenu);
    }

    public override void MenuCloseAll()
    {
        base.MenuCloseAll();
        
        newGamePopupUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        newGamePopupUI.gameObject.SetActive(false);
        noticePopupUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        noticePopupUI.SetActive(false);
        nameConfirmedPopupUI.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        nameConfirmedPopupUI.SetActive(false);

        tempMenu = MainMenuObj;
    }

    public override void MenuOpen(GameObject _selectMenu)
    {      
        MainMenuObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(1920, 0);
        _selectMenu.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        _selectMenu.SetActive(true);
        
        tempMenu = _selectMenu;
    }

    public override void MenuClose(GameObject _closeMenu)
    {
        _closeMenu.SetActive(false);
        MainMenuObj.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        tempMenu = MainMenuObj;
    }

    public void NoticeClose()
    {
        noticePopupUI.SetActive(false);
    }
}
