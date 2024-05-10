using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    //카메라 높이
    public float height = 5f;
    //카메라와 타겟사이의 거리
    public float distance = 10f;
    //카메라가 돌아가는 회전 값
    public float angle = 45f;
    //카메라가 바라보는 지점 offset
    public float lookHeight = 1f;

    //이동속도
    public float speed = 10f;

    private void Start()
    {
        UpdateCamera();
    }

    private void LateUpdate()
    {
        UpdateCamera();
    }

    void UpdateCamera()
    {
        if (target == null)
            return;

        Vector3 worldPostion = (Vector3.forward * -distance) + (Vector3.up * height);
        Vector3 rotateVector = Quaternion.AngleAxis(angle, Vector3.up) * worldPostion;

        Vector3 targetPostion = target.position;
        targetPostion.y += lookHeight;

        Vector3 finalPosition = targetPostion + rotateVector;
        transform.position = Vector3.Lerp(transform.position, finalPosition, speed * Time.deltaTime);

        transform.LookAt(targetPostion);
    }
}
