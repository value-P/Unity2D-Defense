using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float distance;
    public Transform CameraArm;

    void Update()
    {
        distance -= Input.GetAxis("Mouse ScrollWheel");     // ���콺 ���� �ø��� ��������� ������ �־������� �Ѵ�

        distance = Mathf.Clamp(distance, 1f, 6.6f);       // ���ѵα�

        float calculatedDistance;                           // ���������� ���� �Ÿ�

        RaycastHit hit;                  // �浹����
        Ray currentRay = new Ray();      // �� ��
        currentRay.origin = CameraArm.position;              // �����
        currentRay.direction = -CameraArm.forward.normalized; // ����

        Physics.Raycast(currentRay, out hit, distance);      // �� �߻�

        if (hit.collider == null) return;

        if (hit.collider.tag == "Ground")              // ������ �ε����� �پ����
        {
            calculatedDistance = hit.distance;
        }
        else                                  // �ִٸ� �ű������
        {
            calculatedDistance = distance;
        }

        transform.localPosition = new Vector3(0, 0, 1) * -calculatedDistance;
    }

}
