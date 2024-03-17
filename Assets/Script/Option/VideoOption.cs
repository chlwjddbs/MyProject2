using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;


public class VideoOption : MonoBehaviour
{
    //OptionManager�� �̱���
    private OptionManager optionManager;

    //��� ������ �ػ� ����� ǥ������ DropDown
    public TMP_Dropdown resDropdown;

    //���� ȭ�� ���
    public FullScreenMode screenMode;

    //��ũ�� ����� �ʱ� ��
    private int screenInitial = 0;
    //���õ� ��ũ�� ����� ��
    public int scrModeNum;

    //�ʱ� DropDown�� Value ��
    private int resInitial = 0;
    //���õ� DropDown�� Value
    public int resVal = 0;
    
    //��ũ����� ����� ������ ��� �׷�
    public ToggleGroup screenModeGroup;

    //�ػ󵵸� ���� �������� ������ �ֱ� ���� �ӽ� Resolution List
    private List<Resolution> tempResolutions = new List<Resolution>();

    //��� �ػ� ���
    private List<Resolution> resolutionsAll = new List<Resolution>();
    //16:9 �ػ� ���
    private List<Resolution> resolutions16_9 = new List<Resolution>();
    //16:10 �ػ� ���
    private List<Resolution> resolutions16_10 = new List<Resolution>();

    //���� ������� �ػ� ���
    private List<Resolution> currnetResolutions = new List<Resolution>();

    //�ػ� ����� �ʱⰪ
    private int ratioInitial = 0;
    //���� �ػ� ���
    //public string str_Ratio = "All";
    public int ratioNum;
    
    //�ػ� ��� ��� ����� ������ ��� �׷�
    public ToggleGroup ratioGroup;

    //��� ȭ���
    private List<TMP_Dropdown.OptionData> ratioDataAll = new List<TMP_Dropdown.OptionData>();
    private List<TMP_Dropdown.OptionData> ratioData_16_9 = new List<TMP_Dropdown.OptionData>();
    private List<TMP_Dropdown.OptionData> ratioData_16_10 = new List<TMP_Dropdown.OptionData>();

    //ss set start
    private bool ssScrMode = true;
    private bool ssRatio = true;

    private void Awake()
    {
        //��Ӵٿ� �ɼ��� List�� �Ǿ� �־ Clear �Լ��� ���� �ʱ�ȭ ���ش�.
        resDropdown.options.Clear();

        //�ػ� ����� ���� �������� �����ֱ� ���� Reverse���
        tempResolutions = new List<Resolution>(Screen.resolutions);
        tempResolutions.Reverse();

        //���� ������� �ִ� �ֻ����� �����Ѵ�.
        int maxRefreshRate = tempResolutions[0].refreshRate;
       
        //��� ������ �ػ� ����߿� �ִ� �ֻ��� �ػ󵵸� Dropdown�� �־��ش�.
        foreach (Resolution res in tempResolutions)
        {
            if (res.refreshRate == maxRefreshRate)
            {
                //Dropdown�� �ֱ� ���ؼ��� ������ ������ OptionData�̱� ������ ���� ������ �ش�.
                TMP_Dropdown.OptionData resData = new TMP_Dropdown.OptionData();

                //�ػ��� ������ �����ش�.
                float aspectRatio = (float)res.width / res.height;
                string aspectRatioString = aspectRatio.ToString("F2");
                float aspectRatioF2 = float.Parse(aspectRatioString);

                //DropDownMenu �� ǥ�ÿ� string
                string DDM_aspectRatio;

                //������ ���� string �ο�
                switch (aspectRatioF2)
                {
                    case (1.50f):
                        DDM_aspectRatio = "[3:2]";
                        break;

                    case (1.33f):
                        DDM_aspectRatio = "[4:3]";
                        break;

                    case (1.25f):
                        DDM_aspectRatio = "[5:4]";
                        break;

                    case (1.77f):
                    case (1.78f):                       
                        DDM_aspectRatio = "[16:9]";
                        resData.text = (res.width + " x " + res.height + "  " + DDM_aspectRatio);
                        ratioData_16_9.Add(resData);
                        resolutions16_9.Add(res);
                        break;

                    case (1.56f):
                    case (1.60f):
                    case (1.67f):
                        DDM_aspectRatio = "[16:10]";
                        resData.text = (res.width + " x " + res.height + "  " + DDM_aspectRatio);
                        ratioData_16_10.Add(resData);
                        resolutions16_10.Add(res);
                        break;

                    default:
                        DDM_aspectRatio = " ";
                        break;
                }
                
                //������ȭ ��ų �ػ󵵸� width,height �� ������ ǥ���� �ش�. ex) 2560 x 1440 [16:9]
                resData.text = (res.width + " x " + res.height + "  " + DDM_aspectRatio);
                ratioDataAll.Add(resData);
                //������� OptionData�� dropdown.options.Add(data)�� ���� �־��ش�.
                resDropdown.options.Add(resData);
                //Debug.Log(res.width + " x " + res.height + " " + res.refreshRate);
                //Debug.Log(res.width + " x " + res.height + " " + DDM_aspectRatio);  
                currnetResolutions.Add(res);
                resolutionsAll.Add(res);
            }
        }       
    }

