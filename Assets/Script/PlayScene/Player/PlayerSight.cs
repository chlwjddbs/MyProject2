using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSight : MonoBehaviour
{
    //�þ� �Ÿ�
    public float sightRaduis = float.MaxValue;

    //�þ߳� ��ü�� �����ϴ� ����
    public float sightRange = 50f;

    //�þ� ����
    [Range(0, 360)]
    public float sightAngle;

    //�þ߿� ��� ��ֹ�
    public LayerMask targetMask, obstacleMask, checkFadeMask, wallsMask, objectMask;

    //�þ߿� ���� ������Ʈ ����Ʈ
    public List<Transform> viewTarget = new List<Transform>();

    public float maskCutawayDis = 0.1f;

    public float meshResolution;

    Mesh viewMesh;
    public MeshFilter viewMeshFilter;

    public int edgeResolveIterations;
    public float edgeDstThreshold;

    public Vector3 sigthOffset;
    public Vector3 drawOffset;

    public List<Transform> visibleWalls = new List<Transform>();

    private void Start()
    {
        viewMesh = new Mesh();
        viewMesh.name = "View Mesh";
        viewMeshFilter.mesh = viewMesh;
    }

    // Update is called once per frame
    void Update()
    {
        viewTargetAdd();      //���� �� �� ����ϱ�
        viewTargetRemove();   //���� �� �� �����ϱ�
        //VisibleWall();        //�� ���� : �þ߿� ���� ���� ����
        //VisibleWall_2();
        VisibleWall_3();
        //VisibleUnderWall(); //UnderWall ����
        VisibleParticle();
        //SightWall();        //�� ���� : �þ߸� ������ �� ������ ó���ϱ�
        //SightWall_2();
        HideWall();
        VisibleObject();
    }

    private void LateUpdate()
    {
        visibleWalls.Clear();
        DrawSight(); //�þ� �׸���
    }

    //�� Ž���ϱ�
    private void viewTargetAdd()
    {
        //�÷��̾�������� sightRaduisũ���� OverlapSphere�� �׷� �� �ȿ� ���� targetMask(Enemy)�� targetView�� ã�ƿ´�.
        Collider[] targetView = Physics.OverlapSphere(transform.position, sightRaduis, targetMask);

        //targetView = ������ Ÿ���� �迭
        for (int i = 0; i < targetView.Length; i++)
        {

            //������ Ÿ�� ���ϱ�
            Transform target = targetView[i].transform;

            //������ Ÿ���� ���� ���ϱ�
            Vector3 targetDir = (target.position - (transform.position + sigthOffset)).normalized;

            // �÷��̾�� forward�� target�� �̷�� ���� ������ �����̰� ���� ���� �̳��� �ִٸ�
            if (Vector3.Angle(transform.forward, targetDir) < sightAngle / 2)
            {
                //�÷��̾�� Ÿ���� �Ÿ��� ����.
                float targetdDis = Vector3.Distance(transform.position + sigthOffset, target.transform.position);

                //�÷��̾� ��ġ���� Ÿ�� �Ÿ� ��ŭ ���̸� ���.
                //�� �� ���̿� ��ֹ��� ���� �ʾҰ�
                //if (!Physics.Raycast(transform.position, targetDir, targetdDis, obstacleMask))
                Debug.DrawRay(transform.position + sigthOffset, targetDir * targetdDis,Color.blue);

                //Ÿ���� �þ߰Ÿ� ������ ������ Ÿ���� �������� ���ش�.
                if (targetdDis > sightRange)
                {
                    if (target.GetComponent<Enemy>().RenderBox.activeSelf == true)
                    {
                        target.GetComponent<Enemy>().RenderBox.SetActive(false);
                    }
                    //�������� �ִ� ��Ͽ� �ִ� ���̸� ����
                    if (viewTarget.Contains(target))
                    {
                        viewTarget.Remove(target);
                    }
                }
                //Ÿ���� �þ߰Ÿ� ���� ���� ��
                else
                {
                    //Ÿ������ ���̸� �߻��� ���̰� ���� ���� �ʴ´ٸ�
                    if (!Physics.Raycast(transform.position + sigthOffset, targetDir, targetdDis, wallsMask))
                    {
                        //����Ʈ�� �߰����� ���� Ÿ���̸� ����Ʈ�� �߰��ϰ� �������� ���ش�.
                        if (!viewTarget.Contains(target))
                        {
                            target.GetComponent<Enemy>().RenderBox.SetActive(true);
                            viewTarget.Add(target);
                        }
                    }
                    //���̰� ��ֹ��� ��Ұ�
                    //else if (Physics.Raycast(transform.position, targetDir, targetdDis, obstacleMask))
                    else if (Physics.Raycast(transform.position + sigthOffset, targetDir, targetdDis, wallsMask))
                    {
                        //����Ʈ�� Ÿ���� �ִٸ� Ÿ�ٿ��� �����ϰ� �������� ���ش�.
                        if (viewTarget.Contains(target))
                        {
                            target.GetComponent<Enemy>().RenderBox.SetActive(false);
                            viewTarget.Remove(target);
                        }
                    }
                }
            }
            /*
            //������������ �þ߰��� ������ ���� �� Ÿ�ٿ��� ����
            else
            {
                target.GetComponent<Enemy>().RenderBox.SetActive(false);
                viewTarget.Remove(target);
            }
            */
        }
    }

    //Ž�� ���� �� �̳ʹ� ����
    private void viewTargetRemove()
    {
        //���� �������� �̳ʹ̸� �����ϴ� ����Ʈ ����
        List<Transform> target = new List<Transform>();

        //�������� �̳ʹ�
        Collider[] targetView = Physics.OverlapSphere(transform.position, sightRaduis, targetMask);

        for (int i = 0; i < targetView.Length; i++)
        {

            target.Add(targetView[i].transform);

        }

        //���� �������� ����Ʈ�� �̹� �����Ǿ� ��ϵ� �̳ʹ̸� ��.
        for (int j = 0; j < viewTarget.Count; j++)
        {
            //�̳ʹ̰� �������� ������ ���� ���������� �������� �̳ʹ� ����Ʈ���� ������ �ȴ�.
            //������ �̹� ������� �̳ʹ� ����Ʈ�� �״���̱� ������
            //�̹� ��ϵ� �̳ʹ̿� ���� �������� �̳ʹ̸� ���Ͽ�,
            //�������� �̳ʹ� ����Ʈ�� ��ϵ� �̳ʹ̰� ������� �����Ѵ�.
            if (!target.Contains(viewTarget[j]))
            {
                viewTarget.Remove(viewTarget[j]);
            }
        }
        /*
            for (int i = 0; i < viewTarget.Count; i++)
        {

            if ((transform.position - viewTarget[i].position).magnitude > sightRaduis)
            {
                viewTarget.Remove(viewTarget[i]);
            }
        }
        */
    }

    //�þ߿� ���� ���� ����
    /*
    public void VisibleWall()
    {
        //�÷��̾�������� sightRaduisũ���� OverlapSphere�� �׷� �� �ȿ� ���� ������ ã�´�.
        Collider[] targetWall = Physics.OverlapSphere(transform.position, sightRaduis, obstacleMask);

        for (int i = 0; i < targetWall.Length; i++)
        {
            //������ �� ���ϱ�
            Transform wall = targetWall[i].transform;

            //������ ���� ���� ���ϱ�
            Vector3 wallDir = (wall.position - (transform.position + sigthOffset)).normalized;

            //�÷��̾� �þ� �ޱ۾ȿ� ���� �ִٸ�
            if (Vector3.Angle(transform.forward, wallDir) < sightAngle / 2)
            {
                //�÷��̾�� ���� �Ÿ�
                float wallDis = Vector3.Distance(transform.position + sigthOffset, wall.transform.position);

                //�÷��̾�� �� �������� ������ �Ÿ� ��ŭ ����ĳ��Ʈ�� �� ���� �޾ƿ´�.
                //RaycastHit[] hit = Physics.RaycastAll(transform.position, wallDir, wallDis, obstacleMask);
                RaycastHit[] hit = Physics.RaycastAll(transform.position + sigthOffset, wallDir, wallDis, wallsMask);

                if (wall.name == "1")
                {
                    Debug.DrawRay(transform.position + sigthOffset, wallDir * wallDis, Color.blue, 1f);
                    Debug.Log(hit.Length);
                }

                //�浹�� ���� 2�� �̻��϶�
                if (hit.Length > 1f)
                {
                    //�浹�� ���� 2�� �̻������� �þ߿� ���� ���̶�� �����ش�.
                    if (visibleWalls.Contains(wall))
                    {
                        if (wall.GetComponent<MeshRenderer>().enabled == false)
                        {
                            wall.GetComponent<MeshRenderer>().enabled = true;
                            if (wall.GetChild(0).name == "UnderWall")
                            {
                                wall.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                            }
                        }
                    }
                    else
                    {
                        if (wall.GetComponent<MeshRenderer>().enabled == true)
                        {
                            //�þ߿� ������ ���� ���� �Ž÷������� ���ش�. (�Ⱥ��̰� ��)
                            wall.GetComponent<MeshRenderer>().enabled = false;
                            if (wall.GetChild(0).name == "UnderWall")
                            {
                                wall.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
                            }
                        }
                    }
                }
                else
                {
                    if (wall.GetComponent<MeshRenderer>().enabled == false)
                    {
                        wall.GetComponent<MeshRenderer>().enabled = true;
                        if (wall.GetChild(0).name == "UnderWall")
                        {
                            wall.GetChild(0).GetComponent<MeshRenderer>().enabled = true;
                        }
                    }
                }

            }
        }
    }

    public void VisibleWall_2()
    {
        //�÷��̾�������� sightRaduisũ���� OverlapSphere�� �׷� �� �ȿ� ���� ������ ã�´�.
        Collider[] targetWall = Physics.OverlapSphere(transform.position, sightRaduis, 1 << LayerMask.NameToLayer("Wall_2"));

        for (int i = 0; i < targetWall.Length; i++)
        {
            //������ �� ���ϱ�
            Transform wall = targetWall[i].transform;

            //������ ���� ���� ���ϱ�
            Vector3 wallDir = (wall.position - (transform.position + sigthOffset)).normalized;

            //�÷��̾� �þ� �ޱ۾ȿ� ���� �ִٸ�
            if (Vector3.Angle(transform.forward, wallDir) < sightAngle / 2)
            {
                //�÷��̾�� ���� �Ÿ�
                float wallDis = Vector3.Distance(transform.position + sigthOffset, wall.transform.position);

                //�÷��̾�� �� �������� ������ �Ÿ� ��ŭ ����ĳ��Ʈ�� �� ���� �޾ƿ´�.
                RaycastHit[] hit = Physics.RaycastAll(transform.position + sigthOffset, wallDir, wallDis, wallsMask);

                if (wall.name == "1")
                {
                    Debug.DrawRay(transform.position + sigthOffset, wallDir * wallDis, Color.blue, 1f);
                    Debug.Log(hit.Length);
                }

                //�浹�� ���� 2�� �̻��϶�
                if (hit.Length > 1f)
                {
                    //�浹�� ���� 2�� �̻������� �þ߿� ���� ���̶�� �����ش�.
                    if (visibleWalls.Contains(wall))
                    {
                        if (wall.GetComponent<DrawWall_2>().isDraw == false)
                        {
                            wall.GetComponent<DrawWall_2>().isDraw = true;
                        }

                    }
                    else
                    {
                        if (wall.GetComponent<DrawWall_2>().isDraw == true)
                        {
                            //�þ߿� ������ ���� ���� �Ž÷������� ���ش�. (�Ⱥ��̰� ��)
                            wall.GetComponent<DrawWall_2>().isDraw = false;
                        }

                    }
                }
                else
                {
                    if (wall.GetComponent<DrawWall_2>().isDraw == false)
                    {
                        wall.GetComponent<DrawWall_2>().isDraw = true;

                    }


                }

            }
        }
    }
    */
    public void VisibleWall_3()
    {
        //�÷��̾�������� sightRaduisũ���� OverlapSphere�� �׷� �� �ȿ� ���� ������ ã�´�.
        Collider[] targetWall = Physics.OverlapSphere(transform.position, sightRaduis, 1 << LayerMask.NameToLayer("Wall"));

        for (int i = 0; i < targetWall.Length; i++)
        {
            //������ �� ���ϱ�
            Transform wall = targetWall[i].transform;

            //������ ���� ���� ���ϱ�
            Vector3 wallDir = (wall.position - (transform.position + sigthOffset)).normalized;

            //�÷��̾� �þ� �ޱ۾ȿ� ���� �ִٸ�
            if (Vector3.Angle(transform.forward, wallDir) < sightAngle / 2)
            {
                //�÷��̾�� ���� �Ÿ�
                float wallDis = Vector3.Distance(transform.position + sigthOffset, wall.transform.position);

                //������ �Ÿ��� �þ߹������� �־����� �þ߿� ������ �ʾҴٰ� �����Ѵ�.
                if (wallDis > sightRange)
                {
                    if (wall.GetComponent<DrawWalls>().isDraw == true)
                    {
                        wall.GetComponent<DrawWalls>().isDraw = false;
                    }
                }
                else
                {
                    //�÷��̾�� �� �������� ������ �Ÿ� ��ŭ ����ĳ��Ʈ�� �� ���� �޾ƿ´�.
                    RaycastHit[] hit = Physics.RaycastAll(transform.position + sigthOffset, wallDir, wallDis, wallsMask);

                    if (wall.name == "1")
                    {
                        Debug.DrawRay(transform.position + sigthOffset, wallDir * wallDis, Color.blue, 1f);
                        Debug.Log(hit.Length);
                        Debug.Log(wallDis);
                    }

                    //�浹�� ���� 2�� �̻��϶�
                    if (hit.Length > 1f)
                    {
                        //�浹�� ���� 2�� �̻������� �þ߿� ���� ���̶�� �����ش�.
                        if (visibleWalls.Contains(wall))
                        {
                            if (wall.GetComponent<DrawWalls>().isDraw == false)
                            {
                                wall.GetComponent<DrawWalls>().isDraw = true;
                            }
                        }
                        else
                        {
                            if (wall.GetComponent<DrawWalls>().isDraw == true)
                            {
                                //�þ߿� ������ ���� ���� �Ž÷������� ���ش�. (�Ⱥ��̰� ��)
                                wall.GetComponent<DrawWalls>().isDraw = false;
                            }
                        }
                    }
                    else
                    {
                        if (wall.GetComponent<DrawWalls>().isDraw == false)
                        {
                            wall.GetComponent<DrawWalls>().isDraw = true;
                        }
                    }
                }
            }
        }
    }

    public void VisibleObject()
    {
        //�÷��̾�������� sightRaduisũ���� OverlapSphere�� �׷� �� �ȿ� ���� ������Ʈ���� ã�´�.
        Collider[] objects = Physics.OverlapSphere(transform.position, sightRaduis, objectMask);

        for (int i = 0; i < objects.Length; i++)
        {
            //������ ������Ʈ ���ϱ�
            Transform _object = objects[i].transform;

            //������ ������Ʈ�� ���� ���ϱ�
            Vector3 _objectDir = (_object.position - (transform.position + sigthOffset)).normalized;

            //�÷��̾� �þ� �ޱ۾ȿ� ������Ʈ�� �ִٸ�
            if (Vector3.Angle(transform.forward, _objectDir) < sightAngle / 2)
            {
                //�÷��̾�� ������Ʈ�� �Ÿ�
                float _objectDis = Vector3.Distance(transform.position + sigthOffset, _object.transform.position);

                if (_objectDis > sightRange)
                {
                    if (_object.GetComponent<DrawOutline>() != null)
                    {
                        _object.GetComponent<DrawOutline>().isDraw = true;
                    }

                    if (_object.GetComponent<SetCursorImage>() != null)
                    {
                        _object.GetComponent<SetCursorImage>().isDraw = true;
                    }
                }
                else
                {
                    //�÷��̾�� ������Ʈ�� �������� ������Ʈ���� �Ÿ� ��ŭ ����ĳ��Ʈ�� �� ������Ʈ�� �޾ƿ´�.
                    RaycastHit[] hit = Physics.RaycastAll(transform.position + sigthOffset, _objectDir, _objectDis, wallsMask);

                    if (_object.name == "1")
                    {
                        Debug.DrawRay(transform.position + sigthOffset, _objectDir * _objectDis, Color.blue, 1f);
                        Debug.Log(hit.Length);
                    }

                    //�浹�� ���� 1�� �̻��϶�
                    if (hit.Length >= 1f)
                    {
                        if (_object.GetComponent<DrawOutline>() != null)
                        {
                            _object.GetComponent<DrawOutline>().isDraw = false;
                        }

                        if (_object.GetComponent<SetCursorImage>() != null)
                        {
                            _object.GetComponent<SetCursorImage>().isDraw = false;
                        }
                    }
                    else
                    {
                        if (_object.GetComponent<DrawOutline>() != null)
                        {
                            _object.GetComponent<DrawOutline>().isDraw = true;
                        }

                        if (_object.GetComponent<SetCursorImage>() != null)
                        {
                            _object.GetComponent<SetCursorImage>().isDraw = true;
                        }
                    }
                }
            }
        }
    }
    public void VisibleParticle()
    {
        //�÷��̾� �ֺ��� ��ƼŬ�� ã�´�.
        Collider[] particles = Physics.OverlapSphere(transform.position, sightRaduis, 1 << LayerMask.NameToLayer("Particle"));
        for (int i = 0; i < particles.Length; i++)
        {
            //������ ��ƼŬ ���ϱ�
            Transform particle = particles[i].transform;

            //������ ��ƼŬ�� ���� ���ϱ�
            Vector3 ptcDir = (particle.position - (transform.position + sigthOffset)).normalized;

            //�÷��̾� �þ� ���ȿ� ��ƼŬ�� �ִٸ�
            if (Vector3.Angle(transform.forward, ptcDir) < sightAngle / 2)
            {
                //�÷��̾�� ��ƼŬ �Ÿ�
                float ptcDis = Vector3.Distance(transform.position + sigthOffset, particle.transform.position);

                if (ptcDis > sightRange)
                {
                    if (particle.GetChild(0).gameObject.activeSelf == true)
                    {
                        particle.GetChild(0).gameObject.SetActive(false);
                    }
                }
                else
                {
                    //if (!Physics.Raycast(transform.position, ptcDir, ptcDis, obstacleMask))
                    if (!Physics.Raycast(transform.position + sigthOffset, ptcDir, ptcDis, wallsMask))
                    {
                        Debug.DrawRay(transform.position + sigthOffset, ptcDir * ptcDis, Color.blue, 1f);
                        //��ƼŬ �������� ���̸� ���� ��ֹ��� �ɸ��� �ʴ´ٸ� ��ƼŬ Ȱ��ȭ
                        if (particle.GetChild(0).gameObject.activeSelf == false)
                        {
                            particle.GetChild(0).gameObject.SetActive(true);
                        }
                    }
                    else
                    {
                        //��ƼŬ �������� ���̸� ���� ��ֹ��� �ɸ��� ��ƼŬ ��Ȱ��ȭ
                        if (particle.GetChild(0).gameObject.activeSelf == true)
                        {
                            particle.GetChild(0).gameObject.SetActive(false);
                        }
                    }
                }
            }
        }
    }

    /* VisibleUnderWall()
    public void VisibleUnderWall()
    {
        //�÷��̾�������� sightRaduisũ���� OverlapSphere�� �׷� �� �ȿ� ���� ������ ã�´�.
        Collider[] targetUnderWall = Physics.OverlapSphere(transform.position, sightRaduis, 1 << LayerMask.NameToLayer("UnderWall"));

        //UnderWall�� ���̰� ���� y�� ���� ���� Ray�� ��� �Ϲݺ��� ���� �±� ������ y���� �������ش�.
        Vector3 rayPos = transform.position+sigthOffset;

        for (int i = 0; i < targetUnderWall.Length; i++)
        {
            //������ �� ���ϱ�
            Transform wall = targetUnderWall[i].transform;

            //������ ���� ���� ���ϱ�
            Vector3 wallDir = (wall.position - transform.position).normalized;

            //�÷��̾� �þ� �ޱ۾ȿ� ���� �ִٸ�
            if (Vector3.Angle(transform.forward, wallDir) < sightAngle / 2)
            {
                //�÷��̾�� ���� �Ÿ�
                float wallDis = Vector3.Distance(rayPos, wall.transform.position);

                //�÷��̾�� �� �������� ������ �Ÿ� ��ŭ ����ĳ��Ʈ�� �� ���� �޾ƿ´�.
                RaycastHit[] hit = Physics.RaycastAll(rayPos, wallDir, wallDis , 1 << LayerMask.NameToLayer("UnderWall"));
                Debug.DrawRay(rayPos, wall.position - transform.position, Color.black);
                //�浹�� ���� 2�� �̻��϶�

                if (hit.Length > 1f)
                {
                    //���� �Ž÷������� ���ش�. (�Ⱥ��̰� ��)
                    wall.GetComponent<MeshRenderer>().enabled = false;
                }
                else
                {
                    wall.GetComponent<MeshRenderer>().enabled = true;
                }
            }
        }
    }
    */

    /*
    //�� ������ ó��
    public void SightWall()
    {
        //�÷��̾�������� sightRaduisũ���� OverlapSphere�� �׷� �� �ȿ� ���� �� Ʈ���Ÿ� ã�´�.
        Collider[] checkWall = Physics.OverlapSphere(transform.position, sightRaduis, checkFadeMask);

        for (int i = 0; i < checkWall.Length; i++)
        {
            //������ �� Ʈ���� ���ϱ�
            Transform wall = checkWall[i].transform;

            //������ ���� ���� ���ϱ�
            Vector3 wallDir = (wall.position - (transform.position + sigthOffset)).normalized;

            //�÷��̾� �þ� �ޱ۾ȿ� Ʈ���Ű� �ִٸ�
            if (Vector3.Angle(transform.forward, wallDir) < sightAngle / 2)
            {
                //�÷��̾�� ���� �Ÿ�
                float wallDis = Vector3.Distance(transform.position + sigthOffset, wall.transform.position);

                //�÷��̾�� �� �������� ������ �Ÿ� ��ŭ ����ĳ��Ʈ�� �� Ʈ���Ÿ� �޾ƿ´�.
                //if (!Physics.Raycast(transform.position, wallDir, wallDis, obstacleMask))
                if (!Physics.Raycast(transform.position + sigthOffset, wallDir, wallDis, wallsMask))
                {
                    //�þ߿� Ʈ���Ű� �������� ���� ������ ó��
                    wall.GetComponentInParent<DrawWall>().isHide = true;
                }
                else
                {
                    //�þ߿��� Ʈ���Ű� �������� ���󺹱�
                    wall.GetComponentInParent<DrawWall>().isHide = false;
                    //Debug.Log(wall.parent.name);
                }


            }
        }
    }

    public void SightWall_2()
    {
        //�÷��̾�������� sightRaduisũ���� OverlapSphere�� �׷� �� �ȿ� ���� �� Ʈ���Ÿ� ã�´�.
        Collider[] checkWall = Physics.OverlapSphere(transform.position, sightRaduis, 1 << LayerMask.NameToLayer("CheckWall_2"));

        for (int i = 0; i < checkWall.Length; i++)
        {
            //������ �� Ʈ���� ���ϱ�
            Transform wall = checkWall[i].transform;

            //������ ���� ���� ���ϱ�
            Vector3 wallDir = (wall.position - (transform.position + sigthOffset)).normalized;

            //�÷��̾� �þ� �ޱ۾ȿ� Ʈ���Ű� �ִٸ�
            if (Vector3.Angle(transform.forward, wallDir) < sightAngle / 2)
            {
                //�÷��̾�� ���� �Ÿ�
                float wallDis = Vector3.Distance(transform.position + sigthOffset, wall.transform.position);

                //�÷��̾�� �� �������� ������ �Ÿ� ��ŭ ����ĳ��Ʈ�� �� Ʈ���Ÿ� �޾ƿ´�.
                //if (!Physics.Raycast(transform.position, wallDir, wallDis, obstacleMask))
                if (!Physics.Raycast(transform.position + sigthOffset, wallDir, wallDis, wallsMask))
                {
                    //�þ߿� Ʈ���Ű� �������� ���� ������ ó��
                    wall.GetComponentInParent<DrawWalls>().isHide = true;
                }
                else
                {
                    //�þ߿��� Ʈ���Ű� �������� ���󺹱�
                    wall.GetComponentInParent<DrawWalls>().isHide = false;
                }


            }
        }
    }
    */

    public void HideWall()
    {
        //Debug.DrawRay(transform.position + sigthOffset, Vector3.forward * 123f, Color.blue, 1f);
        //�÷��̾�������� sightRaduisũ���� OverlapSphere�� �׷� �� �ȿ� ���� checkWall���� ã�´�.
        Collider[] checkWalls = Physics.OverlapSphere(transform.position, sightRaduis, 1 << LayerMask.NameToLayer("CheckWall"));

        for (int i = 0; i < checkWalls.Length; i++)
        {
            //������ checkWall
            Transform checkWall = checkWalls[i].transform;

            //������ checkWall�� �θ� ���� �þ߳��� ���� ��Ͽ� �ִٸ�
            if (visibleWalls.Contains(checkWall.parent))
            {
                //checkWall ���� ���ϱ�
                Vector3 wallDir = (checkWall.position - (transform.position + sigthOffset)).normalized;

                //�÷��̾�� checkWall�� �Ÿ�
                float wallDis = Vector3.Distance(transform.position + sigthOffset, checkWall.transform.position);

                //checkWall �������� ���̸� ����
                RaycastHit[] hit = Physics.RaycastAll(transform.position + sigthOffset, wallDir, wallDis, wallsMask);
                
                //�ƹ��͵� ���� �ʾҴٸ�
                //(wllsMask, �� ���� ���� ���̰� ���� �ʾҴٴ°��� �÷��̾ �� �ڿ��� �ٶ� �ñ� ������ ���̰� ������ ���� ���� �������� ���� ������ó�����ش�.)
                if (hit.Length == 0)
                {
                    if (!checkWall.GetComponentInParent<DrawWalls>().isHide)
                    {
                        //���� �������ϰ� �ٲ��ش�.
                        checkWall.GetComponentInParent<DrawWalls>().isHide = true;
                    }
                }
                else
                {
                    //�ϳ� �̻� ���̿� ������ ���� ���� �������� �Ǻ��� �ش�.
                    for (int j = 0; j < hit.Length; j++)
                    {
                        //���� hit[j].transform.gameObject�� checkWall�� �θ��� ���� ������ ���� �ִ°��̱� ������ ���� �������ϰ� �Ѵ�.
                        if (hit[j].transform.gameObject == checkWall.parent.gameObject)
                        {
                            if (checkWall.GetComponentInParent<DrawWalls>().isHide)
                            {
                                //���� �������ϰ� �ٲ��ش�.
                                checkWall.GetComponentInParent<DrawWalls>().isHide = false;
                            }
                            break;
                        }
                        //checkWall�� �θ� �ɸ��� �ʾҴٸ� ���� �ڸ� ���� �ְ�, �ٸ� ���� �ɸ��������� �þ߳��� ���� ���̱� ������ ������ ó���� ���ش�.
                        else if (j == hit.Length - 1)
                        {
                            if (!checkWall.GetComponentInParent<DrawWalls>().isHide)
                            {
                                //���� �������ϰ� �ٲ��ش�.
                                checkWall.GetComponentInParent<DrawWalls>().isHide = true;
                            }
                        }
                    }
                }
            }
            else
            {
                if (!checkWall.GetComponentInParent<DrawWalls>().isHide)
                {
                    //���� �������ϰ� �ٲ��ش�.
                    checkWall.GetComponentInParent<DrawWalls>().isHide = true;
                }
            }
        }
    }

    //������ ������
    public bool Dropable(Vector3 DropPos)
    {
        DropPos.y = 0.5f;
        //DropPos������ �Ÿ��� ����
        float DropDis = Vector3.Distance(transform.position + sigthOffset, DropPos);

        //DropPos���� ������ ����
        Vector3 Dropdir = (DropPos - (transform.position + sigthOffset)).normalized;

        //�÷��̾���ġ�κ��� DropPos���� ���̸� ���� ��ֹ��� �ɸ��� ������
        if (!Physics.Raycast(transform.position + sigthOffset, Dropdir, DropDis, wallsMask))
        {
            //������ ����
            return true;
        }
        else
        {
            //�׸��� ������ ������ �Ұ���
            return false;
        }
    }

    public Vector3 DirFromAngle(float angleDegrees, bool angleIsGlobal)
    {

        if (!angleIsGlobal)
        {
            angleDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Cos((-angleDegrees + 90) * Mathf.Deg2Rad), 0, Mathf.Sin((-angleDegrees + 90) * Mathf.Deg2Rad));
    }

    private void DrawSight()
    {
        //sightAngle�� meshResolution�� ������ŭ �ɰ���.
        int stepCount = Mathf.RoundToInt(sightAngle * meshResolution);
        float stepAngleSize = sightAngle / stepCount;
        List<Vector3> viewPoints = new List<Vector3>();
        ViewCastInfo prevViewCast = new ViewCastInfo();

        for (int i = 0; i <= stepCount; i++)
        {
            //sightAngle�� �ɰ��� �þ߰����� ���� ViewCast�� ���ش�.
            float angle = transform.eulerAngles.y - sightAngle / 2 + stepAngleSize * i;
            ViewCastInfo newViewCast = ViewCast(angle);

            // i�� 0�̸� prevViewCast�� �ƹ� ���� ���� ���� ������ �� �� �����Ƿ� �ǳʶڴ�.
            if (i != 0)
            {
                bool edgeDstThresholdExceed = Mathf.Abs(prevViewCast.dst - newViewCast.dst) > edgeDstThreshold;

                // �� �� �� raycast�� ��ֹ��� ������ �ʾҰų� �� raycast�� ���� �ٸ� ��ֹ��� hit �� ���̶��(edgeDstThresholdExceed ���η� ���)
                if (prevViewCast.hit != newViewCast.hit || (prevViewCast.hit && newViewCast.hit && edgeDstThresholdExceed))
                {
                    Edge e = FindEdge(prevViewCast, newViewCast);

                    // zero�� �ƴ� ������ �߰���
                    if (e.PointA != Vector3.zero)
                    {
                        viewPoints.Add(e.PointA);
                    }

                    if (e.PointB != Vector3.zero)
                    {
                        viewPoints.Add(e.PointB);
                    }
                }
            }

            viewPoints.Add(newViewCast.point);
            prevViewCast = newViewCast;
        }

        int vertexCount = viewPoints.Count + 1;
        Vector3[] vertices = new Vector3[vertexCount];
        int[] triangles = new int[(vertexCount - 2) * 3];
        vertices[0] = Vector3.zero;
        for (int i = 0; i < vertexCount - 1; i++)
        {
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]) + Vector3.forward * maskCutawayDis;
            if (i < vertexCount - 2)
            {
                triangles[i * 3] = 0;
                triangles[i * 3 + 1] = i + 1;
                triangles[i * 3 + 2] = i + 2;
            }
        }
        viewMesh.Clear();
        viewMesh.vertices = vertices;
        viewMesh.triangles = triangles;
        viewMesh.RecalculateNormals();
    }

    public struct ViewCastInfo
    {
        public bool hit;
        public Vector3 point;
        public float dst;
        public float angle;
        public Transform visibleWall;

        public ViewCastInfo(bool _hit, Vector3 _point, float _dst, float _angle, Transform _wall)
        {
            hit = _hit;
            point = _point;
            dst = _dst;
            angle = _angle;
            visibleWall = _wall;
        }
    }

    ViewCastInfo ViewCast(float globalAngle)
    {
        //angle���� ������ �޾ƿ´�.
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit;
       // List<Transform> checkList = new List<Transform>();

        //�� �������� ���̸� ��� wallMask�� ��Ʈ �Ǿ��ٸ�
        if (Physics.Raycast(transform.position + drawOffset, dir, out hit, sightRaduis, wallsMask))
        {
            //���̴� ���� �߰����ش�.
            if (!visibleWalls.Contains(hit.transform))
            {
                visibleWalls.Add(hit.transform);
            }
                //hit.transform.GetComponent<MeshRenderer>().enabled = false;

                //hit�� ���� ������ �����Ѵ�.
                return new ViewCastInfo(true, hit.point, hit.distance, globalAngle, hit.transform);
        }
        else
        {
            //hit���� �ʾ����� flase�� �Բ� �þ��� ���Ÿ��� �����Ѵ�.
            return new ViewCastInfo(false, transform.position + dir * sightRaduis, sightRaduis, globalAngle, null);
        }

       
      
}

    public struct Edge
    {
        public Vector3 PointA, PointB;
        public Edge(Vector3 _PointA, Vector3 _PointB)
        {
            PointA = _PointA;
            PointB = _PointB;
        }
    }

    Edge FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = minAngle + (maxAngle - minAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);
            bool edgeDstThresholdExceed = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceed)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }

        return new Edge(minPoint, maxPoint);
    }
}
