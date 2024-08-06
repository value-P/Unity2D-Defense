using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerStateBase
{
    public abstract void Enter(PlayerBase target);
    public abstract void Execute(PlayerBase target);
    public abstract void Exit(PlayerBase target);

}
