using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase
{
    public static Dictionary<string, ItemBase> itemList = new Dictionary<string, ItemBase>();   // ���ڷ� �˻��ϴ� ������ ��ųʸ�
    
    public static void InitializeList()
    {
        Debug.Log("�̴ϼȶ�����");
        // csv���Ͽ��� ���������� ���� ������
        string csvText = ResourceManager.csvs["ItemInfo"].text;

        string[] rows = csvText.Split('\n'); // ���ͷ� ���� ����
        for(int i = 1; i < rows.Length; i++) // 0������ �׸���̴�
        {
            string[] columns = rows[i].Split(','); // ���� �ٿ��� �޸��� �������� ������
            for(int c = 0; c < columns.Length; c++)
            {
                columns[c] = columns[c].Trim(); // �յ� ������ ������ ����
            };


            itemList.Add 
            (
                columns[0], // ������ �̸�
                new ItemBase
                (
                    columns[0],                                 // ������ �̸�
                    ResourceManager.GetIconSprite(columns[1]),  // ������ �̸�
                    (Rarity)System.Enum.Parse(typeof(Rarity),columns[2]), // ���
                    (ItemType)System.Enum.Parse(typeof(ItemType),columns[3]), // �뵵
                    System.Int32.Parse(columns[4]), // �ִ� ����
                    columns[5], // ����  
                    ((System.Func<PlayerBase,ItemBase, int>)(typeof(ItemBase).GetMethod(columns[6])).CreateDelegate(typeof(System.Func<PlayerBase, ItemBase, int>))),
                    //      ����ȯ                     �� Ŭ��������    ���̸��Ǹ޼��� ��������   ��������Ʈ�����         characterBase�� ������ ���� int�� ��ȯ����
                    System.Int32.Parse(columns[7]), // ���ݷ�
                    System.Int32.Parse(columns[8]), // ����
                    System.Int32.Parse(columns[9])  // ȸ����
                )
            );
        };
        
    }

    public static ItemBase Search(string wantName)
    {
        if (itemList.ContainsKey(wantName))
        {
            return itemList[wantName];
        }
        return null;
    }

    public string name;     // ������ �̸�
    public string context;  // ������ ����

    public Sprite icon;     // ������ ������
    public int maxNumber;   // ���� ��ĭ�� �� �� �ִ� �ִ� ����

    public Rarity grade;    // ������ ���
    public ItemType type;   // �������� ����

    // ��� �̿ܿ� �� 0
    public int attack; // ���ݷ�
    public int defense; // ����

    // ȸ������ �̿ܿ� �� 0
    public int recover; // ȸ����

    public int enchant = 0; // �⺻ ��ȭ��ġ 0 : ��� ���

    // �����ڿ��� ���� �Ѱܹޱ�
    public ItemBase(string _name, Sprite _icon, Rarity _grade, ItemType _type, int _maxNum, string _context 
                    ,System.Func<PlayerBase,ItemBase,int> wantUse, int _attack, int _defense, int _recover)
    {
        name = _name;
        icon = _icon;
        grade = _grade;
        type = _type;
        maxNumber = _maxNum;
        context = _context;
        Use = wantUse;
        attack = _attack;
        defense = _defense;
        recover = _recover;
    }

    public virtual string GetContext()
    {
        string result = $"[{name.Setting(Color.white, "b")}]";

        result += (type > ItemType._���_���� && type < ItemType._���_��) ? $" + {enchant}".Setting(Color.red, "i") + "\n" : "\n";  // ���� ��ȭ��ġ ǥ��

        result += $"<{grade.ToText()} / {type} >\n\n";

        result += context + "\n\n";

        if (type == ItemType.����) result += $"���ݷ� : + {attack}";
        else if (type > ItemType.���� && type < ItemType._���_��) result += $"���� : + {defense}";

        return result;
    }

    // ������ ����� CSV�� ����� �Լ��� �޾ƿͼ� �־��� ���̰� ����� Use�� ����
    public System.Func<PlayerBase, ItemBase,int> Use;

    public static int NothingUse(PlayerBase target, ItemBase item) { return 0; }   // ���ȿ�� ���� �ֵ�

    public static int RecoveryUse(PlayerBase target, ItemBase item) // ü��ȸ�� ����
    {
        target.stat.HealthCurrent += item.recover;

        return 1;
    }

    public static int MaxHpUpUse(PlayerBase target, ItemBase item) // �ִ�ü�� ����
    {
        target.stat.HealthMax += item.recover;
        target.stat.HealthCurrent += item.recover;

        return 1;
    }

    public static int EquipUse(PlayerBase target, ItemBase item) // ���
    {
        return 1;
    }
 
}
