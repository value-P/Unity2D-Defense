using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MonsterStateBase
{
    public abstract void Enter(MonsterBase target);
    public abstract void Execute(MonsterBase target);
    public abstract void Exit(MonsterBase target);
}
