using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBoxManager : MonoBehaviour
{
    [SerializeField]
    PrefabCacheData[] ItemBoxFiles;

    Dictionary<string, GameObject> FileCache = new Dictionary<string, GameObject>();

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject Generate(int index, Vector3 position)
    {
        if (!((FWNetworkManager)FWNetworkManager.singleton).isServer)
            return null;

        if (index < 0 || index >= ItemBoxFiles.Length)
        {
            Debug.LogError("Generate error! out of range! index = " + index);
            return null;
        }

        string filePath = ItemBoxFiles[index].filePath;
        GameObject go = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ItemBoxCacheSystem.Archive(filePath, position);

        ItemBox item = go.GetComponent<ItemBox>();
        item.FilePath = filePath;

        return go;
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
        if (!((FWNetworkManager)FWNetworkManager.singleton).isServer)
            return;

        for (int i = 0; i < ItemBoxFiles.Length; i++)
        {
            GameObject go = Load(ItemBoxFiles[i].filePath);
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ItemBoxCacheSystem.GenerateCache(ItemBoxFiles[i].filePath, go, ItemBoxFiles[i].cacheCount, this.transform);
        }
    }

    public bool Remove(ItemBox item)
    {
        if (!((FWNetworkManager)FWNetworkManager.singleton).isServer)
            return true;

        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ItemBoxCacheSystem.Restore(item.FilePath, item.gameObject);
        return true;
    }

}
