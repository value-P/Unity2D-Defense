using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractBase : MonoBehaviour
{
    public string interactName; // ���� ��ȣ�ۿ��� �ϴ°�

    public virtual void Interact() { } // �ڽĿ� ���� ��ȣ�ۿ� ����
}
