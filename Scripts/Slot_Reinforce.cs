using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slot_Reinforce : SlotBase
{
    protected override void Update()
    {
        if (targetContainer != null)
        {
            if (targetContainer.item != null)
            {
                iconImage.sprite = targetContainer.item.icon; //이미지 따오기
                numberText.text = targetContainer.item.enchant.ToString(); // 강화수치 따오기
            }
            else // 아이템이 없는 경우
            {
                numberText.text = "";
            };
            // 대상 아이템이 있는 경우 활성화
            iconImage.enabled = (targetContainer.item != null);
        };

    }

    public override void OnClick()
    {
        ItemContainer container = InventoryBase.mouseContainer;

        if (InputManager.GetKeyState(MouseCode.LeftClick) == KeyState.On)
        {
            // 좌클릭 했을때 마우스컨테이너에 아이템이 장비템이라면 교환
            if(container.item != null && container.item.type > ItemType._장비_시작 && container.item.type < ItemType._장비_끝)
            {
                targetContainer.Swap(container);
            }
            else if(container.item == null && targetContainer.item != null) // 마우스컨테이너 비어있고 강화슬롯에 있다면 교체
            {
                targetContainer.Swap(container);
            };
        }
        else if(InputManager.GetKeyState(MouseCode.RightClick) == KeyState.On)
        {
            if(targetContainer != null)
            {
                int num = InventoryBase.inventoryDic["Inventory"].Add(targetContainer.item, targetContainer.Number); // 우클릭시 인벤토리에 넣어주고
                targetContainer.Number -= (1 - num); // 자리 없어서 돌려받는거 아니면 사라지도록
            };
        };
    }
}
