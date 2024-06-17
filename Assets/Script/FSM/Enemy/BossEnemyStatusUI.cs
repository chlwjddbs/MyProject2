using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BossEnemyStatusUI : MonoBehaviour
{
    public static BossEnemyStatusUI instance;

    private Enemy_FSM bossEnemy;
    public GameObject HealthBar;
    public Image HealthFill;
    public TextMeshProUGUI hpText;

    private float currentHp;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        HealthBar.SetActive(false);
    }

   

    public void SetBoss(Enemy_FSM _bossEnemy)
    {
        HealthBar.SetActive(true);
        bossEnemy = _bossEnemy;
    }

    public void SetHpBar()
    {
        Debug.Log(bossEnemy.RemainHealth / bossEnemy.MaxHealth);
        HealthFill.fillAmount = bossEnemy.RemainHealth / bossEnemy.MaxHealth;

        currentHp = (bossEnemy.RemainHealth / bossEnemy.MaxHealth) * 100;

        if (currentHp < 1)
        {
            hpText.text = Mathf.Clamp(float.Parse(currentHp.ToString("N1")),0,100) + "%";
        }
        else
        {
            hpText.text = Mathf.Clamp(Mathf.FloorToInt(currentHp), 0, 100) + " %";
            
        }
        Debug.Log(bossEnemy.RemainHealth / bossEnemy.MaxHealth * 100 + " %");
    }
}
