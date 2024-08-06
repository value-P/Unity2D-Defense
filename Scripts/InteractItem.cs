using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractItem : InteractBase
{
    public InventoryTab itemList = new InventoryTab_Luggage(null, new Vector2Int(5,5));
    public InventoryBase Inven => InventoryBase.inventoryDic["Luggage"];

    public override void Interact()
    {
        if (Inven) Inven.SetTab(itemList); // UI갱신

        CloserableUIBase.OpenUI("Luggage");
        PlayerBase.mouseFix = false;
    }

    public static ItemContainer RandomizeDropItem(ItemBase wantItem) // 성공하면 등급에 따른 랜덤한 개수의 장비를 반환 실패하면 null
    {

        ItemContainer result = new ItemContainer();
        int randNum = Random.Range(0, 100); // 0 ~ 99 사이의 숫자
        // 등급에 따라 확률, 개수 차등

        switch (wantItem.grade)
        {
            // 노말 80퍼, 1 ~ 4개
            case Rarity.Normal:
                if (randNum >= 0 && randNum < 80) // 성공
                {
                    int num = Random.Range(1, 5);
                    result.item = wantItem;
                    result.Number = num;
                }
                else
                    result = null;
                break;

            // 레어 60퍼, 1 ~ 4개
            case Rarity.Rare:
                if (randNum >= 0 && randNum < 60) // 성공
                {
                    int num = Random.Range(1, 5);
                    result.item = wantItem;
                    result.Number = num;
                }
                else
                    result = null;
                break;

            // 영웅 40퍼, 1 ~ 3개
            case Rarity.Heroic:
                if (randNum >= 0 && randNum < 40) // 성공
                {
                    int num = Random.Range(10, 40);
                    result.item = wantItem;
                    result.Number = num;
                }
                else
                    result = null;
                break;

            // 전설 20퍼, 1 ~ 2개
            case Rarity.Legend:
                if (randNum >= 0 && randNum < 20) // 성공
                {
                    int num = Random.Range(1, 3);
                    result.item = wantItem;
                    result.Number = num;
                }
                else
                    result = null;
                break;

            default:
                result = null;
                break;
        };


        return result;
    }

    public void SetItemList(List<string> dropList)
    {
        foreach(var current in dropList)
        {
            ItemBase wantItem = ItemBase.Search(current);           // 이름에 대응하는 아이템 받기
            ItemContainer container = RandomizeDropItem(wantItem);  // 랜덤으로 돌리기
            if (container != null)                                  // 성공했다면 이 상자가 가지고있은 컨테이너 리스트에 등록
            {
                itemList.Add(container.item, container.Number);
            };
        };
    }

    private void OnEnable()
    {
        StartCoroutine("AutoDisable");
    }

    private IEnumerator AutoDisable()
    {
        yield return new WaitForSeconds(30f);
        gameObject.SetActive(false);
    }
}
