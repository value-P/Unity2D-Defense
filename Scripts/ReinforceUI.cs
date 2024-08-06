using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReinforceUI : CloserableUIBase
{
    PlayerBase player { get => GameManager.player; } // �÷��̾� 

    [Header("���� ����")]
    protected static ItemContainer reinforceContainer = new ItemContainer(); // ���������� ���������̳�
    public SlotBase slot; // �������� ����

    [Header("���ǥ��â")]
    public string materialName; // ��� �̸�
    protected int wantMaterialNum = 0; // ���ϴ� ��� ��
    public int MaterialNum  // �÷��̾��� ��ȭ��(����̸� �־����)������ �޾ƿ�
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
    public TextMeshProUGUI materialText; // ������ �ؽ�Ʈ

    [Header("�����Ǳ�� �� ����")]
    public Image gauge;
    protected Animator anim;

    protected override void Start()
    {
        anim = GetComponent<Animator>();
        base.Start();
        slot.SetContainer(reinforceContainer); // �� �����̳ʰ� ���������� ����
    }

    private void Update()
    {
        UpdateMaterialText(); // ��� �ؽ�Ʈ ����
        
        if(reinforceContainer != null)
        {
            SetMaterialNum(reinforceContainer.item); // �ʿ��� ��� ���� ����
        }
    }

    public void OnClickReinforceBtn() // ��ȭ ��ư ������ ������ �޼���
    {
        if (MaterialNum < wantMaterialNum || reinforceContainer.item == null) return; // ��� �����ϰų� ��� ������ ���� �ȵǵ���

        anim.SetTrigger("ClickButton"); // �ִ� ���
    }

    private void SetMaterialNum(ItemBase item) // �ʿ��� ���� �������ִ� �޼���
    {
        if (reinforceContainer.item == null) return;

        int result = 0;

        if(reinforceContainer.item.type == ItemType.����) // ����� ��� ���� ��
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
        else if(reinforceContainer.item.type > ItemType.���� && reinforceContainer.item.type < ItemType._���_��)
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
    private void UpdateMaterialText() // ��� �ؽ�Ʈ ������Ʈ
    {
        if (reinforceContainer.item != null) materialText.text = MaterialNum < wantMaterialNum ?
                $"��ȭ�� {MaterialNum} / {wantMaterialNum}" : $"��ȭ�� {MaterialNum} / {wantMaterialNum}".Setting(Color.white); //��� �����ϸ� �״�� �ƴϸ� ���
        else  materialText.text = $"��ȭ�� 0 / 0";
    }

    public bool RandomizeReinforce(ItemBase item)
    {
        int result = Random.Range(0, 100);

        switch (item.enchant) // ��ȭ ��ġ�� ���� ��ȭ ����Ȯ���� �ٸ�
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
        if(RandomizeReinforce(reinforceContainer.item)) // �����ϸ� ��ȭ��ġ ����
        {
            reinforceContainer.item.enchant++;
        }
        MaterialNum -= wantMaterialNum; // ���� �پ���
    }

}
