using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Bar
{ Hp, Exp}

public class ShowPlayerBar : MonoBehaviour
{
    public PlayerBase target; // 체력, 경험치 를 보여주고자 하는 캐릭터

    public Bar wantBar; // HP인지 EXP인지 선택
    public Slider slider;

    int showStat; // 보여줄 스탯

    void Update()
    {
        switch (wantBar)
        {
            case Bar.Hp:
                slider.value = target.stat.healthRate;
                break;

            case Bar.Exp:
                slider.value = target.stat.expRate;
                break;

            default: return;
        }
    }
}
