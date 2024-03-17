using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class OptionManager : MonoBehaviour
{
    public static OptionManager instance;

    private void Awake()
    {
        /*
        if(instance != null)
        {
            Destroy(this.gameObject);
            return;
        }        
        DontDestroyOnLoad(gameObject);
        */
        instance = this;
    }

    public VideoOption videoOption;
    public AudioOption audioOption;
    public LanguageOption languageOption;

    private string screenMode = null;

    //tmp_xxx : ���� ���� �� ��Ҹ� �ϰų� �����Ͽ� ������ �״�� ������ ������ �Ѵ�. ��� ���ÿ� ���� ������ ������� �ϱ� ������ ���� �ߴ� �ɼ��� �����Ѵ�.

    public List<int> tmp_Screen = new List<int>();
    public int resolutionValue = 0;
    public List<int> tmp_ResVale = new List<int>();

    public List<Resolution> relutions = new List<Resolution>();
    
    public string ratio = "All";
    public List<int> tmp_Ratio;

    public List<int> tmp_MasterVol;
    public List<int> tmp_BgmVol;
    public List<int> tmp_SfxVol;
    public List<int> tmp_AmbVol;

    //�ɼ� ���� �� ������ �ɼ��� �����ϰų� ����ϱ� �� �������� ���� ǥ��
    public bool isChange = false;

    public Button saveButton;
    
    private void Start()
    {
        //Video�ɼ��� ScreenMode �� �ɼ��� ����� ��� ���� ���� �Ǹ� �ڵ����� OnChangeValue�� ���� SaveButton�� Ȱ��ȭ ���θ� üũ�ϰԵȴ�.
        //SaveButton�� SettingMenuUI�� �ڽ������� SettingMenuUI�� ���������� ��ư�� �ùٸ��� �������� ���Ѵ�.
        //�׷��� SettingMenuUI�� UI�ۿ� �о�״� ������ ������ �� ���� ��ġ�� ���� ���� �ְ� ���ش�.       
        GameStart();
    }

    public void GameStart()
    {
        videoOption.GameStart();
        audioOption.GameStart();
        languageOption.GameStart();

        //����� ������ �ҷ����鼭 ���� ���� �ٲ�µ� �ٲ� ���� �ӽ� ����Ʈ�� ����Ȱ� �����ش�.
        ClearList();
        //SettingMenuUI.SetActive(false);

        DataManager.instance.isSet = true;
    }

    public void SaveOption()
    {
        if (!isChange)
        {
            return;
        }

        ClearList();
        Save();


        //screenMode = videoOption.tmp_Screen;
        //resolutionValue = videoOption.resVal;
        //ratio = videoOption.tmp_Ratio;
    }

    public void Save()
    {
        videoOption.SaveOption();
        audioOption.SaveOption();

    }

    //SaveButton�� Ȱ��, ��Ȱ��ȭ ���� ���θ� �����ش�.
    public void ToggleSaveButton(bool _isChane)
    {
        isChange = _isChane;
        if (isChange)
        {
            saveButton.GetComponent<MenuButton>().ChangeButtonState(true);
        }
        else
        {
            saveButton.GetComponent<MenuButton>().ChangeButtonState(false);
        }
    }

    public void CancelOption()
    {
        if (!isChange)
        {
            return;
        }

        /*
        videoOption.resDropdown.value = tmp_ResVale[0];
        videoOption.ChangeResolution();
        */
        
        videoOption.CancelOption(tmp_Screen.Count == 0? -1 : tmp_Screen[0], tmp_ResVale.Count ==0? -1 : tmp_ResVale[0], tmp_Ratio.Count == 0? -1 : tmp_Ratio[0]);
        audioOption.ChangeOption("Cancel");
        ClearList();
        //videoOption.ChangeRatio(ratio);
    }

    public void ResetOption()
    {
        videoOption.ResetOption();
        audioOption.ChangeOption("Reset");
        ClearList();
        Save();
    }

    public void ClearList()
    {
        tmp_Screen.Clear();
        tmp_ResVale.Clear();
        tmp_Ratio.Clear();
        relutions.Clear();
        tmp_MasterVol.Clear();
        tmp_BgmVol.Clear();
        tmp_SfxVol.Clear();
        tmp_AmbVol.Clear();

        ToggleSaveButton(false);
    }

}
