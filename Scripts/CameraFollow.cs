using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public float distance;
    public Transform CameraArm;

    void Update()
    {
        distance -= Input.GetAxis("Mouse ScrollWheel");     // 마우스 휠을 올리면 가까워지고 내리면 멀어지도록 한다

        distance = Mathf.Clamp(distance, 1f, 6.6f);       // 제한두기

        float calculatedDistance;                           // 최종적으로 계산된 거리

        RaycastHit hit;                  // 충돌정보
        Ray currentRay = new Ray();      // 내 선
        currentRay.origin = CameraArm.position;              // 출발점
        currentRay.direction = -CameraArm.forward.normalized; // 방향

        Physics.Raycast(currentRay, out hit, distance);      // 선 발사

        if (hit.collider == null) return;

        if (hit.collider.tag == "Ground")              // 지형에 부딪히면 줄어들음
        {
            calculatedDistance = hit.distance;
        }
        else                                  // 있다면 거기까지만
        {
            calculatedDistance = distance;
        }

        transform.localPosition = new Vector3(0, 0, 1) * -calculatedDistance;
    }

}
