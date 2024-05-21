using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class ObjectPoolingManager : MonoBehaviour
{
    public class ObjectInfo
    {
        public string objectName;
        public GameObject objectPrefab;
        public int objectCount;
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

    public IObjectPool<GameObject> lichBall;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetPoolingObj(IObjectPool<GameObject> _a)
    {

    }

    public void OnGetObject(GameObject _poolObj)
    {
        _poolObj.SetActive(true);
    }

    public void OnReleaseObject(GameObject _poolObj)
    {
        _poolObj.SetActive(false);
    }

    public void OnDestroyObject(GameObject _poolObj)
    {
        Destroy(_poolObj);
    }
}
