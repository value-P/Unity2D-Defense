using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBase
{
    public static Dictionary<string, ItemBase> itemList = new Dictionary<string, ItemBase>();   // 문자로 검색하는 아이템 딕셔너리
    
    public static void InitializeList()
    {
        Debug.Log("이니셜라이즈");
        // csv파일에서 아이템정보 글자 빼오기
        string csvText = ResourceManager.csvs["ItemInfo"].text;

        string[] rows = csvText.Split('\n'); // 엔터로 줄을 나눔
        for(int i = 1; i < rows.Length; i++) // 0번줄은 항목명이다
        {
            string[] columns = rows[i].Split(','); // 현재 줄에서 콤마를 기준으로 나눠줌
            for(int c = 0; c < columns.Length; c++)
            {
                columns[c] = columns[c].Trim(); // 앞뒤 공백을 제거한 상태
            };


            itemList.Add 
            (
                columns[0], // 아이템 이름
                new ItemBase
                (
                    columns[0],                                 // 아이템 이름
                    ResourceManager.GetIconSprite(columns[1]),  // 아이콘 이름
                    (Rarity)System.Enum.Parse(typeof(Rarity),columns[2]), // 등급
                    (ItemType)System.Enum.Parse(typeof(ItemType),columns[3]), // 용도
                    System.Int32.Parse(columns[4]), // 최대 개수
                    columns[5], // 설명  
                    ((System.Func<PlayerBase,ItemBase, int>)(typeof(ItemBase).GetMethod(columns[6])).CreateDelegate(typeof(System.Func<PlayerBase, ItemBase, int>))),
                    //      형변환                     이 클래스에서    요이름의메서드 가져오기   델리게이트만들기         characterBase를 변수로 갖는 int가 반환값인
                    System.Int32.Parse(columns[7]), // 공격력
                    System.Int32.Parse(columns[8]), // 방어력
                    System.Int32.Parse(columns[9])  // 회복량
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

    public string name;     // 아이템 이름
    public string context;  // 아이템 설명

    public Sprite icon;     // 아이템 아이콘
    public int maxNumber;   // 슬롯 한칸에 들어갈 수 있는 최대 개수

    public Rarity grade;    // 아이템 등급
    public ItemType type;   // 아이템의 종류

    // 장비 이외에 다 0
    public int attack; // 공격력
    public int defense; // 방어력

    // 회복물약 이외에 다 0
    public int recover; // 회복력

    public int enchant = 0; // 기본 강화수치 0 : 장비만 사용

    // 생성자에서 정보 넘겨받기
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

        result += (type > ItemType._장비_시작 && type < ItemType._장비_끝) ? $" + {enchant}".Setting(Color.red, "i") + "\n" : "\n";  // 장비면 강화수치 표현

        result += $"<{grade.ToText()} / {type} >\n\n";

        result += context + "\n\n";

        if (type == ItemType.무기) result += $"공격력 : + {attack}";
        else if (type > ItemType.무기 && type < ItemType._장비_끝) result += $"방어력 : + {defense}";

        return result;
    }

    // 아이템 사용을 CSV에 등록한 함수로 받아와서 넣어줄 것이고 사용은 Use로 통합
    public System.Func<PlayerBase, ItemBase,int> Use;

    public static int NothingUse(PlayerBase target, ItemBase item) { return 0; }   // 사용효과 없는 애들

    public static int RecoveryUse(PlayerBase target, ItemBase item) // 체력회복 물약
    {
        target.stat.HealthCurrent += item.recover;

        return 1;
    }

    public static int MaxHpUpUse(PlayerBase target, ItemBase item) // 최대체력 증가
    {
        target.stat.HealthMax += item.recover;
        target.stat.HealthCurrent += item.recover;

        return 1;
    }

    public static int EquipUse(PlayerBase target, ItemBase item) // 장비
    {
        return 1;
    }
 
}
