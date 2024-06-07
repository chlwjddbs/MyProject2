using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchPlayer : MonoBehaviour
{
    //Enemy�� Player�� ã���� ��� //���� Player�� �ƴ϶� �ٸ� Ÿ���� Enemy�� ������ �����ϱ� �߰��غ���.
    [SerializeField] private Transform target;
    public Transform Target { get { return target; } }

    //Enemy�� Player���� �Ÿ�
    [SerializeField]private float targetDis = Mathf.Infinity;
    public float TargetDis
    {
        get { return targetDis; }
    }

    //Player�� �ִ� ����
    private Vector3 targetDir;
    public Vector3 TargetDir
    {
        get { return targetDir; }
    }

    //Player�� Ž���ϱ� ���� Ray �߻� ��ġ ����
    [SerializeField]private Vector3 sightOffset;
    public Vector3 SightOffset { get { return sightOffset; } }

    [SerializeField] private float actionRange = 15f;
    public float ActionRange { get { return actionRange; } }

    //Enemy�� Player�� Ž�� �� �� �ִ� ����
    [SerializeField] private float detectRange = 30f;
    public float DetectRange { get { return detectRange; } } 

    //Enemy�� Ž���ؾ� �Ǵ� target Layer
    public LayerMask targetMask;

    //Target�� ������ �����ϱ� ���� update�ֱ�
    private float updateTargetTime = 0.1f;

    public bool visibelTarget; //{ get { return VisibleTarget(targetDir, targetDis);} }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateTarget(updateTargetTime));
    }

    IEnumerator UpdateTarget(float delay)
    {
        var wfs = new WaitForSeconds(delay);

        while (true)
        {
            //updateTargetTime �Ű������� �޾� updateTargetTime(���� 0.1��) ���� �÷��̾� ��ġ�� �����Ѵ�.
            yield return wfs;
            SearchTarget();
        }
    }

    public void SearchTarget()
    {
        //detectRange ������ Collider
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectRange, targetMask);

        if (Target == null)
        {
            foreach (var searchEnemy in enemies)
            {
                //���� ���� Collider�߿� Player�� �ִٸ�
                if (searchEnemy.TryGetComponent<Player>(out Player _target))
                {
                    if (target != _target)
                    {
                        target = _target.transform;
                    }
                    //Player�� ��ġ Ȯ��
                    LocateTarget(_target.transform);
                }
            }
        }
        else
        {
            //Player�� ���� ���� ������ ����� ���� ����
            if (TargetDis > detectRange)
            {
                target = null;
                targetDis = Mathf.Infinity;
                targetDir = Vector3.zero;
                //visibelTarget = false;
                return;
            }
            LocateTarget(Target);           
        }
    }


    public void LocateTarget(Transform _target)
    {
        targetDir = (_target.transform.position - transform.position).normalized;
        targetDis = (_target.transform.position - transform.position).magnitude;

        VisibleTarget(TargetDir, TargetDis);
    }

    public bool VisibleTarget(Vector3 _dir, float _dis)
    {
        Debug.DrawRay(transform.position + sightOffset, _dir * _dis, Color.black);

        //target �������� ���̸� ���� ���� ���̰� �ɸ���
        if (Physics.Raycast(transform.position + sightOffset, _dir, _dis, 1<<12))
        {
            visibelTarget = false;
            return false;
        }
        else
        {
            //target�� ���̴��� �Ⱥ��̴����� ���� ������ ����ĳ��Ʈ�� ������ ���̰� ���� ������ �þ߰� �����ɷ� �����Ͽ� �÷��̾ ������ �ʰ��ϰ� �ִ�.
            //�÷��̾ ���̸� �¾����� ���δٰ� �����Ҽ��� ������ �׷� ��� �÷��̾ ���������� ������ �����̰ų� ���̸� ���ϴ� ���
            //�þ߳��� ��ֹ��� �������� �÷��̾ ������ �ʴ´ٰ� ���� �� �� �ֱ� ������ ���� ����� ä��.
            visibelTarget = true;
            return true;
        }
    }

    public void StopSearch()
    {
        StopAllCoroutines();
    }
}
