using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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

    public List<IObjectPool<GameObject>> poolList = new List<IObjectPool<GameObject>>();
    public Dictionary<string, FoolObjectInfo> poolObjDic = new Dictionary<string, FoolObjectInfo>();

    //현재 QuestUI에서만 간단하게 사용할 풀링오브젝트로 저장은 따로 구현하지 않고 이용하여 딕셔너리는 필요없다.
    public IObjectPool<Image> imagePool;


    public void RegisetPoolObj(GameObject _poolObj, IObjectPool<GameObject> _poolObjs, int _count = 2)
    {
        if (!poolObjDic.TryGetValue(_poolObj.name, out FoolObjectInfo value))
        {
            poolList.Add(_poolObjs);
            poolObjDic.Add(_poolObj.name, new FoolObjectInfo(_poolObj.name, _poolObjs, _count));
        }
    }

    public void RegisetPoolImageObj(Image _poolObj, IObjectPool<Image> _poolObjs, int _count = 2)
    {
        imagePool = _poolObjs;
    }

    public IObjectPool<GameObject> FindPool(string _foolName)
    {
        if (poolObjDic.TryGetValue(_foolName, out FoolObjectInfo value))
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
