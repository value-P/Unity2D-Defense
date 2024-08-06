using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractText : MonoBehaviour
{
    public TextMeshProUGUI showingText; // 적용할 tmpPro
    
    private void Update()
    {
        if (PlayerBase.interactFocus) // 플레이어가 보고있는 상호작용 대상이 있다면
        {
            showingText.text = $"E키를 눌러 {PlayerBase.interactFocus.interactName}";
        }
        else
        {
            showingText.text = "";
        }
    }
}