    private void Start()
    {
        optionManager = OptionManager.instance;      
    }

    public void GameStart()
    {
        if (PlayerPrefs.HasKey("VideoOption"))
        {
            LoadOption();
        }
        else
        {
            Debug.Log(scrModeNum);
            scrModeNum = screenInitial;
            Debug.Log(scrModeNum);
            ratioNum = ratioInitial;
            resVal = resInitial;
        }
        StartScreenMode(scrModeNum);
        StartRatio(ratioNum);
        StartResolution(resVal);

    }

    public void SaveOption()
    {
        PlayerPrefs.SetString("VideoOption", "Save");
        PlayerPrefs.SetInt("ScreenMode", scrModeNum);
        PlayerPrefs.SetInt("Ratio", ratioNum);
        PlayerPrefs.SetInt("Resolution", resVal);
    }

    public void LoadOption()
    {
        scrModeNum = PlayerPrefs.GetInt("ScreenMode", scrModeNum);
        ratioNum = PlayerPrefs.GetInt("Ratio", ratioNum);
        resVal = PlayerPrefs.GetInt("Resolution", resVal);
    }

    //Dropdown�� On Value Changed�� ���� �����Ͽ� Dropdown�� ���� �� �� ���� �ػ󵵸� �ٲ��ش�.
    //ex) Dropdown�� 3���� �ػ� ����� ������ �� 2��° �ػ󵵸� �����ϸ� Value�� 1�� �ȴ�.
    //���� �� Value�� ���� ���� private Resolution[] resolutions�� ����ִ� �ػ� ��ϰ� ����.
    //���� �� Value�� ���� resolutions�� �����ϸ� ������ �ػ󵵷� ���� �����ϴ�.
    public void ChangeResolution()
    {
        optionManager.tmp_ResVale.Add(resVal);
        optionManager.relutions.Add(currnetResolutions[resVal]);
        //������ Dropdown�� Value���� �޴´�.
        resVal = resDropdown.value;
        //�Ѱܹ��� value�� ���� resolution�� �ִ� �ػ󵵿� �����Ͽ� �ػ󵵸� �����Ѵ�.
        Screen.SetResolution(currnetResolutions[resVal].width, currnetResolutions[resVal].height, screenMode);
        Debug.Log(currnetResolutions[resVal].width +" x " + currnetResolutions[resVal].height + " " + screenMode);
        optionManager.ToggleSaveButton(true);
        Debug.Log("�ػ� ����");
    }

    //��۷κ��� string �Ű� ������ �޾ƿ� switch�� ���� ��ũ�� ��� ����
    public void ChangeScreenMode(int _screenMode)
    {
        if (ssScrMode)
        {
            ssScrMode = false;
            return;
        }
        Debug.Log(_screenMode);
        if (scrModeNum == _screenMode | !screenModeGroup.transform.GetChild(_screenMode).GetComponent<Toggle>().isOn)
        {
            return;
        }
        Debug.Log(_screenMode);
        optionManager.tmp_Screen.Add(scrModeNum);
        //tmp_ScreenMode = screenMode.ToString();

        SetScreenMode(_screenMode);

        optionManager.ToggleSaveButton(true);
    }

