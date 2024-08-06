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
                GameObject managerObject = GameObject.Find("GameManager"); // 게임매니져 없는지 확인
                if(managerObject == null)
                {
                    managerObject = new GameObject("GameManager"); // 없으면 만들기
                };

                _instance = managerObject.AddComponent<GameManager>(); // 없으면 만들었고 있으면 비어있으니 넣어주기
            }

            return _instance;
        }
    }

    // 플에이어
    public static PlayerBase player;

    #region ////////////////////
    static int pauseClaim = 0; //정지 명령 횟수
    public static bool Pause
    {
        get => pauseClaim > 0; // 멈추라고 하는 애가 있는지 확인
        set => pauseClaim = Mathf.Max(value ? pauseClaim + 1 : pauseClaim - 1, 0);
        // 멈추라고 하면 정지명령+1, 움직이라고 하면(false) -1 단, 멈추는 것이 0보다 작으면 안되니 Max를 이용해 제한
    } // 다른 애들이 게임 멈추고싶으면 이걸 이용

    // 강제로 게임을 멈추거나 실행시키는 함수
    public static void ForcePause(bool value)
    {
        pauseClaim = value ? Mathf.Max(1, pauseClaim) : 0; // 멈추고자하면 최소1로 고정, 풀고싶으면 아무리 많아도 0으로
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
        Time.timeScale = Pause ? 0 : 1f;  // 게임을 멈춘다고 하면 시간 배율을 0으로 조절한다
    }

    public virtual void Initialize() { }
    public static void OnPlayerDead() { }    // 캐릭터 사망시 작동하기

}
