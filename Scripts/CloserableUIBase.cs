using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloserableUIBase : MonoBehaviour
{
    public string windowName;   // â�� �̸�
    public bool isOpen;         // ó���� �������� ������

    // �� �̸��� �������� Dictionary�� ����
    public static Dictionary<string, List<CloserableUIBase>> uiDic = new Dictionary<string, List<CloserableUIBase>>();

    protected virtual void Start()
    {
        List<CloserableUIBase> targetList;

        // �� �̸��� �ִٸ� �޾Ƽ� ����
        if(uiDic.ContainsKey(windowName))
        {
            targetList = uiDic[windowName];
        }
        else // ���ٸ� ���� �߰��ϱ�
        {
            targetList = new List<CloserableUIBase>();
            uiDic.Add(windowName, targetList);
        }

        targetList.Add(this); // �ڸ� ��������� �߰����ֱ�

        if (!isOpen)
        {
            gameObject.SetActive(false); // �ȿ���� �Ǿ������� ���� ����
        }
    }

    public static void ToggleUI(string wantName) // �����ִٸ� ���� ���������� Ű��
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

    public static void OpenUI(string _windowName) // UI ����
    {
        if(uiDic.ContainsKey(_windowName))
        {
            foreach(var current in uiDic[_windowName]) // �ش� �̸��� ���� UI����Ʈ ���Ƽ� ����
            {
                current.ClaimOpen();
            }
        }
    }

    public static void CloseUI(string _windowName)
    {
        if (uiDic.ContainsKey(_windowName))
        {
            foreach (var current in uiDic[_windowName]) // �ش� �̸��� ���� UI����Ʈ ���Ƽ� ����
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
