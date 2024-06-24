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
    public TextMeshProUGUI nameText;

    private float currentHp;
    private Color over45 = new Color32(35, 35, 35, 255);
    private Color under45 = new Color32(255, 35, 35, 255);

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        HealthBar.SetActive(false);
        nameText.gameObject.SetActive(false);
    }

   

    public void SetBoss(Enemy_FSM _bossEnemy)
    {
        HealthBar.SetActive(true);
        nameText.gameObject.SetActive(true);
        bossEnemy = _bossEnemy;
        SetHpBar();
        nameText.text = bossEnemy.name;
    }

    public void SetHpBar()
    {
        Debug.Log(bossEnemy.RemainHealth / bossEnemy.MaxHealth);
        HealthFill.fillAmount = bossEnemy.RemainHealth / bossEnemy.MaxHealth;

        currentHp = (bossEnemy.RemainHealth / bossEnemy.MaxHealth) * 100;

        if (currentHp < 1)
        {
            hpText.text = Mathf.Clamp(float.Parse(currentHp.ToString("N1")),0,100) + "%";
            hpText.color = under45;
        }
        else
        {
            hpText.text = Mathf.Clamp(Mathf.FloorToInt(currentHp), 0, 100) + " %";
            if(currentHp >= 45)
            {
                hpText.color = over45;
            }
            else
            {
                hpText.color = under45;
            }
        }
        Debug.Log(bossEnemy.RemainHealth / bossEnemy.MaxHealth * 100 + " %");
    }

    public void Death()
    {
        HealthBar.SetActive(false);
        nameText.gameObject.SetActive(false);
        bossEnemy = null;
    }
}
