using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractText : MonoBehaviour
{
    public TextMeshProUGUI showingText; // ������ tmpPro
    
    private void Update()
    {
        if (PlayerBase.interactFocus) // �÷��̾ �����ִ� ��ȣ�ۿ� ����� �ִٸ�
        {
            showingText.text = $"EŰ�� ���� {PlayerBase.interactFocus.interactName}";
        }
        else
        {
            showingText.text = "";
        }
    }
}
