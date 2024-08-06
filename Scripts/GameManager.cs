using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance = null;
    public static GameManager Instance
    {
        get
        {
            if(_instance == null)
            {
                GameObject managerObject = GameObject.Find("GameManager"); // ���ӸŴ��� ������ Ȯ��
                if(managerObject == null)
                {
                    managerObject = new GameObject("GameManager"); // ������ �����
                };

                _instance = managerObject.AddComponent<GameManager>(); // ������ ������� ������ ��������� �־��ֱ�
            }

            return _instance;
        }
    }

    // �ÿ��̾�
    public static PlayerBase player;

    #region ////////////////////
    static int pauseClaim = 0; //���� ��� Ƚ��
    public static bool Pause
    {
        get => pauseClaim > 0; // ���߶�� �ϴ� �ְ� �ִ��� Ȯ��
        set => pauseClaim = Mathf.Max(value ? pauseClaim + 1 : pauseClaim - 1, 0);
        // ���߶�� �ϸ� �������+1, �����̶�� �ϸ�(false) -1 ��, ���ߴ� ���� 0���� ������ �ȵǴ� Max�� �̿��� ����
    } // �ٸ� �ֵ��� ���� ���߰������ �̰� �̿�

    // ������ ������ ���߰ų� �����Ű�� �Լ�
    public static void ForcePause(bool value)
    {
        pauseClaim = value ? Mathf.Max(1, pauseClaim) : 0; // ���߰����ϸ� �ּ�1�� ����, Ǯ������� �ƹ��� ���Ƶ� 0����
    }

    #endregion ////////////////////
    private void Awake()
    {
        player = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerBase>();
    }

    void OnEnable()
    {
        this.Singleton(ref _instance);
        Initialize();
    }

    public virtual void Update()
    {
        Time.timeScale = Pause ? 0 : 1f;  // ������ ����ٰ� �ϸ� �ð� ������ 0���� �����Ѵ�
    }

    public virtual void Initialize() { }
    public static void OnPlayerDead() { }    // ĳ���� ����� �۵��ϱ�

}
