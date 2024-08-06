using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainer
{
    public ItemBase item;
    protected int _number;
    public int      Number
    {
        get => _number;
        set
        {
            if (item == null) return;
            _number = Mathf.Min(value,item.maxNumber); // ���� ���� �ִ� �� �� �� ���� �ɷ� �޴´�

            if(_number <= 0) // ������ 0���ϰ� �Ǹ� �ڵ����� ��쵵���Ѵ�
            {
                item = null;
                _number = 0;
            };
        }
    }

    public virtual void Use(PlayerBase target) // ����� ������ ���
    {
        Number -= item.Use(target, item);
    }

    public virtual void Swap(ItemContainer other) // ������ �ڽŰ� ������� ��ȯ
    {
        ItemBase othersItem = other.item;
        other.item = item;
        item = othersItem;

        int othersNum = other.Number;
        other.Number = Number;
        Number = othersNum;
    }
}

public class InventoryBase : MonoBehaviour
{
    public static ItemContainer mouseContainer = new ItemContainer(); // ���콺�� ���� ���� �����̳�
    public static Dictionary<string, InventoryBase> inventoryDic = new Dictionary<string, InventoryBase>();
    public string inventoryName;

    public PlayerBase owner; // �� �κ��丮�� ������ �ִ� ĳ����

    public List<InventoryTab> tabs = new List<InventoryTab>(); // ������ �ǵ��� ��� ����Ʈ

    public Vector2Int[] size; // �κ��丮 �� �ϳ��� ������
    public string[] tabNames; // ���� �̸��� ���� Ŭ���� ����, ������ ���⼭

    public int tabCurrent { get; set; }  // ���� ���� �ε���

    public SlotBase[,] slots;       // ������ �����̳ʸ� �����ִ� ���Ե�
    public GameObject slotPrefab;   // ���� ������   
    public Transform slotLayout;    // ������ �������� �θ������Ʈ�� Ʈ������

    void Awake()
    {
        inventoryDic.Add(inventoryName, this);

        // �ִ� x�� y�� �غ�
        int maxSlotX = 0;
        int maxSlotY = 0;
        foreach(var current in size)
        {
            maxSlotX = Mathf.Max(maxSlotX, current.x);
            maxSlotY = Mathf.Max(maxSlotY, current.y);
        }

        slots = new SlotBase[maxSlotY, maxSlotX];
        
        for(int i = 0;i < tabNames.Length; i++)
        {
            var tabType = System.Type.GetType(tabNames[i]); // ������� �ϴ� Ŭ������ �̸� Ȯ��

            // type�������� InventoryTab�� ��ӹ޴� ���ο� �ν��Ͻ� ����, Ÿ�԰� |  �����ڸ� ���� �Ű�������
            var inst = (InventoryTab)System.Activator.CreateInstance(tabType, this, (size.Length <= 1) ? size[0] : size[i]);
            tabs.Add(inst); // ��Ͽ� �߰�
        }


        for(int y = 0; y < slots.GetLength(0); y++)
        {
            for(int x = 0; x < slots.GetLength(1); x++)
            {
                //                         ������Ʈ   �θ� Ʈ������     ���� �༮�� ���Ժ��̽� ��ũ��Ʈ
                slots[y, x] = Instantiate(slotPrefab, slotLayout).GetComponent<SlotBase>();
            };
        };

        SetTab(0);
    }

    private void Start()
    {
        // ������ �ִٸ� ������ �κ� ����Ʈ�� �ڽ� �߰�
        if (owner != null) owner.stat.inven.Add(this);
    }

    public void SetTab(int wantIndex) // ���ϴ� �ε����� ������ �������ش�
    {
        if (wantIndex < 0 || wantIndex >= tabNames.Length) return; // ���� �ε����� ������

        tabCurrent = wantIndex; // ���� �� �ε��� ����!

        for(int y = 0; y < slots.GetLength(0); y++)
        {
            for(int x = 0; x < slots.GetLength(1); x++)
            {
                slots[y, x].SetContainer(tabs[tabCurrent].Find(y, x)); // ���� �� ������ ��ȸ�� ���� ����� ������ �ҷ�����
            };
        };
    }

    public void SetTab(InventoryTab targetTab) // ���� ���� �޾ƿͼ� ������ش�(Luggage)���� ���
    {
        if (targetTab == null) return;

        for (int y = 0; y < slots.GetLength(0); y++)
        {
            for (int x = 0; x < slots.GetLength(1); x++)
            {
                slots[y, x].SetContainer(targetTab.Find(y, x)); // ���� �� ������ ��ȸ�� ���� ����� ������ �ҷ�����
            };
        };
    }

    public int Add(ItemBase wantItem, int wantAmount = 1)
    {
        foreach(var current in tabs)
        {
            wantAmount = current.Add(wantItem, wantAmount);
        };

        return wantAmount; // �� ���°� �ֳ� ��ȯ����
    }

