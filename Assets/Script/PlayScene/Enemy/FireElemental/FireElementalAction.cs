using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireElementalAction : MonoBehaviour
{
    public FireElemental fireElemental;
    
    public Transform treepleBurn;
    public GameObject burnCirclePrefab;
    public GameObject burnPrefab;

    //public float Test;

    public float burnCount;
    private float startburnStack = 3;
    private float resetCount = 0;

    private void Update()
    {
        //3������ ���̸�
        if (burnCount == startburnStack)
        {
            //Burn�� ȸ����Ų��
            RotateBurn();
        }
    }

    public void TreepleBurn()
    {
        StartCoroutine("SpawnBurn");
    }

    IEnumerator SpawnBurn()
    {
        //���ݽ� Burn�� ��ȯ �� ��ġ�� �̸� �˷��ִ� ������ ����
        //��ȯ �� ��ġ : �÷��̾��� ��ġ
        Vector3 burnPoint = new Vector3(fireElemental.target.transform.position.x, 1f, fireElemental.target.transform.position.z);
        GameObject burn_circle = Instantiate(burnCirclePrefab, burnPoint, Quaternion.identity);      

        yield return new WaitForSeconds(1f);

        Vector3 burnpos = burn_circle.transform.position;

        //������ 1
        if (!fireElemental.isPase_2)
        {
            GameObject _burn = Instantiate(burnPrefab, burnpos, Quaternion.identity, treepleBurn);
            _burn.GetComponent<RotateAroundBurn>().attackDamage = GetComponent<FireElemental>().attackDamage;
        }
        //������ 2
        else
        {
            //��Ⱥ�Ǵ� burn�� �ӵ��� 1.5�� ����
            GameObject _burn = Instantiate(burnPrefab, burnpos, Quaternion.identity, treepleBurn);
            _burn.GetComponent<RotateAroundBurn>().turnSpeed *= 1.5f;
            _burn.GetComponent<RotateAroundBurn>().attackDamage = GetComponent<FireElemental>().attackDamage * 1.5f;
            //_burn.GetComponent<RotateAroundBurn>().burnLifetime *= 1.5f;
        }

        //burn�� ���� ī��Ʈ ����
        burnCount++;
       
    }

    public void RotateBurn()
    {
        StartCoroutine("TreepleBurnStart");
    }

    IEnumerator TreepleBurnStart()
    {
        yield return new WaitForSeconds(.5f);
        //burn�� ��ȯ�Ǵ� ���� ����� (treepleBurn) �� �ڽ�(burn)�� 3��(3����)�� �Ǹ� TreepleBurn ����
        for (int i = 0; i < treepleBurn.childCount; i++)
        {
            //RotateAroundBurn�� isTrun�� true�� �Ǹ� TreeplBurn �ߵ�
            treepleBurn.GetChild(i).GetComponent<RotateAroundBurn>().isTurn = true;    
        }

        //��ų�� �ߵ��Ǹ� ���� �ʱ�ȭ
        burnCount = resetCount;
    }

    public void endAttack()
    {
        fireElemental.isAttack = false;
        fireElemental.eState = EnemyState.Idle;
        fireElemental.enemyAnime.SetInteger("eState", (int)fireElemental.eState);  
    }
}
