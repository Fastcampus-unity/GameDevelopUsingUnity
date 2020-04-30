using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    EnemyFactory enemyFactory;

    [SerializeField]
    List<Enemy> enemies = new List<Enemy>();

    public List<Enemy> Enemies
    {
        get
        {
            return enemies;
        }
    }

    [SerializeField]
    PrefabCacheData [] enemyFiles;

    // Start is called before the first frame update
    void Start()
    {
        //Prepare();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public bool GenerateEnemy(SquadronMemberStruct data)
    {
        if (!((FWNetworkManager)FWNetworkManager.singleton).isServer)
            return true;

        string FilePath = SystemManager.Instance.EnemyTable.GetEnemy(data.EnemyID).FilePath;
        GameObject go = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyCacheSystem.Archive(FilePath, new Vector3(data.GeneratePointX, data.GeneratePointY, 0));

        Enemy enemy = go.GetComponent<Enemy>();
        enemy.Reset(data);
        enemy.AddList();
        return true;
    }

    public bool RemoveEnemy(Enemy enemy)
    {
        if (!((FWNetworkManager)FWNetworkManager.singleton).isServer)
            return true;

        if (!enemies.Contains(enemy))
        {
            Debug.LogError("No exist Enemy");
            return false;
        }

        enemy.RemoveList();
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyCacheSystem.Restore(enemy.FilePath, enemy.gameObject);

        return true;
    }

    public void Prepare()
    {
        if (!((FWNetworkManager)FWNetworkManager.singleton).isServer)
            return;

        for (int i = 0; i < enemyFiles.Length; i++)
        {
            GameObject go = enemyFactory.Load(enemyFiles[i].filePath);
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyCacheSystem.GenerateCache(enemyFiles[i].filePath, go, enemyFiles[i].cacheCount, this.transform);
        }
    }

    public bool AddList(Enemy enemy)
    {
        if (enemies.Contains(enemy))
            return false;

        enemies.Add(enemy);
        return true;
    }

    public bool RemoveList(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
            return false;

        enemies.Remove(enemy);
        return true;
    }

    public List<Enemy> GetContainEnemies(Collider collider)
    {
        List<Enemy> contains = new List<Enemy>();

        Collider enemyCollider;
        for (int i = 0; i < enemies.Count; i++)
        {
            enemyCollider = enemies[i].GetComponentInChildren<Collider>();
            if(enemyCollider == null)
            {
                Debug.LogError(enemies[i] + name + " model is not correct!");
                continue;
            }
            
            if(collider.bounds.Intersects(enemyCollider.bounds))
                contains.Add(enemies[i]);
        }

        return contains;
    }

    public int GetEnemyListCount()
    {
        return enemies.Count;
    }
}
