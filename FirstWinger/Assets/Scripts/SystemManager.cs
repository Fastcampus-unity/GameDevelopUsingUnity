using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SystemManager : MonoBehaviour
{
    /// <summary>
    /// 싱글톤 인스턴스
    /// </summary>
    static SystemManager instance = null;

    public static SystemManager Instance
    {
        get
        {
            return instance;
        }
    }
    //

    [SerializeField]
    EnemyTable enemyTable;

    public EnemyTable EnemyTable
    {
        get
        {
            return enemyTable;
        }
    }

    [SerializeField]
    ItemTable itemTable;

    public ItemTable ItemTable
    {
        get
        {
            return itemTable;
        }
    }

    [SerializeField]
    ItemDropTable itemDropTable;

    public ItemDropTable ItemDropTable
    {
        get
        {
            return itemDropTable;
        }
    }

    BaseSceneMain currentSceneMain;

    public BaseSceneMain CurrentSceneMain
    {
        set
        {
            currentSceneMain = value;
        }
    }

    [SerializeField]
    NetworkConnectionInfo connectionInfo = new NetworkConnectionInfo();

    public NetworkConnectionInfo ConnectionInfo
    {
        get
        {
            return connectionInfo;
        }
    }

    void Awake()
    {
        // 유일하게 존재할 수 있도록 에러 처리
        if(instance != null)
        {
            Debug.LogError("SystemManager is initialized twice!");
            Destroy(gameObject);
            return;
        }

        instance = this;

        // Scene 이동간에 사라지지 않도록 처리
        DontDestroyOnLoad(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {

        BaseSceneMain baseSceneMain = GameObject.FindObjectOfType<BaseSceneMain>();
        SystemManager.Instance.CurrentSceneMain = baseSceneMain;

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public T GetCurrentSceneMain<T>()
         where T : BaseSceneMain
    {
        return currentSceneMain as T;
    }
}
