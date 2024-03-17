using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundBurn : MonoBehaviour
{
    public bool isTurn = false;

    //Burn�� ȸ���� �߽��� : FireElemental�� ��ġ
    private Vector3 aroundPoint;

    //ȸ�� �ӵ�
    public float turnSpeed;

    //burn�� ���� �ð�
    public float burnLifetime = 5f;

    //burn�� ���ݷ�
    public float attackDamage = 20f;

    public GameObject explosionEffectPrefab;

    private float countdown = 0;
    private float resetcount = 1;

    private FireElemental fireElemental;

    public Sound[] burnSound;

    private void Start()
    {
        fireElemental = FindObjectOfType<FireElemental>();
        aroundPoint = fireElemental.transform.position;

        foreach (var s in burnSound)
        {
            AudioManager.instance.AddExternalSound(s);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //FireElemental�� ����� ��ų ����
        if (fireElemental.isDeath)
        {
            ExplosionEffect();
            Destroy(gameObject);
            return;
        }
        BurnRotate();
    }

    public void BurnRotate()
    {
        if (isTurn)
        {
            //�߽� ���� �������� trunSpeed�� �ӵ��� ȸ��
            transform.RotateAround(aroundPoint, Vector3.up, turnSpeed * Time.deltaTime);

            //burnLifetiem ���� burn ����
            burnLifetime -= Time.deltaTime;
            if (burnLifetime <= 0)
            {
                ExplosionEffect();
                Destroy(gameObject);
            }
        }

    }

    public void ExplosionEffect()
    {
        GameObject explosionEffect = Instantiate(explosionEffectPrefab, new Vector3(transform.position.x, transform.position.y+0.5f, transform.position.z) ,Quaternion.identity);
        Destroy(explosionEffect, 2f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            AudioManager.instance.PlayExSound("burnHit");
            other.GetComponent<PlayerStatus>().TakeDamage(attackDamage);
            ExplosionEffect();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0)
            {
                AudioManager.instance.PlayExSound("burnHit");
                other.GetComponent<PlayerStatus>().TakeDamage(attackDamage);
                ExplosionEffect();
                countdown = resetcount;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            countdown = resetcount;
        }
    }

}
