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
    protected string path;
    [SerializeField] protected string dirName;
    protected string autoSave = "AutoSave";
    //private string fileName;
    public DirectoryInfo direcInfo;

    // Start is called before the first frame update
    protected virtual void Start()
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

    
    public virtual void CreateSaveData()
    {
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
    }
    

    public virtual void DeleteData()
    {
        
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

public class BindKeyData
{
    public List<KeyOption> keyOtion = new List<KeyOption>();
    public List<KeyCode> bindCode = new List<KeyCode>();
}