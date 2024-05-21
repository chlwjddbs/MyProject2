using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SkeletonLich : Enemy_FSM
{
    public IObjectPool<LichBall> lichballPool;
    //public ObjectPoolingManager poolingManager;

    public GameObject lichballprefab;

    private void Awake()
    {
        //poolingManager = ObjectPoolingManager.instance;
        lichballPool = new ObjectPool<LichBall>(CreatePoolObj,OnGet,OnRelease,OnDes,maxSize:3);
    }

    public LichBall CreatePoolObj()
    {
        LichBall lichball = Instantiate(lichballprefab).GetComponent<LichBall>();
        lichball.RegisterPoolingManager(lichballPool);
        return lichball;
    }

    public void OnGet(LichBall _lichBall)
    {
        _lichBall.gameObject.SetActive(true);
    }

    public void OnRelease(LichBall _lichBall)
    {
        _lichBall.gameObject.SetActive(false);
    }

    public void OnDes(LichBall _lichBall)
    {
        Destroy(_lichBall.gameObject);
    }

    public void Attack()
    {
        LichBall lichball = lichballPool.Get();
        lichball.GetTargetPos(Target.position, Target.gameObject, attackDamage);
    }
}
