using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStatusUI : MonoBehaviour
{
    public GameObject playerStatusUI;
    public PlayerStatus playerStatus;
    public Image equipWeaponImage;
    public Sprite UnequipWeaponImage;
    public Image HpBar;
    public Image MpBar;
    // Start is called before the first frame update
    void Start()
    {
        //���� ���۽� ���⸦ �������� ���� �󽽷� �̹����� �����ش�.
        equipWeaponImage.sprite = UnequipWeaponImage;
    }

    //�÷��̾��� HP�� ������ ������ �� hpbar fillamount ����
    public void SetHpBar()
    {
        HpBar.fillAmount = playerStatus.remainHealth / playerStatus.maxHealth;
        playerStatus.hpUI.text = playerStatus.remainHealth + " / " + playerStatus.maxHealth;
        PlayerStatus.stausUI?.Invoke();
    }

    //�÷��̾��� MP�� ������ ������ �� mpbar fillamount ����
    public void SetMpBar()
    {
        MpBar.fillAmount = playerStatus.remainMana / playerStatus.maxMana;
        playerStatus.mpUI.text = playerStatus.remainMana + " / " + playerStatus.maxMana;
        PlayerStatus.stausUI?.Invoke();
    }

    //������ ����� �̹����� �����ش�.
    public void EquipWeapon(Sprite _weaponImage)
    {
        equipWeaponImage.sprite = _weaponImage;
    }

    //�� ���� �̹����� �����ش�.
    public void UnEquipWeapon()
    {
        equipWeaponImage.sprite = UnequipWeaponImage;
    }
}
