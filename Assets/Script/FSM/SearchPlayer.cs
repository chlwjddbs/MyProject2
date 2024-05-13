using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchPlayer : MonoBehaviour
{
    //Enemy가 Player를 찾으면 등록 //추후 Player뿐 아니라 다른 타입의 Enemy를 적으로 규정하기 추가해보기.
    [SerializeField] private Transform target;
    public Transform Target { get { return target; } }

    //Enemy와 Player와의 거리
    [SerializeField]private float targetDis = Mathf.Infinity;
    public float TargetDis
    {
        get { return targetDis; }
    }

    //Player가 있는 방향
    private Vector3 targetDir;
    public Vector3 TargetDir
    {
        get { return targetDir; }
    }

    //Player를 탐지하기 위한 Ray 발사 위치 보정
    [SerializeField]private Vector3 sightOffset;

    //Enemy가 Player를 탐지 할 수 있는 범위
    [SerializeField] private float detectRange = 15f;
    public float DetectRange { get { return detectRange; } } 

    //Enemy가 탐지해야 되는 target Layer
    public LayerMask targetMask;

    //Target의 정보를 갱신하기 위한 update주기
    private float updateTargetTime = 0.1f;

    public bool VisibelTarget { get { return VisibleTarget(targetDir, targetDis); } }

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(UpdateTarget(updateTargetTime));
    }

    IEnumerator UpdateTarget(float delay)
    {
        //플레이어가 죽지 않았을때 계속해서 플레이어를 찾는다.
        while (!PlayerStatus.isDeath)
        {
            //updateTargetTime 매개변수로 받아 updateTargetTime(현재 0.1초) 마다 플레이어 위치를 갱신한다.
            yield return new WaitForSeconds(delay);
            SearchTarget();
        }
    }

    public void SearchTarget()
    {
        //detectRange 범위의 Collider
        Collider[] enemies = Physics.OverlapSphere(transform.position, detectRange, targetMask);

        if (Target == null)
        {
            foreach (var searchEnemy in enemies)
            {
                //범위 내의 Collider중에 Player가 있다면
                if (searchEnemy.TryGetComponent<PlayerStatus>(out PlayerStatus _target))
                {
                    //Player의 위치 확인
                    LocateTarget(_target.transform);

                    //Player가 보이는지 확인 후 추가해준다.
                    if (VisibleTarget(TargetDir, TargetDis))
                    {
                        target = _target.transform;
                    }
                }
            }
        }
        else
        {
            //Player가 감지 범위 밖으로 벗어나거나 시야에 가려져 보이지 않는다면 정보 제거
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
 
        //플레이어 방향으로 레이를 쏴서 벽에 닿으면
        if (Physics.Raycast(transform.position + sightOffset, _dir, _dis, 1 << 12))
        {
            //시야가 벽에 막혔기 때문에 플레이어가 안보인다고 판정한다.
            return false;
        }
        else
        {
            //시야가 벽에 막히기 않았기 때문에 플레이어가 보인다고 판정한다.
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
