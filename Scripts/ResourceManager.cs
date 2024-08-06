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

    public static Dictionary<string, Sprite> iconSprites = null; // GetResources�� ����ؾ� �����
    public static Dictionary<string, TextAsset> csvs = null;

    public static Sprite GetIconSprite(string wantName) // ���ϴ� �̸��� �������� �ִٸ� ��ȯ���ְ� �׷��� �ʴٸ� null
    {
        if (iconSprites.ContainsKey(wantName)) return iconSprites[wantName];

        return null;
    }
    public virtual void GetResources() // �ڵ����� ���ҽ��� �޾ƿ� ����Ʈ�� �������ش�
    {
        if(iconSprites == null)
        {
            // Sprite ���� �ޱ�
            iconSprites = new Dictionary<string, Sprite>(); // ����

            Sprite[] spriteResults = Resources.LoadAll<Sprite>("Icons");

            foreach(var current in spriteResults)
            {
                iconSprites.Add(current.name, current);
            };

            // CSV text �ޱ�
            csvs = new Dictionary<string, TextAsset>();

            TextAsset[] textResults = Resources.LoadAll<TextAsset>("CSVs");

            foreach(var current in textResults)
            {
                csvs.Add(current.name, current);
            };

            // �����ۿ� ���� �ִ��� �˷��ֱ�
            ItemBase.InitializeList();
        }
    }


}
