using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class IceElemental : Enemy
{
    public GameObject IceEffect;
    private IceElementalAction action;

    public UnityAction blast;

    public GameObject deathEffect;

    public TeleportGate teleprotGate;

    // Start is called before the first frame update
    protected override void Start()
    {
        enemyAnime = GetComponentInChildren<Animator>();
        currentHealth = startHealth;
        maxHealth = startHealth;
        agent = GetComponent<NavMeshAgent>();
        action = GetComponent<IceElementalAction>();
    }

    protected override void Update()
    {
        PlayerDeath();

        if (isDeath || !isActive || isStop)
        {
            return;
        }

        if (isCasting)
        {
            return;
        }

        FollowIceEffect();

        CheckTargetPos();

        if (attackTimer > 0)
        {
            attackTimer -= Time.deltaTime;
        }

        if (targetDis <= attackRange)
        {
            if (attackTimer <= 0)
            {
                isAttack = true;
                SetState(EnemyState.Attack);
                attackTimer = attackCoolTime;
            }
            else if (attackTimer > 0 && !isAttack)
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * rotateSpeed);
            }
        }
        else if (targetDis > attackRange && !isAttack)
        {
            SetState(EnemyState.Chase);
        }

        SwitchState();
    }

    //���ο� ��������Ʈ�� iceElemental�� ����ٴ�
    public void FollowIceEffect()
    {
        IceEffect.transform.position = new Vector3(transform.position.x, IceEffect.transform.position.y, transform.position.z);
    }

    public override void LookChase()
    {
        
    }

    public override void TakeDamage(float _damage)
    {
        if (isDeath)
        {
            return;
        }

        //���� �������� 0���� Ŭ��
        if (_damage > 0)
        {
            //HP����
            currentHealth -= _damage;
            //Damaged ����
            Dagamed();
            Debug.Log((currentHealth/maxHealth) * 100 + "%");
            PlayEnemySound(damagedSound);
        }
        
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    public override void Dagamed()
    {
        //iceBlast ���� �� �������� ���� ��
        if (action.isIceBlast)
        {
            //IceBlast���� ����� blast ȣ��
            //IceBlast�� ũ�Ⱑ �پ��.
            blast?.Invoke();
        }
    }

    public override void Die()
    {
        base.Die();

        IceEffect.SetActive(false);
        PlayEnemySound("olf_dead1");
        deathEffect.SetActive(true);
        Destroy(deathEffect, 3f);
        teleprotGate.transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        teleprotGate.gateCoordinate = teleprotGate.transform.position;
        teleprotGate.gameObject.SetActive(true);
        teleprotGate.OpenGate();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDeath)
        {
            return;
        }
        if (other.CompareTag("Player"))
        {
            other.GetComponent<PlayerStatus>().TakeDamage(attackDamage);
            PlayEnemySound("olf_hit");
        }
    }
}
