using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundBurn : MonoBehaviour
{
    public bool isTurn = false;

    //Burn이 회전할 중심축 : FireElemental의 위치
    private Vector3 aroundPoint;

    //회전 속도
    public float turnSpeed;

    //burn의 지속 시간
    public float burnLifetime = 5f;
    //burn의 지속 시간을 초기화
    private float burnLifeReset = 5f;

    //burn의 공격력
    public float attackDamage = 20f;

    public GameObject explosionEffectPrefab;

    private float countdown = 0;
    private float resetcount = 1;

    private FireElemental fireElemental;

    public Sound[] burnSound;

    private void Start()
    {
        foreach (var s in burnSound)
        {
            AudioManager.instance.AddExternalSound(s);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //FireElemental이 사망시 스킬 제거
        if (fireElemental.isDeath)
        {
            ExplosionEffect();
            fireElemental.burnPool.Release(gameObject);
            //Destroy(gameObject);
            return;
        }
        BurnRotate();
    }

    public void SetBurn(FireElemental _fireElemental)
    {
        fireElemental = _fireElemental;
        aroundPoint = fireElemental.transform.position;
    }

    public void ResetBurn(float _burnLifeTiem)
    {
        burnLifetime = _burnLifeTiem;
        countdown = resetcount;
        isTurn = false;
    }

    public void BurnRotate()
    {
        if (isTurn)
        {
            //중심 축을 기준으로 trunSpeed의 속도로 회전
            transform.RotateAround(aroundPoint, Vector3.up, turnSpeed * Time.deltaTime);

            //burnLifetiem 이후 burn 제거
            burnLifetime -= Time.deltaTime;
            if (burnLifetime <= 0)
            {
                ExplosionEffect();
                ResetBurn(burnLifeReset);
                fireElemental.burnPool.Release(gameObject);
                //Destroy(gameObject);
            }
        }
    }

    public void ExplosionEffect()
    {
        GameObject explosionEffect = fireElemental.burnEffectPool.Get();
        if(explosionEffect.TryGetComponent<ReleasePool>(out ReleasePool value))
        {
            value.transform.position = transform.position;
            value.SetPool(fireElemental.burnEffectPool, 2f);
        }
        //StartCoroutine(DisableEffect(explosionEffect));
        //Destroy(explosionEffect, 2f);
    }

    /*
    IEnumerator DisableEffect(GameObject _explosionEffect)
    {
        yield return new WaitForSeconds(2f);
        fireElemental.burnEffectPool.Release(_explosionEffect);
    }
    
    public void DisableEffect(GameObject _explosionEffect)
    {
        fireElemental.burnEffectPool.Release(_explosionEffect);
    }
    */

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            AudioManager.instance.PlayExternalSound("burnHit");
            if(other.TryGetComponent<PlayerStatus>(out PlayerStatus value))
            {
                value.TakeDamage(attackDamage);
                //other.GetComponent<IAttackable>().TakeDamage(attackDamage, null);
                ExplosionEffect();
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.transform.CompareTag("Player"))
        {
            countdown -= Time.deltaTime;
            if (countdown <= 0)
            {
                AudioManager.instance.PlayExternalSound("burnHit");
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
