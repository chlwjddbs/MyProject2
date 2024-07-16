using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;

public class SkeletonLich : Enemy_FSM
{
    private ObjectPoolingManager poolingManager;
    public IObjectPool<GameObject> connectPool;

    [Header("Lich Ball")]
    public GameObject lichballprefab;
    public Transform lichballPoint;
    public int useLichballCount = 1;

    [Header("Spawn Skeleton")]
    public GameObject spawnskeleonsPrefab;
    public GameObject spawnskeleonsAuraPrefab;
    public Transform spawnPoint;

    public float spawnCooltime = 100f;
    private float remainCooltime = 0f;
    private bool spawnable = true;
    [SerializeReference] protected string spawnSound;

    protected override void Start()
    {
        SetData();
    }

    protected override void Update()
    {
        base.Update();
        SpawnSkelton();
    }

    public override void SetData()
    {
        base.SetData();
    }

    public override void SetPool()
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

    public GameObject CreatePool()
    {
        GameObject lichball = Instantiate(lichballprefab, lichballPoint);
        //lichball.GetComponent<LichBall>().SetPooling(findPool);
        return lichball;
    }

    public override void Attack()
    {
        if (connectPool.Get().TryGetComponent<IProjectile>(out IProjectile value))
        {
            value.SetTarget(Target, lichballPoint.position, attackDamage);
        }
    }

    public void SpawnSkelton()
    {
        if (remainCooltime > 0)
        {
            remainCooltime -= Time.deltaTime;          
        }
        else
        {
            spawnable = true;
        }

        if(spawnable)
        {
            if (TargetDis < attackRange)
            {
                spawnable = false;
                PlayESound("spawnSkeleton");
                SpawnEnemyManager spawnManager = Instantiate(spawnskeleonsPrefab, spawnPoint.position, Quaternion.identity).GetComponent<SpawnEnemyManager>();
                spawnManager.SetData();
                GameObject spawnskeleonsAura = Instantiate(spawnskeleonsAuraPrefab, spawnPoint.position, Quaternion.identity);
                remainCooltime = spawnCooltime;
                Destroy(spawnskeleonsAura, 2f);
            }
        }
    }
}
