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
    public Vector3 SightOffset { get { return sightOffset; } }

    [SerializeField] private float actionRange = 15f;
    public float ActionRange { get { return actionRange; } }

    //Enemy가 Player를 탐지 할 수 있는 범위
    [SerializeField] private float detectRange = 30f;
    public float DetectRange { get { return detectRange; } } 

    //Enemy가 탐지해야 되는 target Layer
    public LayerMask targetMask;

    //Target의 정보를 갱신하기 위한 update주기
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
            //updateTargetTime 매개변수로 받아 updateTargetTime(현재 0.1초) 마다 플레이어 위치를 갱신한다.
            yield return wfs;
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
                if (searchEnemy.TryGetComponent<Player>(out Player _target))
                {
                    if (target != _target)
                    {
                        target = _target.transform;
                    }
                    //Player의 위치 확인
                    LocateTarget(_target.transform);
                }
            }
        }
        else
        {
            //Player가 감지 범위 밖으로 벗어나면 정보 제거
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

        //target 방향으로 레이를 쏴서 벽에 레이가 걸리면
        if (Physics.Raycast(transform.position + sightOffset, _dir, _dis, 1<<12))
        {
            visibelTarget = false;
            return false;
        }
        else
        {
            //target이 보이는지 안보이는지에 대한 판정은 레이캐스트를 쐈을때 레이가 벽에 닿으면 시야가 막힌걸로 판정하여 플레이어가 보이지 않게하고 있다.
            //플레이어가 레이를 맞았을때 보인다고 판정할수도 있지만 그런 경우 플레이어가 순간적으로 빠르게 움직이거나 레이를 피하는 경우
            //시야내에 장애물이 없음에도 플레이어가 보이지 않는다고 판정 할 수 있기 때문에 현재 방식을 채택.
            visibelTarget = true;
            return true;
        }
    }

    public void StopSearch()
    {
        StopAllCoroutines();
    }
}
