using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    public CharacterBase owner { get; set; }   // 이 스테이터스를 가지고 있는 캐릭터

    [SerializeField]
    public List<InventoryBase> inven = new List<InventoryBase>(); // 캐릭터가 지니는 인벤토리 리스트

    [SerializeField, Tooltip("캐릭터의 현재 레벨 표시")]
    protected int _level;
    public int Level 
    { 
        get => _level; 
        set => _level = value; 
    }

    // ----------------------------------------------------------------------------
    [Header("생명력")]

    [SerializeField, Tooltip("캐릭터의 현재 생명력")]
    protected float _healthCurrent;
    public float HealthCurrent
    {
        get => _healthCurrent;
        set => _healthCurrent = Mathf.Min(value, _healthMax);
    }

    [SerializeField, Tooltip("캐릭터의 최대 생명력")]
    protected float _healthMax;
    public float HealthMax
    {
        get => _healthMax;
        set
        {
            _healthMax = Mathf.Max(0, value);                       // 최대 생명력은 0보다 작을 수 없다
            _healthCurrent = Mathf.Min(_healthCurrent, HealthMax);  // 최대 생명력이 줄어들면 현재 생명력을 제한하도록
        }
    }

    [Tooltip("캐릭터의 현재 생명력 비율")]
    public float healthRate { get => _healthCurrent / _healthMax; }

    // ----------------------------------------------------------------------------
    [Header("경험치")]

    [SerializeField, Tooltip("캐릭터의 현재 경험치")]
    protected float _expCurrent;
    public float ExpCurrent
    {
        get => _expCurrent;
        set
        {
            if (value >= ExpMax)
            {
                Level++;
                _expCurrent = value - ExpMax;
            }
            else 
                _expCurrent = value;
        }
    }

    [SerializeField, Tooltip("캐릭터의 최대 경험치")]
    protected float _expMax;
    public float ExpMax // 레벨에 따른 최대 경험치의 증가
    {
        get => _expMax; 
        set
        {
            _expMax *= (Level * expMultiplier);
        }
    }

    [Tooltip("캐릭터의 현재 생명력 비율")]
    public float expRate { get => _expCurrent / _expMax; }

    // ----------------------------------------------------------------------------

    [Header("이동")]

    [SerializeField, Tooltip("캐릭터가 1초당 이동할 수 있는 속도입니다.")]
    protected float _moveSpeedBase;
    public float MoveSpeedBase => _moveSpeedBase;

    [SerializeField, Tooltip("캐릭터가 점프할 때 사용하는 파워")]
    protected float _jumpPower;
    public float JumpPower => _jumpPower;

    [SerializeField, Tooltip("캐릭터가 구를때 사용하는 파워")]
    protected float _dodgePower;
    public float DodgePower => _dodgePower;

    /// <summary> 현재 캐릭터의 이동속도 </summary>
    public float MoveSpeed => _moveSpeedBase * moveSpeedMultiplier;

    // ----------------------------------------------------------------------------
    [Header("공격")]

    [SerializeField, Tooltip("캐릭터의 기본 공격력")]
    protected float _attackDamage;
    public float AttackDamage => _attackDamage;

    [SerializeField, Tooltip("캐릭터의 공격 딜레이")]
    protected float _attackDelay;
    public float AttackDelay => _attackDelay;

    // ----------------------------------------------------------------------------
    [Header("각종 배율")]
    
    /// <summary> 이동속도 배율 </summary>
    public float moveSpeedMultiplier = 1.0f;
    public float expMultiplier = 1.5f;

    #region 다양한상태이상
    /// <summary> 행동불가 </summary>
    protected int actStack = 0;
    // 움직일 수 있다고 하면 스택내리고 없다면 스택올림
    public bool Actable
    {
        get { return actStack <= 0; }
        set { actStack = value ? --actStack : ++actStack; } 
    }

    /// <summary> 이동불가 </summary>
    protected int moveStack = 0;
    public bool Movable
    {
        get { return moveStack <= 0; }
        set { moveStack = value ? --moveStack : ++moveStack; } 
    }

    /// <summary> 공격불가 </summary>
    protected int attackStack = 0;
    public bool Attackable
    {
        get { return attackStack <= 0; }
        set { attackStack = value ? --attackStack : ++attackStack; } 
    }

    /// <summary> 제어불능이나 다른 행동은 가능한 경우 </summary>
    protected int controlStack = 0;
    public bool Controllable
    {
        get { return controlStack <= 0; }
        set { controlStack = value ? --controlStack : ++controlStack; } 
    }

    #endregion

    public int AddItem(ItemBase wantItem, int wantAmount = 1)
    {
        foreach(var current in inven)
        {
            wantAmount = current.Add(wantItem, wantAmount);

        };
        return wantAmount; // 덜 먹은거 있나 확인
    }

    //-----------------------------------------------------------------------
    [Header("몬스터 경험치")]
    public float dropExp; 
}
