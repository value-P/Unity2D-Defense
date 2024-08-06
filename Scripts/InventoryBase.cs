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
            _number = Mathf.Min(value,item.maxNumber); // 들어온 값과 최대 값 중 더 작은 걸로 받는다

            if(_number <= 0) // 개수가 0이하가 되면 자동으로 비우도록한다
            {
                item = null;
                _number = 0;
            };
        }
    }

    public virtual void Use(PlayerBase target) // 대상이 아이템 사용
    {
        Number -= item.Use(target, item);
    }

    public virtual void Swap(ItemContainer other) // 실행한 자신과 대상정보 교환
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
    public static ItemContainer mouseContainer = new ItemContainer(); // 마우스에 있을 고정 컨테이너
    public static Dictionary<string, InventoryBase> inventoryDic = new Dictionary<string, InventoryBase>();
    public string inventoryName;

    public PlayerBase owner; // 이 인벤토리를 가지고 있는 캐릭터

    public List<InventoryTab> tabs = new List<InventoryTab>(); // 아이템 탭들을 담는 리스트

    public Vector2Int[] size; // 인벤토리 탭 하나의 사이즈
    public string[] tabNames; // 탭의 이름을 통해 클래스 생성, 개수도 여기서

    public int tabCurrent { get; set; }  // 현재 탭의 인덱스

    public SlotBase[,] slots;       // 아이템 컨테이너를 보여주는 슬롯들
    public GameObject slotPrefab;   // 슬롯 프리팹   
    public Transform slotLayout;    // 슬롯을 생성해줄 부모오브젝트의 트랜스폼

    void Awake()
    {
        inventoryDic.Add(inventoryName, this);

        // 최대 x와 y를 준비
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
            var tabType = System.Type.GetType(tabNames[i]); // 만들고자 하는 클래스의 이름 확인

            // type기준으로 InventoryTab을 상속받는 새로운 인스턴스 생성, 타입과 |  생성자를 위한 매개변수들
            var inst = (InventoryTab)System.Activator.CreateInstance(tabType, this, (size.Length <= 1) ? size[0] : size[i]);
            tabs.Add(inst); // 목록에 추가
        }


        for(int y = 0; y < slots.GetLength(0); y++)
        {
            for(int x = 0; x < slots.GetLength(1); x++)
            {
                //                         오브젝트   부모 트랜스폼     만든 녀석의 슬롯베이스 스크립트
                slots[y, x] = Instantiate(slotPrefab, slotLayout).GetComponent<SlotBase>();
            };
        };

        SetTab(0);
    }

    private void Start()
    {
        // 주인이 있다면 주인의 인벤 리스트에 자신 추가
        if (owner != null) owner.stat.inven.Add(this);
    }

    public void SetTab(int wantIndex) // 원하는 인덱스의 탭으로 변경해준다
    {
        if (wantIndex < 0 || wantIndex >= tabNames.Length) return; // 없는 인덱스면 나가기

        tabCurrent = wantIndex; // 현재 탭 인덱스 변경!

        for(int y = 0; y < slots.GetLength(0); y++)
        {
            for(int x = 0; x < slots.GetLength(1); x++)
            {
                slots[y, x].SetContainer(tabs[tabCurrent].Find(y, x)); // 현재 탭 정보를 조회해 같은 장소의 정보를 불러오기
            };
        };
    }

    public void SetTab(InventoryTab targetTab) // 탭을 직접 받아와서 등록해준다(Luggage)에서 사용
    {
        if (targetTab == null) return;

        for (int y = 0; y < slots.GetLength(0); y++)
        {
            for (int x = 0; x < slots.GetLength(1); x++)
            {
                slots[y, x].SetContainer(targetTab.Find(y, x)); // 현재 탭 정보를 조회해 같은 장소의 정보를 불러오기
            };
        };
    }

    public int Add(ItemBase wantItem, int wantAmount = 1)
    {
        foreach(var current in tabs)
        {
            wantAmount = current.Add(wantItem, wantAmount);
        };

        return wantAmount; // 덜 얻어온거 있나 반환해줌
    }

    public void OnClick(ItemContainer target) // 좌클릭 우클릭 통합
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
    public InteractItem showingBox; // 현재 보고있는 상자(Luggage에서만 사용)

    public ItemContainer[,] items;  // 이 탭에 있을 아이템 정보 배열
    protected InventoryBase from;
    public InventoryTab(InventoryBase wantFrom, Vector2Int wantSize) // 생성자로 사이즈만큼 배열 만들어주기
    {
        from = wantFrom;
        // 왼쪽 위부터 시작해 오른쪽아래로 향하는 모양

        items = new ItemContainer[wantSize.y, wantSize.x];  // ※※※※※※※※ 1차원 y축, 2차원x축 ※※※※※※※※

        for(int y = 0; y < items.GetLength(0); y++)
        {
            for(int x = 0; x < items.GetLength(1); x++)
            {
                items[y, x] = new ItemContainer();
            };
        };
    }

    public virtual int Add(ItemBase wantItem, int amount = 1) // 원하는 아이템을 원하는 양만큼 추가
    {
        if (amount <= 0) return 0; // 0개 이하로 넣지 못하도록

        ItemContainer[] findItems = FindAll(wantItem, true); // 꽉찬거 빼고 다 찾기

        foreach(var current in findItems) // 같은 아이템들의 빈자리에 넣어주기
        {
            int shortage = current.item.maxNumber - current.Number; // 아이템이 최대한도까지 꽉찰려면 필요한 개수

            shortage = Mathf.Min(shortage, amount); // 넣을수 있는 양으로 조정하고 넣기
            current.Number += shortage;

            amount -= shortage; // 넣은만큼 빼주기

            if (amount <= 0) return 0;  // 다 넣었으면 나가기
        };

        while(amount > 0) // 아직도 남았다면 빈칸에 넣어주자
        {
            ItemContainer target = FindLeftTop(null); // 맨왼쪽 위에 비어있는 칸을 찾는다

            if (target == null) break; // 빈칸없으면 나가기

            int wantAmount = Mathf.Min(amount, wantItem.maxNumber);
            target.item = wantItem;
            target.Number += wantAmount;

            amount -= wantAmount;
        };

        return amount; // 못채우고 남은 수가 반환된다
    }
    public virtual int Remove(ItemBase wantItem, int amount = 1)  // 원하는 아이템을 원하는 양만큼 제거
    { 
        while(amount > 0)
        {
            ItemContainer currentContainer = FindRightBottom(wantItem); // 오른쪽 아래부터 삭제해간다
            if (currentContainer == null) break;

            int wantAmount = Mathf.Min(currentContainer.Number, amount);
            currentContainer.Number -= wantAmount;
            amount -= wantAmount;
        };

        return amount; // 몇개 못지웠는지 반환
    }
    public virtual int RemoveAll(ItemBase wantItem)               // 원하는 아이템 전부 제거
    {
        int result = 0; // 지운 총 개수

        foreach(var current in items)
        {
            if(current.item == wantItem)
            {
                result += current.Number;
                current.Number = 0; // 자동으로 지워줌
            };
        };
        return result; 
    }
    public virtual int CheckAmount(ItemBase wantItem)             // 원하는 아이템의 개수 확인
    {
        int result = 0;

        foreach(var current in items)
        {
            if (current.item == wantItem)
                result += current.Number;
        };
        return result;
    }
    public virtual int Split(ItemContainer target, int amount)    // 원하는 아이템 컨테이너에서 원하는 만큼 나누기
    {
        if (target == null) return 0; // 빈거 눌러도 아무일없도록

        int currentAmount = Mathf.Min(amount, target.Number);

        target.Number -= currentAmount;
        return currentAmount;
    }
    public virtual ItemContainer[] FindAll(ItemBase wantItem, bool exceptFull = false)  // 원하는 종류의 아이템 모두찾기, 꽉찬거 빼고?
    {
        List<ItemContainer> result = new List<ItemContainer>(); // 결과 여기에 넣기

        foreach(var current in items)
        {
            //   찾고자 하는 아이템이며         NAND 즉 두 조건이 부합하면 false가 된다 ex) 꽉찬거 제외 && 꽉참
            if(current.item == wantItem && !(exceptFull && current.item.maxNumber <= current.Number))
            {
                result.Add(current);
            };
        };

        return result.ToArray();
    }
    public virtual ItemContainer FindLeftTop(ItemBase wantItem)   // 원하는 아이템중 가장 첫번째로 놓여진 거 찾기(왼쪽위끝이 1번)
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
    public virtual ItemContainer FindRightBottom(ItemBase wantItem) // 원하는 아이템중 가장 마지막에 있는거 찾기(오른쪽아래끝 마지막)
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
    public virtual ItemContainer Find(int wantY, int wantX)         // 원하는 위치의 아이템 찾기(y = row, x = column)
    {
        if (items.IsOutSide(wantY, wantX)) return null; // 입력한 인덱스가 범위를 넘어서면 나가도록

        return items[wantY, wantX];
    }

    public virtual void OnLeftClick(ItemContainer target)   // 좌클릭시
    {
        if(Input.GetKey(KeyCode.LeftShift) && InventoryBase.mouseContainer.item == null) // 좌쉬프트 + 비우스 컨테이너 비어있다면
        {
            ItemBase origin = target.item;
            int amount = Split(target, 1); // 1개만 빼주기

            if(amount > 0)
            {
                InventoryBase.mouseContainer.item = origin;
                InventoryBase.mouseContainer.Number = amount;
                return;
            };
        }
        else if(InventoryBase.mouseContainer.item != null && InventoryBase.mouseContainer.item == target.item)
        //               마우스에 들고 있는게 있고                 클릭한 슬롯에 있는 아이템이랑 같으면
        {
            int insertAmount = target.item.maxNumber - target.Number; // 부족한 한도 내에서 채우도록
            insertAmount = Mathf.Min(insertAmount, InventoryBase.mouseContainer.Number);

            target.Number += insertAmount;
            InventoryBase.mouseContainer.Number -= insertAmount;

            return;
        }

        // 기본 : 마우스에 있는 것과 해당 슬롯 교체
        target.Swap(InventoryBase.mouseContainer);
    }
    public virtual void OnRightClick(ItemContainer target)  // 우클릭시
    { 
        if(target.item != null)
        {
            if(InventoryBase.mouseContainer.item == null && Input.GetKey(KeyCode.LeftShift)) // 마우스 비어있고 좌쉬프트 + 우클릭
            {
                if(target.Number > 1) // 1개 이상 있다면
                {
                    int amount = Split(target, target.Number / 2); // 절반만큼 떼서 넣기
                    InventoryBase.mouseContainer.item = target.item;
                    InventoryBase.mouseContainer.Number = amount;
                };

                return;
            };

            target.Use(from.owner);
        }
        else if(target.item == null && InventoryBase.mouseContainer.item != null) // 손에 들린채로 빈칸에 우클릭 하면 1개씩 내려놓기
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
        if (wantItem.type <= ItemType._장비_시작 || wantItem.type >= ItemType._장비_끝) return amount;

        return base.Add(wantItem, amount);
    }

}