    public void SetScreenMode(int _screenMode)
    {      
        switch (_screenMode)
        {
            case (0):
                scrModeNum = 0;
                screenMode = FullScreenMode.ExclusiveFullScreen;          

                break;
            case (1):
                scrModeNum = 1;
                screenMode = FullScreenMode.FullScreenWindow;
                break;
            case (2):
                scrModeNum = 2;
                screenMode = FullScreenMode.Windowed;
                break;
            default:
                break;
        }

        Screen.SetResolution(currnetResolutions[resVal].width, currnetResolutions[resVal].height, screenMode);
    }

    /*
    public void ChangeRatio(int _ratio)
    {
        if (!ratioGroup.transform.GetChild(_ratio).GetComponent<Toggle>().isOn)
        {
            return;
        }
        Debug.Log(_ratio);
        if (ratioNum == _ratio)
        {
            return;
        }
        Debug.Log("����");
        optionManager.tmp_Ratio.Add(ratioNum);
        //tmp_Ratio = str_Ratio;

        switch (_ratio)
        {
            case (0):
                ratioNum = 0;
                currnetResolutions.Clear();
                resDropdown.ClearOptions();
                currnetResolutions = resolutionsAll.ToList();
                foreach (TMP_Dropdown.OptionData data in ratioDataAll)
                {
                    resDropdown.options.Add(data);
                    //Debug.Log(data.text + screenMode);
                }
                break;

            case (1):
                ratioNum = 1;
                currnetResolutions.Clear();
                resDropdown.ClearOptions();
                currnetResolutions = resolutions16_9.ToList();
                foreach (TMP_Dropdown.OptionData data in ratioData_16_9)
                {
                    resDropdown.options.Add(data);
                    //Debug.Log(data.text + screenMode);
                }
                break;

            case (2):
                ratioNum = 2;
                currnetResolutions.Clear();
                resDropdown.ClearOptions();
                currnetResolutions = resolutions16_10.ToList();
                foreach (TMP_Dropdown.OptionData data in ratioData_16_10)
                {
                    resDropdown.options.Add(data);
                    //Debug.Log(data.text + screenMode);
                }
                break;
            
            default:
                break;
        }

        //str_Ratio = _ratio;
        resDropdown.RefreshShownValue();
        //Debug.Log(currnetResolutions.Count);        
        //ChangeResolution();
        optionManager.ToggleSaveButton(true);
    }
    */

    public void ChangeRatio(int _ratio)
    {
        if (ssRatio)
        {
            ssRatio = false;
            return;
        }

        if (ratioNum == _ratio | !ratioGroup.transform.GetChild(_ratio).GetComponent<Toggle>().isOn)
        {
            return;
        }

        optionManager.tmp_Ratio.Add(ratioNum);
        SetRatioDropBox(_ratio);
        ChangeResolution();
        optionManager.ToggleSaveButton(true);

    }

    public void SetRatioDropBox(int _ratio)
    {
        switch (_ratio)
        {
            case (0):
                ratioNum = 0;
                currnetResolutions.Clear();
                resDropdown.ClearOptions();
                currnetResolutions = resolutionsAll.ToList();
                foreach (TMP_Dropdown.OptionData data in ratioDataAll)
                {
                    resDropdown.options.Add(data);
                }
                break;

            case (1):
                ratioNum = 1;
                currnetResolutions.Clear();
                resDropdown.ClearOptions();
                currnetResolutions = resolutions16_9.ToList();
                foreach (TMP_Dropdown.OptionData data in ratioData_16_9)
                {
                    resDropdown.options.Add(data);
                }
                break;

            case (2):
                ratioNum = 2;
                currnetResolutions.Clear();
                resDropdown.ClearOptions();
                currnetResolutions = resolutions16_10.ToList();
                foreach (TMP_Dropdown.OptionData data in ratioData_16_10)
                {
                    resDropdown.options.Add(data);
                }
                break;

            default:
                break;
        }
        resDropdown.RefreshShownValue();
    }

