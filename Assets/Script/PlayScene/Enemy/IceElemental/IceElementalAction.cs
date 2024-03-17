using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceElementalAction : MonoBehaviour
{
    //���������� Dive ���
    public IceElemental iceElemental;
    //������ ���� ��Ʈ�ڽ�
    public CapsuleCollider AttackPoint;

    //������ �нú� ��ų(icecleBoom)
    //��ų ���� ��ġ
    private Vector3 icicleBoomPos;
    //��ų ����Ʈ ������
    public GameObject iceBoomPrefab;
    //��ų�� ������ ����
    public float iceBoomRange;

    //icicleBoom ������ ���� ǥ�� ������
    public GameObject icicleCirclePrefab;

    public float iceBlastGauge;
    private float MaxGauge = 100;
    [HideInInspector] public bool isIceBlast = false;

    public GameObject iceBlastCirclePrefab;
    private Vector3 iceBlastCirclePos = new Vector3(-131.75f, 0.6f, -175f);

    private Vector3 iceElementalOrignPos;

    public GameObject teleport;

    private bool isDeath = false;

    public Sound iceBoomSound;

    public GameObject iceElementalBody;

    private void Start()
    {
        //icicleBoom�� ���Ͱ� Ȱ��ȭ �� 1�� �� 2�ʸ��� �ݺ� ����
        InvokeRepeating("TripleIcicleBoom", 1f ,2.5f);
        iceElementalOrignPos = transform.position;
        AudioManager.instance.AddExternalSound(iceBoomSound);
    }

    private void Update()
    {
        //���Ͱ� ������
        if (iceElemental.isDeath && !isDeath)
        {
            isDeath = true;
            iceElemental.isAttack = false;
            //���ݵ��� ������ ��Ʈ�ڽ��� ���� ������ ��Ʈ�ڽ� ����
            AttackPoint.enabled = false;
            //icicleBoom ����
            CancelInvoke("TripleIcicleBoom");
            StopCoroutine("Blast");
        }

        
    }

    public void StartAttack()
    {
        //���ݽ� ��Ʈ�ڽ� ��
        AttackPoint.enabled = true;
    }

    public void EndAttack()
    {
        iceElemental.isAttack = false;
        iceElemental.eState = EnemyState.Idle;
        iceElemental.enemyAnime.SetInteger("eState", (int)iceElemental.eState);
        AttackPoint.enabled = false;
    }

    IEnumerator IcicleBoom()
    {
        //icicleBoom�� ���͸� �������� iceBoomRange�Ÿ� ���� �������� �ѷ��ִ� ��ǥ ���
        icicleBoomPos.x = iceElemental.GetComponent<Transform>().position.x + (Random.insideUnitCircle.normalized.x * iceBoomRange);
        icicleBoomPos.y = 1.5f;
        icicleBoomPos.z = iceElemental.GetComponent<Transform>().position.z + (Random.insideUnitCircle.normalized.y * iceBoomRange);

        Vector3 iciclePos = icicleBoomPos;

        GameObject iceCIrcle = Instantiate(icicleCirclePrefab, iciclePos, Quaternion.identity);
        Destroy(iceCIrcle, 1f);

        yield return new WaitForSeconds(1f);
        //icicleBoom ���� �� 2�� �� ����
        GameObject iceBoom = Instantiate(iceBoomPrefab, iciclePos, Quaternion.identity);
        Destroy(iceBoom, 2f);
        
    }

    public void TripleIcicleBoom()
    {
        StartCoroutine("TripleBoom");
    }

    //icicleBoom 3�� ����
    IEnumerator TripleBoom()
    {
        StartCoroutine("IcicleBoom");
        yield return new WaitForSeconds(.1f);

        StartCoroutine("IcicleBoom");
        yield return new WaitForSeconds(.1f);

        StartCoroutine("IcicleBoom");
        yield return new WaitForSeconds(.8f);

        iceElemental.PlayEnemySound("olf_iceboom");
    }

    public void AddBlastGauge(float Point)
    {
        //iceBlast ���� �߿� �������� ���� �ʰ��Ѵ�.
        if (isIceBlast)
        {
            return;
        }

        //�÷��̾ icicleBoom�� ������ iceBlast ������ ȹ��
        iceBlastGauge += Point;
        iceBlastGauge = Mathf.Clamp(iceBlastGauge, 0, MaxGauge);

        //Blast �������� 100�� �Ǹ� iceBloast ����
        if (iceBlastGauge == MaxGauge && isIceBlast == false)
        {
            StartCoroutine("Blast");
        }
    }

    IEnumerator Blast()
    {
        isIceBlast = true;
        iceElemental.isAttack = false;
        Instantiate(teleport, transform.position, Quaternion.identity);
        
        AttackPoint.enabled = false;
        iceElemental.isCasting = true;

        iceElemental.PlayEnemySound("olf_telpoin");

        iceElementalBody.SetActive(false);

        yield return new WaitForSeconds(1.25f);

        iceElemental.PlayEnemySound("olf_telpoout");

        yield return new WaitForSeconds(0.9f);

        transform.position = iceElementalOrignPos;
        iceElementalBody.SetActive(true);
        iceElemental.FollowIceEffect();
        iceElemental.SetState(EnemyState.Casting);
        GameObject iceBlastCircle = Instantiate(iceBlastCirclePrefab, iceBlastCirclePos, Quaternion.identity);
        
        Debug.Log("IceBlast");

        yield return new WaitForSeconds(5f);

        iceBlastCircle.GetComponent<IceBlast>().magicCircle.Stop();
        iceBlastCircle.GetComponent<IceBlast>().light_1.Stop();
        iceBlastCircle.GetComponent<IceBlast>().light_2.Stop();

        yield return new WaitForSeconds(1f);

        iceBlastCircle.GetComponent<IceBlast>().stopSound();
        iceBlastCircle.GetComponent<IceBlast>().iceBlast.SetActive(true);

        yield return new WaitForSeconds(.5f);

        iceBlastCircle.GetComponent<SphereCollider>().enabled = true;

        yield return new WaitForSeconds(1f);

        iceBlastCircle.GetComponent<SphereCollider>().enabled = false;
        iceElemental.isCasting = false;
        iceBlastGauge = 0f;
        iceElemental.SetState(EnemyState.Idle);

        yield return new WaitForSeconds(2f);
        isIceBlast = false;
        Destroy(iceBlastCircle);
    }
}
