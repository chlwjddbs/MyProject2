using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Weapon : MonoBehaviour
{
    public PlayerStatus playerStatus;
    public PlayerController playerController;

    //���� �� Ÿ�� ������ �� MeshCollider
    private MeshCollider attackPoint;

    //���� �� ���Ⱑ ���� �⺻ ������ Mesh
    public Mesh basicMesh;

    public EquipItem equipWeapon;

    public float attackDamage;

    private bool isDamage;

    public bool isMultiple = false;

    public List<Collider> enemies;

    public UnityAction AddUpdateEquip;

    public AnimationClip backAttackClip;

  
    // Start is called before the first frame update
    void Start()
    {
        //�⺻ Mesh�� �ʱ�ȭ
        attackPoint = GetComponent<MeshCollider>();
        attackPoint.sharedMesh = basicMesh;
        isDamage = true;
        Equipment.instance.UpdateDamage += SetAttackDamage;
    }

    //������ ������
    public void EquipWeapon(EquipItem weapon)
    {
        equipWeapon = weapon;

        //������ �������� Mesh�� ����
        attackPoint.sharedMesh = equipWeapon.mesh;
        
        playerController.SetAttackAnime(equipWeapon.attackClip);
        
        //������ �������� ���ݷ� �߰�
        //playerStatus.equipDamage = equipWeapon.attack;
        //SetAttackDamage();
    }

    //������ ���� ������
    public void UnequipWeapon()
    {
        //���� ���ݷ� ����
        //playerStatus.equipDamage -= equipWeapon.attack;

        equipWeapon = null;
        //���� Mesh �⺻ Mesh�� ����
        attackPoint.sharedMesh = basicMesh;

        playerController.SetAttackAnime(backAttackClip);
        
        //SetAttackDamage();
    }

    public void SetSkillDage(float _addDage, bool _multiple)
    {
        isDamage = true;
        isMultiple = _multiple;
        attackDamage = (playerStatus.currentDamage) * _addDage;
    }

    public void SetAttackDamage()
    {      
        isDamage = true;
        isMultiple = false;
        attackDamage = playerStatus.currentDamage;
    }

    public void SetDamage()
    {
        attackDamage = playerStatus.currentDamage;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Obstacle"))
        {
            attackPoint.enabled = false;
        }

        if (other.CompareTag("Enemy"))
        {
            if (!enemies.Contains(other))
            {
                enemies.Add(other);
                if (isMultiple)
                {
                    if (other != null)
                    {
                        if (PlayerController.isAction && isDamage)
                        {
                            Vector3 damageDir = (other.transform.position - transform.position).normalized;
                            //damageDir.y = 0.5f;
                            other.GetComponent<Enemy>().Runaway(damageDir);
                            other.GetComponent<Enemy>().TakeDamage(attackDamage);
                        }
                    }
                }
                else
                {
                    if (enemies.Count < 2)
                    {
                        if (other != null)
                        {
                            if (PlayerController.isAction && isDamage)
                            {
                                Vector3 damageDir = (other.transform.position - transform.position).normalized;
                                //damageDir.y = 0.5f;
                                other.GetComponent<Enemy>().Runaway(damageDir);
                                other.GetComponent<Enemy>().TakeDamage(attackDamage);
                            }
                        }
                    }
                }
            }
        }
    }
}
