using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloserableUIBase : MonoBehaviour
{
    public string windowName;   // 창의 이름
    public bool isOpen;         // 처음에 켜져있을 것인지

    // 이 이름이 누구인지 Dictionary로 관리
    public static Dictionary<string, List<CloserableUIBase>> uiDic = new Dictionary<string, List<CloserableUIBase>>();

    protected virtual void Start()
    {
        List<CloserableUIBase> targetList;

        // 내 이름이 있다면 받아서 오고
        if(uiDic.ContainsKey(windowName))
        {
            targetList = uiDic[windowName];
        }
        else // 없다면 만들어서 추가하기
        {
            targetList = new List<CloserableUIBase>();
            uiDic.Add(windowName, targetList);
        }

        targetList.Add(this); // 자리 만들었으니 추가해주기

        if (!isOpen)
        {
            gameObject.SetActive(false); // 안열기로 되어있으면 끄고 시작
        }
    }

    public static void ToggleUI(string wantName) // 켜져있다면 끄고 꺼져있으면 키기
    {
        if(uiDic.ContainsKey(wantName))
        {
            foreach(var current in uiDic[wantName])
            {
                if(current.gameObject.activeInHierarchy)
                {
                    current.ClaimClose();
                }
                else
                {
                    current.ClaimOpen();
                };
            };
        };
    }

    public static void OpenUI(string _windowName) // UI 열기
    {
        if(uiDic.ContainsKey(_windowName))
        {
            foreach(var current in uiDic[_windowName]) // 해당 이름을 가진 UI리스트 돌아서 열기
            {
                current.ClaimOpen();
            }
        }
    }

    public static void CloseUI(string _windowName)
    {
        if (uiDic.ContainsKey(_windowName))
        {
            foreach (var current in uiDic[_windowName]) // 해당 이름을 가진 UI리스트 돌아서 열기
            {
                current.ClaimClose();
            }
        }
    }

    public void ClaimOpen()
    {
        gameObject.SetActive(true);
    }

    public void ClaimClose()
    {
        gameObject.SetActive(false);
    }

}
