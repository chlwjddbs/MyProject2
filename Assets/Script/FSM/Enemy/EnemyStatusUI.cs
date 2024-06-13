using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyStatusUI : MonoBehaviour
{
    private Enemy_FSM enemy;
    public GameObject HpBar;
    public Image HpFill;
    public bool visible;
    private float visibleCount = 5;
    private float countDown;
 
    public void SetData(Enemy_FSM _enemy)
    {
        enemy = _enemy;
        HpBar.SetActive(false);
        HpFill.fillAmount = 1;
        transform.eulerAngles = new Vector3(55, 42.5f, 0);
        visible = false;
        
    }

    public void Updata()
    {
        if (visible)
        {
            transform.eulerAngles = new Vector3(55, 42.5f, 0);
            countDown -= Time.deltaTime;
            if(countDown <= 0)
            {
                visible = false;
                HpBar.SetActive(false);
            }
        }
    }

    public void SetHpBar(bool damaged)
    {
        HpFill.fillAmount = enemy.RemainHealth / enemy.MaxHealth;

        if (damaged)
        {
            countDown = visibleCount;
            visible = true;
            HpBar.SetActive(true);
        }
    }

    public void EnemyDeath()
    {
        StartCoroutine(Death());
    }

    IEnumerator Death()
    {
        transform.eulerAngles = new Vector3(55, 42.5f, 0);
        yield return new WaitForSeconds(2f);
        visible = false;
        HpBar.SetActive(false);
    }
}
