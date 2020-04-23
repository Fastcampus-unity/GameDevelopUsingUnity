using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFactory : MonoBehaviour
{
    public const string EnemyPath = "Prefabs/Enemy";

    Dictionary<string, GameObject> EnemyFileCache = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public GameObject Load(string resourcePath)
    {
        GameObject go = null;

        if(EnemyFileCache.ContainsKey(resourcePath))   // 캐시 확인
        {
            go = EnemyFileCache[resourcePath];
        }
        else
        {
            // 캐시에 없으므로 로드
            go = Resources.Load<GameObject>(resourcePath);
            if(!go)
            {
                Debug.LogError("Load error! path = " + resourcePath);
                return null;
            }
            // 로드 후 캐시에 적재
            EnemyFileCache.Add(resourcePath, go);
        }

        return go;
    }

}