    public void OnClick(ItemContainer target) // ��Ŭ�� ��Ŭ�� ����
    {
        if(InputManager.GetKeyState(MouseCode.LeftClick) == KeyState.On)
        {
            tabs[tabCurrent]?.OnLeftClick(target);
        }
        else if(InputManager.GetKeyState(MouseCode.RightClick) == KeyState.On)
        {
            tabs[tabCurrent]?.OnRightClick(target);
        };
    }

}

public class InventoryTab
{
    public InteractItem showingBox; // ���� �����ִ� ����(Luggage������ ���)

    public ItemContainer[,] items;  // �� �ǿ� ���� ������ ���� �迭
    protected InventoryBase from;
    public InventoryTab(InventoryBase wantFrom, Vector2Int wantSize) // �����ڷ� �����ŭ �迭 ������ֱ�
    {
        from = wantFrom;
        // ���� ������ ������ �����ʾƷ��� ���ϴ� ���

        items = new ItemContainer[wantSize.y, wantSize.x];  // �ءءءءءءء� 1���� y��, 2����x�� �ءءءءءءء�

        for(int y = 0; y < items.GetLength(0); y++)
        {
            for(int x = 0; x < items.GetLength(1); x++)
            {
                items[y, x] = new ItemContainer();
            };
        };
    }

    public virtual int Add(ItemBase wantItem, int amount = 1) // ���ϴ� �������� ���ϴ� �縸ŭ �߰�
    {
        if (amount <= 0) return 0; // 0�� ���Ϸ� ���� ���ϵ���

        ItemContainer[] findItems = FindAll(wantItem, true); // ������ ���� �� ã��

        foreach(var current in findItems) // ���� �����۵��� ���ڸ��� �־��ֱ�
        {
            int shortage = current.item.maxNumber - current.Number; // �������� �ִ��ѵ����� �������� �ʿ��� ����

            shortage = Mathf.Min(shortage, amount); // ������ �ִ� ������ �����ϰ� �ֱ�
            current.Number += shortage;

            amount -= shortage; // ������ŭ ���ֱ�

            if (amount <= 0) return 0;  // �� �־����� ������
        };

        while(amount > 0) // ������ ���Ҵٸ� ��ĭ�� �־�����
        {
            ItemContainer target = FindLeftTop(null); // �ǿ��� ���� ����ִ� ĭ�� ã�´�

            if (target == null) break; // ��ĭ������ ������

            int wantAmount = Mathf.Min(amount, wantItem.maxNumber);
            target.item = wantItem;
            target.Number += wantAmount;

            amount -= wantAmount;
        };

        return amount; // ��ä��� ���� ���� ��ȯ�ȴ�
    }
    public virtual int Remove(ItemBase wantItem, int amount = 1)  // ���ϴ� �������� ���ϴ� �縸ŭ ����
    { 
        while(amount > 0)
        {
            ItemContainer currentContainer = FindRightBottom(wantItem); // ������ �Ʒ����� �����ذ���
            if (currentContainer == null) break;

            int wantAmount = Mathf.Min(currentContainer.Number, amount);
            currentContainer.Number -= wantAmount;
            amount -= wantAmount;
        };

        return amount; // � ���������� ��ȯ
    }
    public virtual int RemoveAll(ItemBase wantItem)               // ���ϴ� ������ ���� ����
    {
        int result = 0; // ���� �� ����

        foreach(var current in items)
        {
            if(current.item == wantItem)
            {
                result += current.Number;
                current.Number = 0; // �ڵ����� ������
            };
        };
        return result; 
    }
    public virtual int CheckAmount(ItemBase wantItem)             // ���ϴ� �������� ���� Ȯ��
    {
        int result = 0;

        foreach(var current in items)
        {
            if (current.item == wantItem)
                result += current.Number;
        };
        return result;
    }
    public virtual int Split(ItemContainer target, int amount)    // ���ϴ� ������ �����̳ʿ��� ���ϴ� ��ŭ ������
    {
        if (target == null) return 0; // ��� ������ �ƹ��Ͼ�����

        int currentAmount = Mathf.Min(amount, target.Number);

        target.Number -= currentAmount;
        return currentAmount;
    }
    public virtual ItemContainer[] FindAll(ItemBase wantItem, bool exceptFull = false)  // ���ϴ� ������ ������ ���ã��, ������ ����?
    {
        List<ItemContainer> result = new List<ItemContainer>(); // ��� ���⿡ �ֱ�

        foreach(var current in items)
        {
            //   ã���� �ϴ� �������̸�         NAND �� �� ������ �����ϸ� false�� �ȴ� ex) ������ ���� && ����
            if(current.item == wantItem && !(exceptFull && current.item.maxNumber <= current.Number))
            {
                result.Add(current);
            };
        };

        return result.ToArray();
    }
    public virtual ItemContainer FindLeftTop(ItemBase wantItem)   // ���ϴ� �������� ���� ù��°�� ������ �� ã��(���������� 1��)
    { 
        for(int y = 0; y < items.GetLength(0); y++)
        {
            for(int x = 0; x < items.GetLength(1); x++)
            {
                if (items[y, x].item == wantItem) return items[y, x];
            };
        };

        return null;
    }
    public virtual ItemContainer FindRightBottom(ItemBase wantItem) // ���ϴ� �������� ���� �������� �ִ°� ã��(�����ʾƷ��� ������)
    {
        for(int y = items.GetLength(0) - 1; y >= 0; y--)
        {
            for(int x = items.GetLength(1) -1; x >= 0; x--)
            {
                if (items[y, x].item == wantItem) return items[y, x];
            };
        };

        return null;
    }
    public virtual ItemContainer Find(int wantY, int wantX)         // ���ϴ� ��ġ�� ������ ã��(y = row, x = column)
    {
        if (items.IsOutSide(wantY, wantX)) return null; // �Է��� �ε����� ������ �Ѿ�� ��������

        return items[wantY, wantX];
    }

