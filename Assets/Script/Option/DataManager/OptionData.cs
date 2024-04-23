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

        //����Ƽ���� �ڵ����� �������ִ� ���
        //path + fileName ���� ��� ������ �ϱ� ���� path�� "/" �߰�
        //C:/Users/������/AppData/LocalLow/DefaultCompany/My project 2 +/Save/ 
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
        //����� ������ ������ �ʱⰪ�̶�� �������� false�� ��ȯ�� �ʱⰪ���� �����Ѵ�.
        if (!File.Exists(path + fileName))
        {
            Debug.Log("�ʱⰪ");
            return false;
        }
        else
        {
            //����� ������ ������ �����͸� �ҷ�ȭ ����� ������ �������ش�.
            string data = File.ReadAllText(path + fileName);
            bindKeyData = JsonUtility.FromJson<BindKeyData>(data);
            Debug.Log("�� �ε�");
            return true;
        }
    }

    public override void DeleteData()
    {
        File.Delete(path + fileName);
    }
}
