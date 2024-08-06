using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotBase : MonoBehaviour
{
    public Image iconImage;                 // 슬롯에 있을 아이템이미지
    public TextMeshProUGUI numberText;      // 아이템 개수 텍스트
    public InventoryBase parentInventory;   // 부모가 되는 인벤토리

    protected ItemContainer targetContainer;      // 이 슬롯이 보여줄 아이템컨테이너

    protected virtual void Start()
    {
        if (parentInventory == null) parentInventory = GetComponentInParent<InventoryBase>();
    }

    protected virtual void Update()
    {
        if(targetContainer != null)
        {
            if(targetContainer.item != null)
            {
                iconImage.sprite = targetContainer.item.icon; //이미지 따오기
                numberText.text = targetContainer.Number.ToString(); // 수 따오기
            }
            else // 아이템이 없는 경우
            {
                numberText.text = "";
            };
            // 대상 아이템이 있는 경우 활성화
            iconImage.enabled = (targetContainer.item != null);
        };
    }

    public void SetContainer(ItemContainer target) // 보여줄 아이템 컨테이너 설정
    {
        targetContainer = target;
    }

    public virtual void OnClick()
    {
        parentInventory?.OnClick(targetContainer);
    }

    public virtual void MouseOver() //마우스가 슬롯 위에 올라가면 
    {
        if (targetContainer.item == null) return;

        ShowItemInfo.targetItem[0] = targetContainer.item;
        CloserableUIBase.OpenUI("ItemInfo");
    }

    public virtual void MouseExit()
    {
        CloserableUIBase.CloseUI("ItemInfo");
    }
}
