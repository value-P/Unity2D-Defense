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
                iconImage.sprite = targetContainer.item.icon; //�̹��� ������
                numberText.text = targetContainer.item.enchant.ToString(); // ��ȭ��ġ ������
            }
            else // �������� ���� ���
            {
                numberText.text = "";
            };
            // ��� �������� �ִ� ��� Ȱ��ȭ
            iconImage.enabled = (targetContainer.item != null);
        };

    }

    public override void OnClick()
    {
        ItemContainer container = InventoryBase.mouseContainer;

        if (InputManager.GetKeyState(MouseCode.LeftClick) == KeyState.On)
        {
            // ��Ŭ�� ������ ���콺�����̳ʿ� �������� ������̶�� ��ȯ
            if(container.item != null && container.item.type > ItemType._���_���� && container.item.type < ItemType._���_��)
            {
                targetContainer.Swap(container);
            }
            else if(container.item == null && targetContainer.item != null) // ���콺�����̳� ����ְ� ��ȭ���Կ� �ִٸ� ��ü
            {
                targetContainer.Swap(container);
            };
        }
        else if(InputManager.GetKeyState(MouseCode.RightClick) == KeyState.On)
        {
            if(targetContainer != null)
            {
                int num = InventoryBase.inventoryDic["Inventory"].Add(targetContainer.item, targetContainer.Number); // ��Ŭ���� �κ��丮�� �־��ְ�
                targetContainer.Number -= (1 - num); // �ڸ� ��� �����޴°� �ƴϸ� ���������
            };
        };
    }
}
