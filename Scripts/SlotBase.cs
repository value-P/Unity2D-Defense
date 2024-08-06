using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SlotBase : MonoBehaviour
{
    public Image iconImage;                 // ���Կ� ���� �������̹���
    public TextMeshProUGUI numberText;      // ������ ���� �ؽ�Ʈ
    public InventoryBase parentInventory;   // �θ� �Ǵ� �κ��丮

    protected ItemContainer targetContainer;      // �� ������ ������ �����������̳�

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
                iconImage.sprite = targetContainer.item.icon; //�̹��� ������
                numberText.text = targetContainer.Number.ToString(); // �� ������
            }
            else // �������� ���� ���
            {
                numberText.text = "";
            };
            // ��� �������� �ִ� ��� Ȱ��ȭ
            iconImage.enabled = (targetContainer.item != null);
        };
    }

    public void SetContainer(ItemContainer target) // ������ ������ �����̳� ����
    {
        targetContainer = target;
    }

    public virtual void OnClick()
    {
        parentInventory?.OnClick(targetContainer);
    }

    public virtual void MouseOver() //���콺�� ���� ���� �ö󰡸� 
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
