using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Localization;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Events;

public class LanguageOption : MonoBehaviour
{
    public static UnityAction setSlot;
    public static UnityAction SortingUI;

    public TMP_Dropdown LanguageList;
    public TextMeshProUGUI currentLanguageText;

    public int selectedLanguageNum = 0;
 

    //��� ����Ʈ ��Ӵٿ� ���� �� ���� �� ���� ����
    public void ChangeLanguage()
    {
        //Debug.Log("����");
        //��� �ٿ� ���� �� �ҷ����� �Լ������� LanguageList.value�� ���� ���� �� ��� �ɼ�
        selectedLanguageNum = LanguageList.value;
        //���� �� �ɼ��� �������ش�.
        PlayerPrefs.SetInt("LanguageOption", selectedLanguageNum);
        //Debug.Log(selectedLanguageNum + " " + LanguageList.options[selectedLanguageNum].text);
        
        //�� ���� �ʿ� ���� ���� ��� ǥ�� UI Text�� �������ش�.
        currentLanguageText.text = LanguageList.options[selectedLanguageNum].text;

        StartCoroutine("LoadingLanguage");
        
        //���� �� ��� ����
        //setSlot?.Invoke();
    }

    IEnumerator LoadingLanguage()
    {
        LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[selectedLanguageNum];
        yield return LocalizationSettings.InitializationOperation;
        //Debug.Log("�ε�");
        setSlot?.Invoke();
        SortingUI?.Invoke();
    }

    /*
    public void SetLanguage()
    {
        Debug.Log(LocalizationSettings.AvailableLocales.Locales.Count);
    }
    */
    public void GameStart()
    {
        StartCoroutine(SetLanguage());
    }

    IEnumerator SetLanguage()
    {
        LanguageList.ClearOptions();
        //Debug.Log(LocalizationSettings.AvailableLocales.Locales.Count);

        //����ȭ ��� ����Ʈ �ε��� �Ϸ� �ɶ����� ��ٷ� �ش�.
        yield return LocalizationSettings.InitializationOperation;
        //����ȭ ��� ����Ʈ�� ����
        //Debug.Log(LocalizationSettings.AvailableLocales.Locales.Count);
        //������ ���
        //Debug.Log(LocalizationSettings.SelectedLocale);
        //����ȭ ��� ����Ʈ�� ù��° �����ڵ��� ���ڿ�
        //Debug.Log(LocalizationSettings.AvailableLocales.Locales[0].ToString());

        //����� �� �ִٸ�
        if (PlayerPrefs.HasKey("LanguageOption"))
        {
            //����� �� ���õ� ���� ����.
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[PlayerPrefs.GetInt("LanguageOption")];
        }
        
        //����Ʈ�� ���� ��ŭ �ݺ����� �����Ͽ�
        for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
        {
            TMP_Dropdown.OptionData dropText = new TMP_Dropdown.OptionData();

            //��� ����Ʈ�� �� �������ش�.
            switch (LocalizationSettings.AvailableLocales.Locales[i].ToString())
            {
                //����Ʈ[0]�� ���� �ڵ尡 "English (en)" ��� ����� ����
                case "English (en)":
                    //����Ʈ dropbox�� English �ɼ� ���
                    dropText.text = "English";
                    LanguageList.options.Add(dropText);

                    //����Ʈ[0]�� �ִ� �� ���� �� ���(����)���
                    if(LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[i])
                    {
                        //���� ���õ� ��� ǥ�� UI�� English test ����
                        currentLanguageText.text = dropText.text;
                        //���õ� ����� ����Ʈ �ѹ� ���
                        selectedLanguageNum = i;
                    }
                    break;
                case "Korean (ko)":
                    dropText.text = "�ѱ���";
                    LanguageList.options.Add(dropText);
                    if (LocalizationSettings.SelectedLocale == LocalizationSettings.AvailableLocales.Locales[i])
                    {
                        currentLanguageText.text = dropText.text;
                        selectedLanguageNum = i;
                    }
                    break;
                default:
                    break;
            }
        }

        //���õ� ���� ���̱� ���� ��Ӵٿ� �ɼ� ����
        //��Ӵٿ�� ���� ���� ���� ����(���� �� �ʿ䰡 ����)
        if (LanguageList.value != selectedLanguageNum)
        {
            //Debug.Log("����");
            LanguageList.value = selectedLanguageNum;
            //setSlot?.Invoke();
        }
        else
        {
            ChangeLanguage();
        }
    }
}
