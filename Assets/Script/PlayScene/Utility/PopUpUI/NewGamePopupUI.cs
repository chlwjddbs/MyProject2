using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.Localization.Components;

public class NewGamePopupUI : MonoBehaviour
{
    public TMP_InputField inputField;

    //���� �� Ư�����ڸ� �ɷ����ִ� ���Խ� ����
    private string nameCheck = @"[^a-zA-Z��-����-�R0-9��-��]";
  
    //@"[^a-zA-Z��-����-�R0-9��-��]"
    //@ : ���Խ� ǥ�� �� ���� ��� ������������
    //�齽����(\) �� �Է½� escape�� ������ �����ϱ� ������ \�� ���ڿ��� ���� ���ؼ��� \\ �̶�� ����ؾ��Ѵ�.
    //������ �׷��� �������� �������ų� ������ �߱��ϱ� ������ @�� �ٿ��ָ� \�� ���ڿ��� �ѹ��� �ν��ϵ��� ���ش�.
    //ex) @"\bAA\b" == "\\bAA\\b"

    //a-z �ҹ��� ���ĺ�
    //A-Z �빮�� ���ĺ�
    //��-�� ����
    //��-�� ����
    //��-�R �ѱ�
    //0-9 0~9 �� ��Ÿ���� ����

    // [] ���ȣ�ȿ� �ִ� ���ڸ� ã�´�.
    //ex) [a-z] �ҹ��� ���ĺ��� ã�´�. [G] �빮�� ���ĺ� G�� ã�´�.

    //[^] ���ȣ���� ^ ���� ������ ���Ѵ�.
    //ex) [^a-z] �ҹ��� ���ĺ��� �����ϰ� ã�´�. [^G] �빮�� ���ĺ�G�� �����ϰ� ã�´�.

    //���� �����Ͽ� [^a-zA-Z��-����-�R0-9��-��] �� ����� Ư�����ڸ� ���Ѵ�.
    //�� ��Ȯ���� ��� ���ĺ��� �ѱ� �� ���ڸ� ������ ���ڸ� ���Ѵ�.
    //nameCheck�� ���� �� Ư�����ڸ� �ɷ��ִ� ���Խ� ������ �ȴ�.

    public GameObject nameConfirmedPopupUI;

    public GameObject noticePopupUI;
    //�ȳ� �˾�â�� ��Ȳ�� �°� ���� ���� LocalizeStringEvent ����.
    public LocalizeStringEvent noticePopupText;

    private DataManager dataManager;

    public string userName;

    
    private void Awake()
    {
        dataManager = DataManager.instance;
        //�ȳ� �˾�â�� MainMenu ���̺� �ִ� entry�� ��� �Ұ�.
        noticePopupText.StringReference.TableReference = "MainMenu";
    }

    public void SetPopup()
    {
        inputField.ActivateInputField();
        inputField.text = "";
    }

    public void OkButton()
    {
        if (inputField.text.Length == 0)
        {
            noticePopupUI.SetActive(true);
            noticePopupText.StringReference.TableEntryReference = "NameEnter";
            Debug.Log("���̵� �Է��ϼ���");
            return;
        }

        NameCheck();
    }

    public void NameCheck()
    {
        if (Regex.IsMatch(inputField.text, nameCheck))
        {
            Debug.Log("Ư�� ���� �� ������ ��� �� �� �����ϴ�.");
            noticePopupUI.SetActive(true);
            noticePopupText.StringReference.TableEntryReference = "NameNotaVailable";
        }
        else
        {
            Debug.Log("���� ����");
            userName = inputField.text;
            nameConfirmedPopupUI.SetActive(true);
        }
    }

    public void CancelButton()
    {
        //����ϱ�� MainMenu.MenuClose �� �����Ͽ� ���� �Ϸ� �Ͽ���.
    }

    public void OnDisable()
    {
        nameConfirmedPopupUI.SetActive(false);
        noticePopupUI.SetActive(false);
    }
}
