using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReinforceUI : CloserableUIBase
{
    PlayerBase player { get => GameManager.player; } // 플레이어 

    [Header("슬롯 관련")]
    protected static ItemContainer reinforceContainer = new ItemContainer(); // 내부적으로 정보컨테이너
    public SlotBase slot; // 보여지는 슬롯

    [Header("재료표시창")]
    public string materialName; // 재료 이름
    protected int wantMaterialNum = 0; // 원하는 재료 양
    public int MaterialNum  // 플레이어의 강화석(재료이름 넣어야함)개수를 받아옴
    { 
        get
        {
            if (ItemBase.Search(materialName) != null)
                return InventoryBase.inventoryDic["Inventory"].tabs[2].CheckAmount(ItemBase.Search(materialName));
            else
                return 0;
        }
        set
        {
            if (ItemBase.Search(materialName) != null)
            {
                InventoryBase.inventoryDic["Inventory"].tabs[2].Remove(ItemBase.Search(materialName), MaterialNum - value);

            }
        }
    }
    public TextMeshProUGUI materialText; // 보여줄 텍스트

    [Header("장인의기운 바 관련")]
    public Image gauge;
    protected Animator anim;

    protected override void Start()
    {
        anim = GetComponent<Animator>();
        base.Start();
        slot.SetContainer(reinforceContainer); // 내 컨테이너가 보여지도록 설정
    }

    private void Update()
    {
        UpdateMaterialText(); // 재료 텍스트 갱신
        
        if(reinforceContainer != null)
        {
            SetMaterialNum(reinforceContainer.item); // 필요한 재료 개수 갱신
        }
    }

    public void OnClickReinforceBtn() // 강화 버튼 누를때 실행할 메서드
    {
        if (MaterialNum < wantMaterialNum || reinforceContainer.item == null) return; // 재료 부족하거나 장비 없으면 실행 안되도록

        anim.SetTrigger("ClickButton"); // 애니 재생
    }

    private void SetMaterialNum(ItemBase item) // 필요한 재료수 가져와주는 메서드
    {
        if (reinforceContainer.item == null) return;

        int result = 0;

        if(reinforceContainer.item.type == ItemType.무기) // 무기는 재료 많이 들어감
        {
            switch (item.grade)
            {
                case Rarity.Normal: result = 2;
                    break;
                case Rarity.Rare: result = 3;
                    break;
                case Rarity.Heroic: result = 4;
                    break;
                case Rarity.Legend: result = 5;
                    break;
                default: return;
            }
        }
        else if(reinforceContainer.item.type > ItemType.무기 && reinforceContainer.item.type < ItemType._장비_끝)
        {
            switch (item.grade)
            {
                case Rarity.Normal: result = 1;
                    break;
                case Rarity.Rare: result = 2;
                    break;
                case Rarity.Heroic: result = 3;
                    break;
                case Rarity.Legend: result = 4;
                    break;
                default: return;
            }
        }

        wantMaterialNum = result;
    }
    private void UpdateMaterialText() // 재료 텍스트 업데이트
    {
        if (reinforceContainer.item != null) materialText.text = MaterialNum < wantMaterialNum ?
                $"강화석 {MaterialNum} / {wantMaterialNum}" : $"강화석 {MaterialNum} / {wantMaterialNum}".Setting(Color.white); //재료 부족하면 그대로 아니면 흰색
        else  materialText.text = $"강화석 0 / 0";
    }

    public bool RandomizeReinforce(ItemBase item)
    {
        int result = Random.Range(0, 100);

        switch (item.enchant) // 강화 수치에 따른 강화 성공확률이 다름
        {
            case 0: return result < 100 ? true : false;
            case 1: return result < 80 ? true : false;
            case 2: return result < 60 ? true : false;
            case 3: return result < 40 ? true : false;
            case 4: return result < 20 ? true : false;
            case 5: return result < 10 ? true : false;
            default: return result < 5 ? true : false;
        }
    }

    public void OnExitGuage()
    {
        if(RandomizeReinforce(reinforceContainer.item)) // 성공하면 강화수치 오름
        {
            reinforceContainer.item.enchant++;
        }
        MaterialNum -= wantMaterialNum; // 개수 줄어들게
    }

}