    public virtual void OnLeftClick(ItemContainer target)   // ��Ŭ����
    {
        if(Input.GetKey(KeyCode.LeftShift) && InventoryBase.mouseContainer.item == null) // �½���Ʈ + ��콺 �����̳� ����ִٸ�
        {
            ItemBase origin = target.item;
            int amount = Split(target, 1); // 1���� ���ֱ�

            if(amount > 0)
            {
                InventoryBase.mouseContainer.item = origin;
                InventoryBase.mouseContainer.Number = amount;
                return;
            };
        }
        else if(InventoryBase.mouseContainer.item != null && InventoryBase.mouseContainer.item == target.item)
        //               ���콺�� ��� �ִ°� �ְ�                 Ŭ���� ���Կ� �ִ� �������̶� ������
        {
            int insertAmount = target.item.maxNumber - target.Number; // ������ �ѵ� ������ ä�쵵��
            insertAmount = Mathf.Min(insertAmount, InventoryBase.mouseContainer.Number);

            target.Number += insertAmount;
            InventoryBase.mouseContainer.Number -= insertAmount;

            return;
        }

        // �⺻ : ���콺�� �ִ� �Ͱ� �ش� ���� ��ü
        target.Swap(InventoryBase.mouseContainer);
    }
    public virtual void OnRightClick(ItemContainer target)  // ��Ŭ����
    { 
        if(target.item != null)
        {
            if(InventoryBase.mouseContainer.item == null && Input.GetKey(KeyCode.LeftShift)) // ���콺 ����ְ� �½���Ʈ + ��Ŭ��
            {
                if(target.Number > 1) // 1�� �̻� �ִٸ�
                {
                    int amount = Split(target, target.Number / 2); // ���ݸ�ŭ ���� �ֱ�
                    InventoryBase.mouseContainer.item = target.item;
                    InventoryBase.mouseContainer.Number = amount;
                };

                return;
            };

            target.Use(from.owner);
        }
        else if(target.item == null && InventoryBase.mouseContainer.item != null) // �տ� �鸰ä�� ��ĭ�� ��Ŭ�� �ϸ� 1���� ��������
        {
            ItemBase origin = InventoryBase.mouseContainer.item;
            int amount = Split(InventoryBase.mouseContainer, 1);
            target.item = origin;
            target.Number = amount;

            return;
        }
    }
}

public class InventoryTab_Equipment : InventoryTab
{
    public InventoryTab_Equipment(InventoryBase wantFrom, Vector2Int wantSize) : base(wantFrom, wantSize) { }

    public override int Add(ItemBase wantItem, int amount = 1)
    {
        if (wantItem.type <= ItemType._���_���� || wantItem.type >= ItemType._���_��) return amount;

        return base.Add(wantItem, amount);
    }

}

public class InventoryTab_Usable : InventoryTab
{
    public InventoryTab_Usable(InventoryBase wantFrom, Vector2Int wantSize) : base(wantFrom, wantSize) { }


    public override int Add(ItemBase wantItem, int amount = 1)
    {
        if (wantItem.type <= ItemType._�Ҹ�ǰ_���� || wantItem.type >= ItemType._�Ҹ�ǰ_��) return amount;

        return base.Add(wantItem, amount);
    }

}

public class InventoryTab_Etc : InventoryTab
{
    public InventoryTab_Etc(InventoryBase wantFrom, Vector2Int wantSize) : base(wantFrom, wantSize) { }

    public override int Add(ItemBase wantItem, int amount = 1)
    {
        if (wantItem.type <= ItemType._��Ÿ_���� || wantItem.type >= ItemType._��Ÿ_��) return amount;

        return base.Add(wantItem, amount);
    }

}

public class InventoryTab_Quest : InventoryTab
{
    public InventoryTab_Quest(InventoryBase wantFrom, Vector2Int wantSize) : base(wantFrom, wantSize) { }

    public override int Add(ItemBase wantItem, int amount = 1)
    {
        if (wantItem.type <= ItemType._����Ʈ_���� || wantItem.type >= ItemType._����Ʈ_��) return amount;

        return base.Add(wantItem, amount);
    }

}

public class InventoryTab_Luggage : InventoryTab
{
    public InventoryTab_Luggage(InventoryBase wantFrom, Vector2Int wantSize) : base(wantFrom, wantSize) { }

    public override void OnLeftClick(ItemContainer target)
    {
        // �ִµ� ���޴µ�?
        target.Number = GameManager.player.stat.AddItem(target.item, target.Number);
    }

}


