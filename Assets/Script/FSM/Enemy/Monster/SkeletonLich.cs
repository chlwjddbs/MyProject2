using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SkeletonLich : Enemy_FSM
{
    public ObjectPoolingManager poolingManager;
    public IObjectPool<GameObject> connectPool;

    public GameObject lichballprefab;
    public Transform lichballPoint;
    public int useLichballCount = 1;

    protected override void Start()
    {
        //poolingManager.test = new ObjectPool<GameObject>(CreatePoolObj, poolingManager.OnGet, poolingManager.OnRelease, poolingManager.OnDes, maxSize: 3);
        SetData();
    }

    public override void SetData()
    {
        base.SetData();
        SetPoolingObj();
    }

    public GameObject CreatePool()
    {
        GameObject lichball = Instantiate(lichballprefab,lichballPoint);
        //lichball.GetComponent<LichBall>().SetPooling(findPool);
        return lichball;
    }

    public void SetPoolingObj()
    {
        poolingManager = ObjectPoolingManager.instance;
        poolingManager.RegisetPoolObj(lichballprefab, new ObjectPool<GameObject>(CreatePool, poolingManager.OnGet, poolingManager.OnRelease, poolingManager.OnDes, maxSize: 3));
        connectPool = poolingManager.FindPool(lichballprefab.name);

        for (int i = 0; i < useLichballCount; i++)
        {
            GameObject lichballs = CreatePool();
            //lichballs.GetComponent<LichBall>().SetPooling(findPool);
            connectPool.Release(lichballs);
        }
    }

    public void Attacks()
    {
        if (connectPool.Get().TryGetComponent<IProjectile>(out IProjectile value))
        {
            value.SetTarget(Target, lichballPoint.position, attackDamage);
        }
    }
}
