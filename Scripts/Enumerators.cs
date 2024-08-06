public enum MonsterType
{
    Wolf,

    Length
}

/// <summary> 이 캐릭터를 다룰 컨트롤러를 찾습니다! </summary>
public enum ControllerType
{
    /// <summary> 멍때린다! </summary>
    None,
    /// <summary> 이 컴퓨터에서 플레이하고 있는 플레이어 </summary>
    LocalPlayer,
    /// <summary> 그냥 캐릭터를 따라옴! </summary>
    AI_FollowPlayer,
}

/// <summary> 얼마나 희귀한 아이템인가요 </summary>
public enum Rarity
{
    Normal,
    Rare,
    Heroic,
    Legend
}

public enum ItemType
{
    _장비_시작 = 1000,
    무기, 방패, 투구, 상의, 하의, 장갑, 신발, 벨트,
    _장비_끝,

    _소모품_시작 = 3000,
    생명력회복,최대체력증가,공격력증가,
    _소모품_끝,

    _기타_시작 = 5000,
    기타, 
    _기타_끝,

    _퀘스트_시작 = 8000,
    퀘스트,
    _퀘스트_끝,

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

