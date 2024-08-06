using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowItemInfo : MonoBehaviour
{
    public static ItemBase[] targetItem = new ItemBase[2]; // ������ �񱳵� �� �� �ֱ� ������ 2��
    public TextMeshProUGUI targetText;

    public int targetIndex; // ����� �Ǵ� �ؽ�Ʈ
    public Image targetImage; // ����� �Ǵ� �̹���

    public RectTransform rectTransform; // �Ǻ������� ���� rectTransform

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    private void Update()
    {
        targetText.text = targetItem[targetIndex] != null ? targetItem[targetIndex].GetContext() : "";
        if (targetItem[targetIndex] != null) targetImage.sprite = targetItem[targetIndex].icon;

        Vector2 calPivot = new Vector2();

        calPivot.x = Input.mousePosition.x > Screen.width * 0.5f ? 1 : 0;
        calPivot.y = Input.mousePosition.y > Screen.height * 0.5f ? 1 : 0;

        rectTransform.pivot = calPivot;
        transform.position = Input.mousePosition;
    }

    private void OnEnable()
    {
        transform.position = Input.mousePosition;

    }
}
