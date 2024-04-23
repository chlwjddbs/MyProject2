using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using TMPro;
using UnityEngine.SceneManagement;

public class OptionData : DataManager
{
    public static OptionData instance;

    public BindKeyData bindKeyData = new BindKeyData();

    [SerializeField] private string fileName = "BindKeyData";

    private void Awake()
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

    protected override void Start()
    {
        base.Start();
    }

    public override void CreateSaveData()
    {
        base.CreateSaveData();

        string data = JsonUtility.ToJson(bindKeyData);
        File.WriteAllText(path + fileName, data);
    }

    public void SetData(Dictionary<KeyOption, KeyOptionInfo> _bindData)
    {
        bindKeyData.keyOtion.Clear();
        bindKeyData.bindCode.Clear();

        foreach (var saveData in _bindData)
        {
            bindKeyData.keyOtion.Add(saveData.Key);
            bindKeyData.bindCode.Add(saveData.Value.bindKey);
            //Debug.Log(saveData.Key);
            //Debug.Log(saveData.Value);
        }

        CreateSaveData();
    }

    public void ChangeData(KeyOption _keyOption, KeyOptionInfo _optionInfo)
    {
        for (int i = 0; i < bindKeyData.keyOtion.Count; i++)
        {
            if (bindKeyData.keyOtion[i] == _keyOption)
            {
                bindKeyData.bindCode[i] = _optionInfo.bindKey;
                break;
            }
        }

        //CreateSaveData();
    }

    IEnumerator C_SaveData(Dictionary<KeyOption, KeyOptionInfo> _bindData)
    {
        bindKeyData.keyOtion.Clear();
        bindKeyData.bindCode.Clear();
        foreach (var saveData in _bindData)
        {
            bindKeyData.keyOtion.Add(saveData.Key);
            bindKeyData.bindCode.Add(saveData.Value.bindKey);
        }
        yield return new WaitForSecondsRealtime(1f);
    }

    public bool LoadData()
    {
        //저장된 정보가 없으면 초기값이라는 뜻임으로 false를 반환해 초기값으로 설정한다.
        if (!File.Exists(path + fileName))
        {
            Debug.Log("초기값");
            return false;
        }
        else
        {
            //저장된 정보가 있으면 데이터를 불러화 저장된 정보로 세팅해준다.
            string data = File.ReadAllText(path + fileName);
            bindKeyData = JsonUtility.FromJson<BindKeyData>(data);
            Debug.Log("값 로드");
            return true;
        }
    }

    public override void DeleteData()
    {
        File.Delete(path + fileName);
    }
}
