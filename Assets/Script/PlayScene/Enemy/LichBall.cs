using UnityEngine;
using UnityEngine.Pool;

public class LichBall : MonoBehaviour, IProjectile
{
    //���� ���� ��ġ
    private Vector3 startPos;
    //��ǥ ��� ��ġ
    private Vector3 targetPos;
    //Slerp�� ���� center��
    private Vector3 center;
    //������ ��������� �ɸ��� �ð�(LichBall�� ���ư��µ� �ɸ��� �ð�)
    public float endAttackTime =1f;
    //��ġ���� ������ �ð�
    private float startTime;

    private bool isTarget = false;

    public float parabolaHeight;

    private Transform target;

    private float attackDamage;

    public IObjectPool<GameObject> objPool;

    /*
    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.FindWithTag("Player");
    }
    */

    // Update is called once per frame
    void Update()
    {       
        if(isTarget == false)
        {
            return;
        }
        targetPos = target.position;
        center = (startPos + targetPos) * 0.5f;
        center -= new Vector3(0, 1f * parabolaHeight, 0);
        Vector3 startCenter = startPos - center;
        Vector3 targetCenter = targetPos - center;
        float complete = (Time.time - startTime) / endAttackTime;       
        transform.position = Vector3.Slerp(startCenter, targetCenter, complete);
        transform.position += center;

        /*
        if(complete > 1f)
        {
            //Destroy(gameObject);
        }
        */
    }

    public void GetTargetPos(Vector3 _targetPos, Transform _target, float _attackDamage)
    {
        target = _target;
        targetPos = _targetPos;      
        startTime = Time.time;
        startPos = transform.position;
        isTarget = true;
        attackDamage = _attackDamage;
        
    }

    public void SetTarget(Transform _target, Vector3 _startPos, float _attackDmg)
    {
        target = _target;
        transform.position = _startPos;
        startPos = _startPos;
        startTime = Time.time;
        attackDamage = _attackDmg;
        isTarget = true;
    }

    public void SetPooling(IObjectPool<GameObject> _pooling)
    {
        objPool = _pooling;
    }

    private void OnTriggerEnter(Collider other)
    {     
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStatus>().TakeDamage(attackDamage);
            //objPool.Release(gameObject);
            ObjectPoolingManager.instance.FindPool("LichBall").Release(gameObject);
        }
    }
}
