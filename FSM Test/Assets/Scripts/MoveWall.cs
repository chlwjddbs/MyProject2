using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWall : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 1f;

    // 1:¿À¸¥ÂÊ, -1:¿ÞÂÊ
    [SerializeField]
    private float dir = 1f;

    [SerializeField]
    private float moveTime = 1f;
    private float countdown = 0;


    // Update is called once per frame
    void Update()
    {
        countdown += Time.deltaTime;
        if(countdown >= moveTime)
        {
            dir *= -1;
            countdown = 0;
        }

        transform.Translate(Vector3.right * dir * moveSpeed * Time.deltaTime, Space.World);
    }
}
