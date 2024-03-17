using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

//Jason�� ���� ���� ������ �����ϱ� ���� Data Class

//����
// 1. ���� �����͸� ����
// 2. �����͸� Json���� ����
// 3. Json�� �ܺο� ����

//�ҷ�����
// 1. �ܺο��� Json�� ����
// 2. Json�� �����ͷ� ����
// 3. ������ ����

//������ �����͸� ���� Ŭ���� ����


public class DataManager : MonoBehaviour
{
    public static DataManager instance;

    //���� ������
    public UserData userData = new UserData();

    //���Կ� Json������ ���� �� ����� loadData
    //loadData�� ������� �ʰ� ���� userData�� �Բ� ����ϸ�
    //���� �ٲ� �Ѿ���� saveFileManager�� ����Ǹ鼭 ���ϵ��� �ҷ����鼭
    //userData �����Ͱ� �ٲ�⋚���� ������ ���������� �۵����� �ʰ� �������� �ҷ��� ���� ���� �������� ����ȴ�.
    //�׷����� loadData�� ���� ������ ����.
    public UserData loadData = new UserData();

    private string path;
    private string autoSave = "AutoSave";
    //private string fileName;
    public DirectoryInfo direcInfo;

    //public int currentSlot;
    public string slotName = "";
    public SaveSlot selectedSlot;

    public Fader fader;

    public bool newGame = true;

    public bool isSet = false;

    private SaveSlot autoSaveSlot;

    //public Dictionary<int> slotItem;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        //����Ƽ���� �ڵ����� �������ִ� ���
        //path + fileName ���� ��� ������ �ϱ� ���� path�� "/" �߰�
        //C:/Users/������/AppData/LocalLow/DefaultCompany/My project 2 +/Save/ 
        path = Application.persistentDataPath + "/Save/";

        direcInfo = new DirectoryInfo(path);
    }
    // Start is called before the first frame update

    private void Start()
    {
        if (!Directory.Exists(path))
        {
            Debug.Log("���� ����");
            Directory.CreateDirectory(path);
            return;
        }
        if (direcInfo.GetFiles().Length == 0)
        {
            Debug.Log("����� ������ �����ϴ�.");
        }
        else
        {
            Debug.Log("���� �ҷ�����");
        }     
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
        Debug.Log(path);
    }

    public void CreateSaveData()
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string data = JsonUtility.ToJson(userData);
        File.WriteAllText(path + slotName, data);
        selectedSlot.SetSlot();
    }
  
    public void LoadData(string _slotName,SaveSlot _saveSlot)
    {
        if(!File.Exists(path + _slotName))
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
        if(slotName == _slotName)
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

    public void DeleteData()
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
        if(slotName == "")
        {
            Debug.Log("���õ� ���� ����");
            return false;
        }

        if(slotName == "AutoSave")
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

    public bool SaveButtonEmpty()
    {
        if(loadData.userName == null)
        {
            return true;
        }
        else
        {
            return false;
        }
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
}

public class UserData
{
    #region ���� �������� ������
    //������ ���� �� �ð�
    public string saveTime;

    //������ ���� ���۽� ���� �̸�
    public string userName = new string("chy");

    //���� ������ �ִ� ��
    public string currentStage;

    //���� ������ �ִ� �����̸�
    public string currentArea;

    //���� ������ �ִ� ���� �̸�
    public string sceneName;
    #endregion

    #region �÷��̾� ������
    public Vector3 playerPos = new Vector3(52.5f, 0, 175f);

    public int level;
    public float currentExp;
    public float nextLvExp;

    public float remainHealth;
    public float health;
    public float remainMana;
    public float mana;

    public float moveSpeed;
    public float basicDamage;
    public float basicDefence;
    #endregion

    #region �÷��̾� ��� ������
    public Item[] inventoryItem;
    //�κ��丮 �⺻ ũ�� 25
    public int invenSize = 25;
    //ĳ���� ������ ������� �κ��丮 0
    public int spareSlot= 0;

    public List<int> slotNum = new List<int>();
    public List<Inventory.InvenItem> invenItem = new List<Inventory.InvenItem>();
  
    public EquipItem[] equipmentItem;
    #endregion

    #region �÷��̾� ��ų ������
    public List<SkillItem> learedSkill = new List<SkillItem>();
    public List<int> skillButtonNum = new List<int>();
    public List<SkillBook.SetEquipSkill> skillButtonInfo = new List<SkillBook.SetEquipSkill>();
    #endregion;

    #region �ڷ���Ʈ ����Ʈ
    public List<int> gateNum;
    public List<GateManager.GateInfo> gateInfo;
    #endregion

    #region ������ ����
    public List<Enemy.EnemyData> eDatas_1F = new List<Enemy.EnemyData>();
    public List<Enemy.EnemyData> eDatas_1F_skeletons = new List<Enemy.EnemyData>();
    public List<Enemy.EnemyData> eDatas_1F_olaf = new List<Enemy.EnemyData>();
    public List<Enemy.EnemyData> eDatas_1F_gubne = new List<Enemy.EnemyData>();
    public List<Enemy.EnemyData> eDatas_1F_spawn = new List<Enemy.EnemyData>();
    #endregion

    #region �ʵ� ������ ����
    public List<bool> startFieldItem_1F = new List<bool>();
    public List<DropItemManager.AddDropItem> FiledItme_1F = new List<DropItemManager.AddDropItem>();
    #endregion

    #region �ʵ� ���� ����
    public List<bool> puzzleKey;
    public List<bool> openGate;
    #endregion
}