using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageManager : MonoBehaviour
{
    public const int EnemyDamageIndex = 0;
    public const int PlayerDamageIndex = 0;

    [SerializeField]
    Transform canvasTransform;

    public Transform CanvasTransform
    {
        get
        {
            return canvasTransform;
        }
    }

    [SerializeField]
    Canvas canvas;

    [SerializeField]
    PrefabCacheData[] Files;

    Dictionary<string, GameObject> FileCache = new Dictionary<string, GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        Prepare();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject Load(string resourcePath)
    {
        GameObject go = null;

        if (FileCache.ContainsKey(resourcePath))   // 캐시 확인
        {
            go = FileCache[resourcePath];
        }
        else
        {
            // 캐시에 없으므로 로드
            go = Resources.Load<GameObject>(resourcePath);
            if (!go)
            {
                Debug.LogError("Load error! path = " + resourcePath);
                return null;
            }
            // 로드 후 캐시에 적재
            FileCache.Add(resourcePath, go);
        }

        return go;
    }

    public void Prepare()
    {
        for (int i = 0; i < Files.Length; i++)
        {
            GameObject go = Load(Files[i].filePath);
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().DamageCacheSystem.GenerateCache(Files[i].filePath, go, Files[i].cacheCount, canvasTransform);
        }
    }

    public GameObject Generate(int index, Vector3 position, int damageValue, Color textColor)
    {
        if (index < 0 || index >= Files.Length)
        {
            Debug.LogError("Generate error! out of range! index = " + index);
            return null;
        }

        string filePath = Files[index].filePath;
        GameObject go = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().DamageCacheSystem.Archive(filePath, Camera.main.WorldToScreenPoint(position));

        UIDamage damage = go.GetComponent<UIDamage>();
        damage.FilePath = filePath;
        damage.ShowDamage(damageValue, textColor);

        return go;
    }

    public bool Remove(UIDamage damage)
    {
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().DamageCacheSystem.Restore(damage.FilePath, damage.gameObject);
        return true;
    }
}
