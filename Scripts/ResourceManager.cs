using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    protected static ResourceManager _instance;
    public static ResourceManager Instance
    {
        get
        {
            if(_instance == null)
            {
                _instance = FindObjectOfType<ResourceManager>();

                if(_instance == null)
                {
                    _instance = GameManager.Instance.gameObject.AddComponent<ResourceManager>();
                    _instance.GetResources();
                }
                else
                {
                    _instance.GetResources();
                };
            };

            return _instance;
        }
    }

    public virtual void Awake()
    {
        this.Singleton(ref _instance);
        if(_instance == this)
        {
            GetResources();
        }
    }

    public static Dictionary<string, Sprite> iconSprites = null; // GetResources를 사용해야 생긴다
    public static Dictionary<string, TextAsset> csvs = null;

    public static Sprite GetIconSprite(string wantName) // 원하는 이름의 아이콘이 있다면 반환해주고 그렇지 않다면 null
    {
        if (iconSprites.ContainsKey(wantName)) return iconSprites[wantName];

        return null;
    }
    public virtual void GetResources() // 자동으로 리소스를 받아와 리스트에 저장해준다
    {
        if(iconSprites == null)
        {
            // Sprite 전부 받기
            iconSprites = new Dictionary<string, Sprite>(); // 생성

            Sprite[] spriteResults = Resources.LoadAll<Sprite>("Icons");

            foreach(var current in spriteResults)
            {
                iconSprites.Add(current.name, current);
            };

            // CSV text 받기
            csvs = new Dictionary<string, TextAsset>();

            TextAsset[] textResults = Resources.LoadAll<TextAsset>("CSVs");

            foreach(var current in textResults)
            {
                csvs.Add(current.name, current);
            };

            // 아이템에 뭐가 있는지 알려주기
            ItemBase.InitializeList();
        }
    }


}