public class InventoryTab_Usable : InventoryTab
{
    public InventoryTab_Usable(InventoryBase wantFrom, Vector2Int wantSize) : base(wantFrom, wantSize) { }


    public override int Add(ItemBase wantItem, int amount = 1)
    {
        if (wantItem.type <= ItemType._소모품_시작 || wantItem.type >= ItemType._소모품_끝) return amount;

        return base.Add(wantItem, amount);
    }

}

public class InventoryTab_Etc : InventoryTab
{
    public InventoryTab_Etc(InventoryBase wantFrom, Vector2Int wantSize) : base(wantFrom, wantSize) { }

    public override int Add(ItemBase wantItem, int amount = 1)
    {
        if (wantItem.type <= ItemType._기타_시작 || wantItem.type >= ItemType._기타_끝) return amount;

        return base.Add(wantItem, amount);
    }

}

public class InventoryTab_Quest : InventoryTab
{
    public InventoryTab_Quest(InventoryBase wantFrom, Vector2Int wantSize) : base(wantFrom, wantSize) { }

    public override int Add(ItemBase wantItem, int amount = 1)
    {
        if (wantItem.type <= ItemType._퀘스트_시작 || wantItem.type >= ItemType._퀘스트_끝) return amount;

        return base.Add(wantItem, amount);
    }

}

public class InventoryTab_Luggage : InventoryTab
{
    public InventoryTab_Luggage(InventoryBase wantFrom, Vector2Int wantSize) : base(wantFrom, wantSize) { }

    public override void OnLeftClick(ItemContainer target)
    {
        // 주는데 덜받는데?
        target.Number = GameManager.player.stat.AddItem(target.item, target.Number);
    }

}


