using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Bar
{ Hp, Exp}

public class ShowPlayerBar : MonoBehaviour
{
    public PlayerBase target; // ü��, ����ġ �� �����ְ��� �ϴ� ĳ����

    public Bar wantBar; // HP���� EXP���� ����
    public Slider slider;

    int showStat; // ������ ����

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
