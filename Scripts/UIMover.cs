using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMover : MonoBehaviour
{
    public RectTransform transform_LT, transform_RB;

    public RectTransform window; // 움직일 UI창

    public bool isMoving = false;
    void Update()
    {
        // 움직일 UI모서리와 모니터 모서리 비교
        Vector3 diff_LT = UI_Manager.screenLeftTop - transform_LT.position;
        Vector3 diff_RB = UI_Manager.screenRightBottom - transform_RB.position;

        // 창이 화면 안쪽이라면 움직이지 않도록
        diff_LT.y = Mathf.Min(diff_LT.y, 0);
        diff_RB.y = Mathf.Max(diff_RB.y, 0);

        diff_LT.x = Mathf.Max(diff_LT.x, 0);
        diff_RB.x = Mathf.Min(diff_RB.x, 0);

        // 벗어난만큼 채워서 나가지 못하고 막히도록
        window.position += (diff_LT + diff_RB);

        if(isMoving)
        {
            Vector3 mouseMove = Input.mousePosition - InputManager.mouseLastPosition;
            window.position += mouseMove;

            if(Input.GetMouseButtonUp(0)) // 마우스에서 좌클릭 뗀순간 안움직이도록
            {
                isMoving = false;
            }
        }
    }

    public void SetMove() // 움직이도록 하는 함수(마우스 좌클릭시)
    {
        isMoving = true;
    }
}
