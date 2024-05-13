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

    //Enemy�� Player�� Ž�� �� �� �ִ� ����
    [SerializeField] private float detectRange = 15f;
    public float DetectRange { get { return detectRange; } } 

    //Enemy�� Ž���ؾ� �Ǵ� target Layer
    public LayerMask targetMask;

    //Target�� ������ �����ϱ� ���� update�ֱ�
    private float updateTargetTime = 0.1f;

    public bool VisibelTarget { get { return VisibleTarget(targetDir, targetDis); } }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateTarget(updateTargetTime));
    }

    IEnumerator UpdateTarget(float delay)
    {
        //�÷��̾ ���� �ʾ����� ����ؼ� �÷��̾ ã�´�.
        while (!PlayerStatus.isDeath)
        {
            //updateTargetTime �Ű������� �޾� updateTargetTime(���� 0.1��) ���� �÷��̾� ��ġ�� �����Ѵ�.
            yield return new WaitForSeconds(delay);
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
                if (searchEnemy.TryGetComponent<PlayerStatus>(out PlayerStatus _target))
                {
                    //Player�� ��ġ Ȯ��
                    LocateTarget(_target.transform);

                    //Player�� ���̴��� Ȯ�� �� �߰����ش�.
                    if (VisibleTarget(TargetDir, TargetDis))
                    {
                        target = _target.transform;
                    }
                }
            }
        }
        else
        {
            //Player�� ���� ���� ������ ����ų� �þ߿� ������ ������ �ʴ´ٸ� ���� ����
            if (TargetDis > detectRange | !VisibleTarget(TargetDir, TargetDis))
            {
                target = null;
                targetDis = Mathf.Infinity;
                targetDir = Vector3.zero;

                return;
            }
            LocateTarget(Target);
        }
    }

    public bool VisibleTarget(Vector3 _dir, float _dis)
    {
        //Debug.DrawRay(transform.position + offset, targetDir * targetDis, Color.black);
 
        //�÷��̾� �������� ���̸� ���� ���� ������
        if (Physics.Raycast(transform.position + sightOffset, _dir, _dis, 1 << 12))
        {
            //�þ߰� ���� ������ ������ �÷��̾ �Ⱥ��δٰ� �����Ѵ�.
            return false;
        }
        else
        {
            //�þ߰� ���� ������ �ʾұ� ������ �÷��̾ ���δٰ� �����Ѵ�.
            return true;
        }
    }

    public void LocateTarget(Transform _target)
    {
        targetDir = (_target.transform.position - transform.position).normalized;
        targetDis = (_target.transform.position - transform.position).magnitude;
    }

    public void StopSearch()
    {
        StopAllCoroutines();
    }
}