    public void CancelOption(int _screenMode, int _resVale, int _ratio)
    {
        if (_screenMode != -1)
        {
            for (int i = 0; i < screenModeGroup.transform.childCount; i++)
            {
                if (i == _screenMode)
                {
                    screenModeGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = true;
                }
                else
                {
                    screenModeGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = false;
                }
            }
        }

        if (_ratio != -1)
        {
            for (int i = 0; i < ratioGroup.transform.childCount; i++)
            {
                if (i == _ratio)
                {
                    ratioGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = true;                 
                }
                else
                {
                    ratioGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = false;
                }
            }
        }

        if (_resVale != -1)
        {           
            resDropdown.value = _resVale;            
        }
    }

    public void ResetOption()
    {
        for (int i = 0; i < screenModeGroup.transform.childCount; i++)
        {
            //screenModeGroup.gameObject.transform.GetChild(i).GetComponent<Toggle>().isOn = false;
            if(i == screenInitial)
            {
                screenModeGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = true;
            }
            else
            {
                screenModeGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = false;
            }
        }

        for (int i = 0; i < ratioGroup.transform.childCount; i++)
        {
            if(i == ratioInitial)
            {
                ratioGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = true;
            }
            else
            {
                ratioGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = false;
            }
        }

        resDropdown.value = resInitial;
    }

    public void StartResolution(int _resVal)
    {
        resVal = _resVal;
        resDropdown.value = resVal;
    }

    //��۷κ��� string �Ű� ������ �޾ƿ� switch�� ���� ��ũ�� ��� ����
    public void StartScreenMode(int _screenMode)
    {
        if (_screenMode != -1)
        {
            for (int i = 0; i < screenModeGroup.transform.childCount; i++)
            {
                if (i == _screenMode)
                {
                    screenModeGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = true;
                }
                else
                {
                    screenModeGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = false;
                }
            }
        }

        switch (_screenMode)
        {
            case (0):
                scrModeNum = 0;
                screenMode = FullScreenMode.ExclusiveFullScreen;

                break;
            case (1):
                scrModeNum = 1;
                screenMode = FullScreenMode.FullScreenWindow;
                break;
            case (2):
                scrModeNum = 2;
                screenMode = FullScreenMode.Windowed;
                break;
            default:
                break;
        }
    }

    public void StartRatio(int _ratio)
    {
        for (int i = 0; i < ratioGroup.transform.childCount; i++)
        {
            if (i == _ratio)
            {
                ratioGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = true;
                

            }
            else
            {
                ratioGroup.transform.GetChild(i).GetComponent<Toggle>().isOn = false;
            }
        }

        SetRatioDropBox(_ratio);
        /*
        switch (_ratio)
        {
            case (0):
                ratioNum = 0;
                currnetResolutions.Clear();
                resDropdown.ClearOptions();
                currnetResolutions = resolutionsAll.ToList();
                foreach (TMP_Dropdown.OptionData data in ratioDataAll)
                {
                    resDropdown.options.Add(data);
                }
                break;

            case (1):
                ratioNum = 1;
                currnetResolutions.Clear();
                resDropdown.ClearOptions();
                currnetResolutions = resolutions16_9.ToList();
                foreach (TMP_Dropdown.OptionData data in ratioData_16_9)
                {
                    resDropdown.options.Add(data);
                }
                break;

            case (2):
                ratioNum = 2;
                currnetResolutions.Clear();
                resDropdown.ClearOptions();
                currnetResolutions = resolutions16_10.ToList();
                foreach (TMP_Dropdown.OptionData data in ratioData_16_10)
                {
                    resDropdown.options.Add(data);
                }
                break;

            default:
                break;
        }

        resDropdown.RefreshShownValue();
        */
    }

}
