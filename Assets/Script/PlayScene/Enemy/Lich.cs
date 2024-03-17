using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lich : Enemy
{
    public GameObject lichballPrefab;
    public Transform lichballPoint;

    public GameObject spawnskeleonsPrefab;
    public GameObject spawnskeleonsAuraPrefab;
    public Transform spawnPoint;
    public float spawnskeleonsCooltime = 100f;
    public float coolDown = 0f;
    private bool isSkill = false;
    private bool isMove = false;

    public GameObject lichAura;


    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        SpawnSkeleton();
    }

    public override void Dagamed()
    {
        if (targetDis <= reactionRange)
        {
            isChasemode = true;
            enemyAnime.SetBool("isChaseMode", true);           
            isRunaway = false;
            chaseResetCount = chaseResetTime;
        }
    }
    public override void DetectRange()
    {
        SetState(EnemyState.Chase);
        enemyAnime.SetBool("isChaseMode", true);
    }

    public override void ActionRange()
    {

        //�ǰݽ� �÷��̾ �Ѿư���
        //�������� ������ �׼� ���������� �ٽ� �ǰ��� ���ϸ� ��� �����ϵ��� �Ѵ�.
        if (isChasemode)
        {
            SetState(EnemyState.Chase);
        }
        else if (isRunaway)
        {
            //����� ���¿��� �ǰݹ޾� ���� ���°� �� ��
            //�÷��̾ �׼� �������� ������ �������� �ʾƵ� ��� ���������Ѵ�.
            //�׷��� ������ �������� �϶� ��� ���� �� �� �ֵ��� �ƹ��� ������ ���� �ʴ´�.
        }
        else
        {
            SetNextPos();
            enemyAnime.SetBool("isChaseMode", false);
        }
    }
    public override void NotcognizeRange()
    {
        if (isRunaway)
        {
        }
        else if (isChasemode)
        {
            chaseResetCount -= Time.deltaTime;
            if (chaseResetCount <= 0)
            {
                isChasemode = false;
                SetNextPos();
                enemyAnime.SetBool("isChaseMode", false);
            }
        }
        else
        {        
            SetNextPos();
            enemyAnime.SetBool("isChaseMode", false);
        }
    }

    public void Attack()
    {
        GameObject Lichball = Instantiate(lichballPrefab,lichballPoint);
        Lichball.GetComponent<LichBall>().GetTargetPos(target.transform.position,target,attackDamage);
        isSkill = true;
    }
    public override void Die()
    {
        base.Die();
        //isDeath = true;
        //enemyAnime.SetBool("isDeath", isDeath);
        hitBox.enabled = false;
        agent.enabled = false;
        lichAura.SetActive(false);
        //target.GetComponent<PlayerStatus>().AddExp(exp);
    }

    public void SpawnSkeleton()
    {
        if (isDeath | !isSkill)
        {
            return;
        }

        if (coolDown > 0)
        {
            coolDown -= Time.deltaTime;
        }
        else
        {
            PlayEnemySound("spawnSkeleton");
            Instantiate(spawnskeleonsPrefab, spawnPoint.position, Quaternion.identity);
            GameObject spawnskeleonsAura = Instantiate(spawnskeleonsAuraPrefab, spawnPoint.position, Quaternion.identity);
            coolDown = spawnskeleonsCooltime;
            Destroy(spawnskeleonsAura, 2f);
        }
    }

    public override void SetState(EnemyState _state)
    {
        //���� ���¸� ���� �� �ִϸ��̼� ������ �ʰ� �������ش�.
        if (eState == _state || isDeath)
        {
            return;
        }

        eState = _state;

        //Debug.Log(eState);
        if(eState == EnemyState.Walk || eState == EnemyState.Chase || eState == EnemyState.Runaway)
        {
            if (!isMove)
            {
                isMove = true;
                PlayEnemySound(moveSound);
            }
        }
        else
        {
            isMove = false;
            StopEnemySound(moveSound);
        }

        if (_state == EnemyState.Chase || _state == EnemyState.Runaway)
        {
            enemyAnime.SetInteger("eState", (int)(EnemyState.Walk));
        }
        else
        {         
            enemyAnime.SetInteger("eState", (int)eState);
        }

        //���°� �ٲ� �� �н��� ������ �ش�.
        //�����ϰų� �װų� �� ���ߴ� �ൿ�� �� �� ��ΰ� ���������� �̲������µ��� �ִϸ��̼��� ������
        agent.ResetPath();
        agent.velocity = Vector3.zero;
    }
}
