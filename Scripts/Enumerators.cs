public enum MonsterType
{
    Wolf,

    Length
}

/// <summary> �� ĳ���͸� �ٷ� ��Ʈ�ѷ��� ã���ϴ�! </summary>
public enum ControllerType
{
    /// <summary> �۶�����! </summary>
    None,
    /// <summary> �� ��ǻ�Ϳ��� �÷����ϰ� �ִ� �÷��̾� </summary>
    LocalPlayer,
    /// <summary> �׳� ĳ���͸� �����! </summary>
    AI_FollowPlayer,
}

/// <summary> �󸶳� ����� �������ΰ��� </summary>
public enum Rarity
{
    Normal,
    Rare,
    Heroic,
    Legend
}

public enum ItemType
{
    _���_���� = 1000,
    ����, ����, ����, ����, ����, �尩, �Ź�, ��Ʈ,
    _���_��,

    _�Ҹ�ǰ_���� = 3000,
    �����ȸ��,�ִ�ü������,���ݷ�����,
    _�Ҹ�ǰ_��,

    _��Ÿ_���� = 5000,
    ��Ÿ, 
    _��Ÿ_��,

    _����Ʈ_���� = 8000,
    ����Ʈ,
    _����Ʈ_��,

}

public enum MonsterState
{
    Patrol,
    Chase,
    Combat,
    Length
}

public enum PlayerState
{
    Idle,
    Walk,
    Run,
    Attack,
    Jump,
    Dodge,
    Hit,

    Length,
}

