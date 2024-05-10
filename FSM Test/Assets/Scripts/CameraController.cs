using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    //ī�޶� ����
    public float height = 5f;
    //ī�޶�� Ÿ�ٻ����� �Ÿ�
    public float distance = 10f;
    //ī�޶� ���ư��� ȸ�� ��
    public float angle = 45f;
    //ī�޶� �ٶ󺸴� ���� offset
    public float lookHeight = 1f;

    //�̵��ӵ�
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
