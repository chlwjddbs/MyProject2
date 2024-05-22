using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolingManager : MonoBehaviour
{
    public class FoolObjectInfo
    {
        public string objectName;
        public IObjectPool<GameObject> objectPool;
        public int objectCount;

        public FoolObjectInfo(string _objectName, IObjectPool<GameObject> _poolObj, int _objectCount)
        {
            objectName = _objectName;
            objectPool = _poolObj;
            objectCount = _objectCount;
        }
    }

    public static ObjectPoolingManager instance;

    public PlayerStatus test;

    private void Awake()
    {
        if(instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public List<IObjectPool<GameObject>> foolList = new List<IObjectPool<GameObject>>();
    public Dictionary<string, FoolObjectInfo> foolObjDic = new Dictionary<string, FoolObjectInfo>();


    public void RegisetPoolObj(GameObject _foolObj, IObjectPool<GameObject> _foolObjs, int _count = 2)
    {
        if (!foolObjDic.TryGetValue(_foolObj.name, out FoolObjectInfo value))
        {
            foolList.Add(_foolObjs);
            foolObjDic.Add(_foolObj.name, new FoolObjectInfo(_foolObj.name, _foolObjs, _count));
        }
    }

    public IObjectPool<GameObject> FindPool(string _foolName)
    {
        if (foolObjDic.TryGetValue(_foolName, out FoolObjectInfo value))
        {
            return value.objectPool;
        }

        return null;
    }

    public void OnGet(GameObject _poolObj)
    {
        _poolObj.SetActive(true);
    }

    public void OnRelease(GameObject _poolObj)
    {
        _poolObj.SetActive(false);
    }

    public void OnDes(GameObject _poolObj)
    {
        Destroy(_poolObj);
    }
}
