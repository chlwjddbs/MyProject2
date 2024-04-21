using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

//Jason을 통해 게임 정보를 관리하기 위한 Data Class

//저장
// 1. 저장 데이터를 받음
// 2. 데이터를 Json으로 변경
// 3. Json을 외부에 저장

//불러오기
// 1. 외부에서 Json을 받음
// 2. Json을 데이터로 변경
// 3. 데이터 적용

//저장할 데이터를 받을 클래스 생성


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
            Debug.Log("폴더 없음");
            Directory.CreateDirectory(path);
            return;
        }
        if (direcInfo.GetFiles().Length == 0)
        {
            Debug.Log("저장된 파일이 없습니다.");
        }
        else
        {
            Debug.Log("파일 불러오기");
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
    #region 게임 스테이지 데이터
    //게임이 저장 된 시간
    public string saveTime;

    //유저가 게임 시작시 만든 이름
    public string userName = new string("chy");

    //현재 유저가 있는 층
    public string currentStage;

    //현재 유저가 있는 지역이름
    public string currentArea;

    //현재 유저가 있는 씬의 이름
    public string sceneName;
    #endregion

    #region 플레이어 데이터
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

    #region 플레이어 장비 데이터
    public Item[] inventoryItem;
    //인벤토리 기본 크기 25
    public int invenSize = 25;
    //캐릭터 생성시 사용중인 인벤토리 0
    public int spareSlot= 0;

    public List<int> slotNum = new List<int>();
    public List<Inventory.InvenItem> invenItem = new List<Inventory.InvenItem>();
  
    public EquipItem[] equipmentItem;
    #endregion

    #region 플레이어 스킬 데이터
    public List<SkillItem> learedSkill = new List<SkillItem>();
    public List<int> skillButtonNum = new List<int>();
    public List<SkillBook.SetEquipSkill> skillButtonInfo = new List<SkillBook.SetEquipSkill>();
    #endregion;

    #region 텔레포트 게이트
    public List<int> gateNum;
    public List<GateManager.GateInfo> gateInfo;
    #endregion

    #region 적유닛 관리
    public List<Enemy.EnemyData> eDatas_1F = new List<Enemy.EnemyData>();
    public List<Enemy.EnemyData> eDatas_1F_skeletons = new List<Enemy.EnemyData>();
    public List<Enemy.EnemyData> eDatas_1F_olaf = new List<Enemy.EnemyData>();
    public List<Enemy.EnemyData> eDatas_1F_gubne = new List<Enemy.EnemyData>();
    public List<Enemy.EnemyData> eDatas_1F_spawn = new List<Enemy.EnemyData>();
    #endregion

    #region 필드 아이템 관리
    public List<bool> startFieldItem_1F = new List<bool>();
    public List<DropItemManager.AddDropItem> FiledItme_1F = new List<DropItemManager.AddDropItem>();
    #endregion

    #region 필드 퍼즐 관리
    public List<bool> puzzleKey;
    public List<bool> openGate;
    #endregion
}

public class BindKeyData
{
    public List<KeyOption> keyOtion = new List<KeyOption>();
    public List<KeyCode> bindCode = new List<KeyCode>();
}