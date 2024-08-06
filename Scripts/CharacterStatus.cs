using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStatus : MonoBehaviour
{
    public CharacterBase owner { get; set; }   // �� �������ͽ��� ������ �ִ� ĳ����

    [SerializeField]
    public List<InventoryBase> inven = new List<InventoryBase>(); // ĳ���Ͱ� ���ϴ� �κ��丮 ����Ʈ

    [SerializeField, Tooltip("ĳ������ ���� ���� ǥ��")]
    protected int _level;
    public int Level 
    { 
        get => _level; 
        set => _level = value; 
    }

    // ----------------------------------------------------------------------------
    [Header("�����")]

    [SerializeField, Tooltip("ĳ������ ���� �����")]
    protected float _healthCurrent;
    public float HealthCurrent
    {
        get => _healthCurrent;
        set => _healthCurrent = Mathf.Min(value, _healthMax);
    }

    [SerializeField, Tooltip("ĳ������ �ִ� �����")]
    protected float _healthMax;
    public float HealthMax
    {
        get => _healthMax;
        set
        {
            _healthMax = Mathf.Max(0, value);                       // �ִ� ������� 0���� ���� �� ����
            _healthCurrent = Mathf.Min(_healthCurrent, HealthMax);  // �ִ� ������� �پ��� ���� ������� �����ϵ���
        }
    }

    [Tooltip("ĳ������ ���� ����� ����")]
    public float healthRate { get => _healthCurrent / _healthMax; }

    // ----------------------------------------------------------------------------
    [Header("����ġ")]

    [SerializeField, Tooltip("ĳ������ ���� ����ġ")]
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

    [SerializeField, Tooltip("ĳ������ �ִ� ����ġ")]
    protected float _expMax;
    public float ExpMax // ������ ���� �ִ� ����ġ�� ����
    {
        get => _expMax; 
        set
        {
            _expMax *= (Level * expMultiplier);
        }
    }

    [Tooltip("ĳ������ ���� ����� ����")]
    public float expRate { get => _expCurrent / _expMax; }

    // ----------------------------------------------------------------------------

    [Header("�̵�")]

    [SerializeField, Tooltip("ĳ���Ͱ� 1�ʴ� �̵��� �� �ִ� �ӵ��Դϴ�.")]
    protected float _moveSpeedBase;
    public float MoveSpeedBase => _moveSpeedBase;

    [SerializeField, Tooltip("ĳ���Ͱ� ������ �� ����ϴ� �Ŀ�")]
    protected float _jumpPower;
    public float JumpPower => _jumpPower;

    [SerializeField, Tooltip("ĳ���Ͱ� ������ ����ϴ� �Ŀ�")]
    protected float _dodgePower;
    public float DodgePower => _dodgePower;

    /// <summary> ���� ĳ������ �̵��ӵ� </summary>
    public float MoveSpeed => _moveSpeedBase * moveSpeedMultiplier;

    // ----------------------------------------------------------------------------
    [Header("����")]

    [SerializeField, Tooltip("ĳ������ �⺻ ���ݷ�")]
    protected float _attackDamage;
    public float AttackDamage => _attackDamage;

    [SerializeField, Tooltip("ĳ������ ���� ������")]
    protected float _attackDelay;
    public float AttackDelay => _attackDelay;

    // ----------------------------------------------------------------------------
    [Header("���� ����")]
    
    /// <summary> �̵��ӵ� ���� </summary>
    public float moveSpeedMultiplier = 1.0f;
    public float expMultiplier = 1.5f;

    #region �پ��ѻ����̻�
    /// <summary> �ൿ�Ұ� </summary>
    protected int actStack = 0;
    // ������ �� �ִٰ� �ϸ� ���ó����� ���ٸ� ���ÿø�
    public bool Actable
    {
        get { return actStack <= 0; }
        set { actStack = value ? --actStack : ++actStack; } 
    }

    /// <summary> �̵��Ұ� </summary>
    protected int moveStack = 0;
    public bool Movable
    {
        get { return moveStack <= 0; }
        set { moveStack = value ? --moveStack : ++moveStack; } 
    }

    /// <summary> ���ݺҰ� </summary>
    protected int attackStack = 0;
    public bool Attackable
    {
        get { return attackStack <= 0; }
        set { attackStack = value ? --attackStack : ++attackStack; } 
    }

    /// <summary> ����Ҵ��̳� �ٸ� �ൿ�� ������ ��� </summary>
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
        return wantAmount; // �� ������ �ֳ� Ȯ��
    }

    //-----------------------------------------------------------------------
    [Header("���� ����ġ")]
    public float dropExp; 
}
