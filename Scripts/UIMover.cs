using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIMover : MonoBehaviour
{
    public RectTransform transform_LT, transform_RB;

    public RectTransform window; // ������ UIâ

    public bool isMoving = false;
    void Update()
    {
        // ������ UI�𼭸��� ����� �𼭸� ��
        Vector3 diff_LT = UI_Manager.screenLeftTop - transform_LT.position;
        Vector3 diff_RB = UI_Manager.screenRightBottom - transform_RB.position;

        // â�� ȭ�� �����̶�� �������� �ʵ���
        diff_LT.y = Mathf.Min(diff_LT.y, 0);
        diff_RB.y = Mathf.Max(diff_RB.y, 0);

        diff_LT.x = Mathf.Max(diff_LT.x, 0);
        diff_RB.x = Mathf.Min(diff_RB.x, 0);

        // �����ŭ ä���� ������ ���ϰ� ��������
        window.position += (diff_LT + diff_RB);

        if(isMoving)
        {
            Vector3 mouseMove = Input.mousePosition - InputManager.mouseLastPosition;
            window.position += mouseMove;

            if(Input.GetMouseButtonUp(0)) // ���콺���� ��Ŭ�� ������ �ȿ����̵���
            {
                isMoving = false;
            }
        }
    }

    public void SetMove() // �����̵��� �ϴ� �Լ�(���콺 ��Ŭ����)
    {
        isMoving = true;
    }
}
