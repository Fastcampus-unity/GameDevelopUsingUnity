using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class PrefabCacheData
{
    public string filePath;
    public int cacheCount;
}

public class PrefabCacheSystem
{
    Dictionary<string, Queue<GameObject>> Caches = new Dictionary<string, Queue<GameObject>>();

    public void GenerateCache(string filePath, GameObject gameObject, int cacheCount, Transform parentTransform = null)
    {
        if(Caches.ContainsKey(filePath))
        {
            Debug.LogWarning("Already cache generated! filePath = " + filePath);
            return;
        }
        else
        {
            Queue<GameObject> queue = new Queue<GameObject>();
            for(int i = 0; i < cacheCount; i++)
            {
                GameObject go = Object.Instantiate<GameObject>(gameObject, parentTransform);

                go.SetActive(false);
                queue.Enqueue(go);

                Enemy enemy = go.GetComponent<Enemy>();
                if(enemy != null)
                {
                    enemy.FilePath = filePath;
                    NetworkServer.Spawn(go);

                    //enemy.RpcSetActive(false);
                }

                Bullet bullet = go.GetComponent<Bullet>();
                if (bullet != null)
                {
                    bullet.FilePath = filePath;
                    NetworkServer.Spawn(go);
                }

                ItemBox item = go.GetComponent<ItemBox>();
                if (item != null)
                {
                    item.FilePath = filePath;
                    NetworkServer.Spawn(go);
                }

            }

            Caches.Add(filePath, queue);
        }
    }

    public GameObject Archive(string filePath, Vector3 position)
    {
        if (!Caches.ContainsKey(filePath))
        {
            Debug.LogError("Archive Errror! no Cache generated! filePath = " + filePath);
            return null;
        }

        if(Caches[filePath].Count == 0)
        {
            Debug.LogWarning("Archive problem! not enough Count");
            return null;
        }

        GameObject go = Caches[filePath].Dequeue();
        go.SetActive(true);
        go.transform.position = position;
        if (((FWNetworkManager)FWNetworkManager.singleton).isServer)
        {
            Enemy enemy = go.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.SetPosition(position);
                enemy.RpcSetActive(true);
            }

            Bullet bullet = go.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.SetPosition(position);
                bullet.RpcSetActive(true);
            }

            ItemBox item = go.GetComponent<ItemBox>();
            if (item != null)
            {
                item.RpcSetPosition(position);
                item.RpcSetActive(true);
            }
        }

        return go;
    }

    public bool Restore(string filePath, GameObject gameObject)
    {
        if(!Caches.ContainsKey(filePath))
        {
            Debug.LogError("Restore Errror! no Cache generated! filePath = " + filePath);
            return false;
        }

        gameObject.SetActive(false);
        if(((FWNetworkManager)FWNetworkManager.singleton).isServer)
        {
            Enemy enemy = gameObject.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.RpcSetActive(false);
            }

            Bullet bullet = gameObject.GetComponent<Bullet>();
            if (bullet != null)
            {
                bullet.RpcSetActive(false);
            }

            ItemBox item = gameObject.GetComponent<ItemBox>();
            if (item != null)
            {
                item.RpcSetActive(false);
            }
        }

        Caches[filePath].Enqueue(gameObject);
        return true;
    }

    public void Add(string filePath, GameObject gameObject)
    {
        Queue<GameObject> queue;
        if (Caches.ContainsKey(filePath))
        {
            queue = Caches[filePath];
        }
        else
        {
            queue = new Queue<GameObject>();
            Caches.Add(filePath, queue);
        }

        queue.Enqueue(gameObject);
    }
}
