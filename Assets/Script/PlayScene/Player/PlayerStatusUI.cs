using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatusUI : MonoBehaviour
{
    public GameObject playerStatusUI;
    public PlayerStatus playerStatus;
    public Player player;
    public Image equipWeaponImage;
    public Sprite UnequipWeaponImage;
    public Image HpBar;
    public Image MpBar;
  
    public RectTransform[] userNameUI;
    public RectTransform[] levelUI;
    public RectTransform[] expUI;
    public RectTransform[] hpUI;
    public RectTransform[] mpUI;
    public RectTransform[] damageUI;
    public RectTransform[] defenceUI;
    public RectTransform[] moveSpeedUI;

    public TextMeshProUGUI userName;
    public TextMeshProUGUI level;
    public TextMeshProUGUI exp;
    public TextMeshProUGUI hp;
    public TextMeshProUGUI mp;
    public TextMeshProUGUI damage;
    public TextMeshProUGUI defence;
    public TextMeshProUGUI moveSpeed;
    // Start is called before the first frame update
    void Start()
    {
        //���� ���۽� ���⸦ �������� ���� �󽽷� �̹����� �����ش�.
        equipWeaponImage.sprite = UnequipWeaponImage;
    }

    public void SetAllUI()
    {
        userName.text = GameData.instance.userData.userName;
        LayoutRebuilder.ForceRebuildLayoutImmediate(userNameUI[0]);
        SetLevelUI();
        SetExpUI();
        SetHpUI();
        SetMpUI();
        SetDamageUI();
        SetDefenceUI();
        SetMoveSpeedUI();
    }

    public void SetLevelUI()
    {
        level.text = player.PlayerLv.ToString();
        LayoutRebuilder.ForceRebuildLayoutImmediate(levelUI[0]);
        LayoutRebuilder.ForceRebuildLayoutImmediate(levelUI[1]);
    }

    public void SetExpUI()
    {
        exp.text = player.CurrentExp + " / " + player.NextLvExp;
        LayoutRebuilder.ForceRebuildLayoutImmediate(expUI[0]);
        LayoutRebuilder.ForceRebuildLayoutImmediate(expUI[1]);
    }

    public void SetHpUI()
    {
        HpBar.fillAmount = player.RemainHealth / player.MaxHealth;
        hp.text = player.RemainHealth + " / " + player.MaxHealth;
        LayoutRebuilder.ForceRebuildLayoutImmediate(hpUI[0]);
        LayoutRebuilder.ForceRebuildLayoutImmediate(hpUI[1]);
    }

    public void SetMpUI()
    {
        MpBar.fillAmount = player.RemainMana / player.MaxMana;
        mp.text = player.RemainMana + " / " + player.MaxMana;
        LayoutRebuilder.ForceRebuildLayoutImmediate(mpUI[0]);
        LayoutRebuilder.ForceRebuildLayoutImmediate(mpUI[1]);
    }

    public void SetDamageUI()
    {
        damage.text = player.AttackDamage.ToString();
        LayoutRebuilder.ForceRebuildLayoutImmediate(damageUI[0]);
        LayoutRebuilder.ForceRebuildLayoutImmediate(damageUI[1]);
    }

    public void SetDefenceUI()
    {
        defence.text = player.DefencePoint.ToString();
        LayoutRebuilder.ForceRebuildLayoutImmediate(defenceUI[0]);
        LayoutRebuilder.ForceRebuildLayoutImmediate(defenceUI[1]);
    }

    public void SetMoveSpeedUI()
    {
        moveSpeed.text = player.MoveSpeed.ToString();
        LayoutRebuilder.ForceRebuildLayoutImmediate(moveSpeedUI[0]);
        LayoutRebuilder.ForceRebuildLayoutImmediate(moveSpeedUI[1]);
    }

    /*
    public void SortingUI()
    {
        RectTransform[] rec = levelTitle.GetComponentsInChildren<RectTransform>();
        for (int i = 0; i < rec.Length; i++)
        {
            //��� �����̳� ���� ���� �� layout�� �����Ͽ� �������ð� �ʿ��Ҷ� ����Ѵ�.
            LayoutRebuilder.ForceRebuildLayoutImmediate(rec[i]);
        }
    }
    */

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
}
