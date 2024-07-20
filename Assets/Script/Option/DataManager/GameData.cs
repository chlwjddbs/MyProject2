using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameData : DataManager
{
    public static GameData instance;

    //���� ������
    public UserData userData = new UserData();

    //���Կ� Json������ ���� �� ����� loadData
    //loadData�� ������� �ʰ� ���� userData�� �Բ� ����ϸ�
    //���� �ٲ� �Ѿ���� saveFileManager�� ����Ǹ鼭 ���ϵ��� �ҷ����鼭
    //userData �����Ͱ� �ٲ�⋚���� ������ ���������� �۵����� �ʰ� �������� �ҷ��� ���� ���� �������� ����ȴ�.
    //�׷����� loadData�� ���� ������ ����.
    public UserData loadData = new UserData();

    //public int currentSlot;
    public string slotName = "";
    public SaveSlot selectedSlot;

    public Fader fader;

    public bool newGame = true;

    public bool isSet = false;

    private SaveSlot autoSaveSlot;

    //public Dictionary<int> slotItem;

    public Player player;

    protected void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        //����Ƽ���� �ڵ����� �������ִ� ���
        //path + fileName ���� ��� ������ �ϱ� ���� path�� "/" �߰�
        //C:/Users/������/AppData/LocalLow/DefaultCompany/My project 2 +/Save/ 
        path = Application.persistentDataPath + dirName;

        direcInfo = new DirectoryInfo(path);
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    public override void CreateSaveData()
    {
        base.CreateSaveData();

        string data = JsonUtility.ToJson(userData);
        File.WriteAllText(path + slotName, data);
        selectedSlot.SetSlot();
    }

    public void AutoSaveData()
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
            Debug.Log("�߰�");
        }

        string data = JsonUtility.ToJson(userData);
        File.WriteAllText(path + autoSave, data);
        autoSaveSlot?.SetSlot();
        //Debug.Log(path);
    }

    public void LoadData(string _slotName, SaveSlot _saveSlot)
    {
        if (!File.Exists(path + _slotName))
        {
            //Debug.Log("����� ������ �����ϴ�.");
            return;
        }
        //Debug.Log(_slotName);
        string data = File.ReadAllText(path + _slotName);
        loadData = JsonUtility.FromJson<UserData>(data);
        _saveSlot.userData = loadData;
        //Debug.Log(loadData.userName);
    }


    public void SelectedSlot(string _slotName, SaveSlot _selectedSlot, UserData _userData)
    {
        if (slotName == _slotName)
        {
            Debug.Log("������ư");
            selectedSlot.DeSelectSlot();
            ResetSlotData();
            return;
        }

        Debug.Log("�ٸ���ư");
        try
        {
            selectedSlot.DeSelectSlot();
        }
        catch
        {

        }
        slotName = _slotName;
        selectedSlot = _selectedSlot;
        loadData = _userData;
        selectedSlot.SelectSlot();
    }

    public void ResetSlotData()
    {
        slotName = "";
        selectedSlot = null;
        loadData = new UserData();
    }

    public override void DeleteData()
    {
        //�׷��� ������ ���� ����
        File.Delete(path + slotName);
        selectedSlot.ResetSlot();
        loadData = selectedSlot.userData;
        selectedSlot = null;
    }

    //������ ������ ���������� �����ϴ� �Լ�
    public bool DeleteButtonSelect()
    {
        //���� ������ ����� �� ""���� ����ȴ�. 
        //���� ������ ���� �ʾ������� return;
        if (slotName == "")
        {
            Debug.Log("���õ� ���� ����");
            return false;
        }

        if (slotName == "AutoSave")
        {
            Debug.Log("���� �� �� ���� ����");
            return false;
        }

        //������ ������ ã�´�.
        //������ �̹� �����Ǿ����ϴ� ���
        if (!File.Exists(path + slotName))
        {
            Debug.Log("�̹� �����Ǿ����ϴ�..");
            return false;
        }
        return true;
    }

    public void LoadGame()
    {
        //userData = selectedSlot.GetComponent<SaveSlot>().userData;
        userData = loadData;
        //Time.timeScale = 1;
        fader.SceneLoad(userData.sceneName);
        newGame = false;
        isSet = false;
        ResetSlotData();
        //SceneManager.LoadScene(userData.sceneName);
        Debug.Log(userData.userName);
        //AudioManager.instance.StopBGM(fader.fadeInTime);
    }

    public void GotoMainMenu()
    {
        userData = new UserData();
        loadData = new UserData();
        fader.SceneLoad("MainMenuScene");
    }

    public void GetAutoSaveSlot(SaveSlot _auto)
    {
        autoSaveSlot = _auto;
    }
    public bool SaveButtonEmpty()
    {
        if (loadData.userName == null)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void FindPlayer()
    {
        player = GameObject.Find("ThePlayer").GetComponent<Player>();
    }
}
