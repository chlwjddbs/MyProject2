using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    private PlayerStatus playerStatus;
    public EquipItem equipShield;
    // Start is called before the first frame update
    void Start()
    {
        playerStatus = GetComponentInParent<PlayerStatus>();
    }

    //������ ������
    public void EquipShield(EquipItem _shield)
    {
        equipShield = _shield;

        //������ �������� ���ݷ� �߰�
        //playerStatus.equipDefence = equipShield.defence;
    }

    //������ ���� ������
    public void UnequipShield()
    {
        //���� ���ݷ� ����
        //playerStatus.equipDefence -= equipShield.defence;
        equipShield = null;
    }

}
