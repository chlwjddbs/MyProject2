using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameData : DataManager
{
    public static GameData instance;

    //유저 데이터
    public UserData userData = new UserData();

    //슬롯에 Json정보를 받을 때 사용할 loadData
    //loadData를 사용하지 않고 위의 userData와 함께 사용하면
    //씬이 바뀌어서 넘어갔을시 saveFileManager가 실행되면서 파일들을 불러오면서
    //userData 데이터가 바뀌기떄문에 저장이 정상적으로 작동하지 않고 마지막에 불러온 파일 정보 기준으로 저장된다.
    //그럼으로 loadData를 따로 나누어 관리.
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

        //유니티에서 자동으로 생성해주는 경로
        //path + fileName 으로 경로 설정을 하기 위에 path에 "/" 추가
        //C:/Users/장은서/AppData/LocalLow/DefaultCompany/My project 2 +/Save/ 
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
            Debug.Log("추가");
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
            //Debug.Log("저장된 파일이 없습니다.");
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
            Debug.Log("같은버튼");
            selectedSlot.DeSelectSlot();
            ResetSlotData();
            return;
        }

        Debug.Log("다른버튼");
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
        //그렇지 않으면 파일 삭제
        File.Delete(path + slotName);
        selectedSlot.ResetSlot();
        loadData = selectedSlot.userData;
        selectedSlot = null;
    }

    //선택한 슬롯의 저장파일을 삭제하는 함수
    public bool DeleteButtonSelect()
    {
        //슬롯 선택을 취소할 시 ""으로 저장된다. 
        //파일 선택이 되지 않았음으로 return;
        if (slotName == "")
        {
            Debug.Log("선택된 정보 없음");
            return false;
        }

        if (slotName == "AutoSave")
        {
            Debug.Log("삭제 할 수 없는 슬롯");
            return false;
        }

        //선택한 파일을 찾는다.
        //없으면 이미 삭제되었습니다 출력
        if (!File.Exists(path + slotName))
        {
            Debug.Log("이미 삭제되었습니다..");
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
