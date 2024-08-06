using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractItem : InteractBase
{
    public InventoryTab itemList = new InventoryTab_Luggage(null, new Vector2Int(5,5));
    public InventoryBase Inven => InventoryBase.inventoryDic["Luggage"];

    public override void Interact()
    {
        if (Inven) Inven.SetTab(itemList); // UI����

        CloserableUIBase.OpenUI("Luggage");
        PlayerBase.mouseFix = false;
    }

    public static ItemContainer RandomizeDropItem(ItemBase wantItem) // �����ϸ� ��޿� ���� ������ ������ ��� ��ȯ �����ϸ� null
    {

        ItemContainer result = new ItemContainer();
        int randNum = Random.Range(0, 100); // 0 ~ 99 ������ ����
        // ��޿� ���� Ȯ��, ���� ����

        switch (wantItem.grade)
        {
            // �븻 80��, 1 ~ 4��
            case Rarity.Normal:
                if (randNum >= 0 && randNum < 80) // ����
                {
                    int num = Random.Range(1, 5);
                    result.item = wantItem;
                    result.Number = num;
                }
                else
                    result = null;
                break;

            // ���� 60��, 1 ~ 4��
            case Rarity.Rare:
                if (randNum >= 0 && randNum < 60) // ����
                {
                    int num = Random.Range(1, 5);
                    result.item = wantItem;
                    result.Number = num;
                }
                else
                    result = null;
                break;

            // ���� 40��, 1 ~ 3��
            case Rarity.Heroic:
                if (randNum >= 0 && randNum < 40) // ����
                {
                    int num = Random.Range(10, 40);
                    result.item = wantItem;
                    result.Number = num;
                }
                else
                    result = null;
                break;

            // ���� 20��, 1 ~ 2��
            case Rarity.Legend:
                if (randNum >= 0 && randNum < 20) // ����
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
            ItemBase wantItem = ItemBase.Search(current);           // �̸��� �����ϴ� ������ �ޱ�
            ItemContainer container = RandomizeDropItem(wantItem);  // �������� ������
            if (container != null)                                  // �����ߴٸ� �� ���ڰ� ���������� �����̳� ����Ʈ�� ���
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
