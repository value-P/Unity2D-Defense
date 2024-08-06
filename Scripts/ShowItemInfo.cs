using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ShowItemInfo : MonoBehaviour
{
    public static ItemBase[] targetItem = new ItemBase[2]; // 아이템 비교도 할 수 있기 때문에 2개
    public TextMeshProUGUI targetText;

    public int targetIndex; // 대상이 되는 텍스트
    public Image targetImage; // 대상이 되는 이미지

    public RectTransform rectTransform; // 피봇조정을 위한 rectTransform

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
