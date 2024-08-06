using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractBase : MonoBehaviour
{
    public string interactName; // 무슨 상호작용을 하는가

    public virtual void Interact() { } // 자식에 만들 상호작용 내용
}
