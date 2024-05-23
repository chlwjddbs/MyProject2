using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceEffect : MonoBehaviour
{
    public IceElemental olf;
    private PlayerStatus playerStatus;

    public GameObject onwer;

    public float slowRate = 0.4f;
    
    private float slowTime = 2f;
    private float count;

    private bool isStay = false;

    //나중에 다수에게 슬로우를 걸기 위해 만들어둔다. 추구 구현하자.
    private List<IMovement> targetList = new List<IMovement>();

    // Update is called once per frame
    void Update()
    {
        count = Mathf.Clamp(count, 0, slowTime);

        if(count <= 0)
        {
            if(playerStatus != null)
            {
                isStay = false;
                playerStatus.ResetMoveSpeed(SpeedRateEnum.minusRate, slowRate);
                //playerStatus.ResetMoveSpeed();
            }
        }
        count -= Time.deltaTime;
        //Debug.Log(count);
    }

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            if (!isStay)
            {
                isStay = true;
                olf.PlayEnemySound("freeze");
            }

            if (other.TryGetComponent<IMovement>(out IMovement value))
            {
                value.MinusMoveSpeed(slowRate, slowTime);
                playerStatus = other.GetComponent<PlayerStatus>();
            }
            count = slowTime;
        }
    }

    private void OnDisable()
    {
        if(playerStatus != null)
        {
            playerStatus.ResetMoveSpeed(SpeedRateEnum.minusRate,slowRate);
        }

        /*
        foreach (var target in targetList)
        {
            target.ResetMoveSpeed(SpeedRateEnum.minusRate, slowRate);
        }
        */
    }
}

/*
private void OnParticleCollision(GameObject other)
{
    if (other.CompareTag("Player"))
    {
        if (!isStay)
        {
            isStay = true;
            olf.PlayEnemySound("freeze");
        }
        player = other.GetComponent<PlayerController>();
        playerStatus = other.GetComponent<PlayerStatus>();
        playerStatus.moveSpeed = playerStatus.baseMoveSpeed * (1 - slowRate);
        playerStatus.moveSpeedUI.text = playerStatus.moveSpeed.ToString();
        count = slowTime;
    }
}
*/