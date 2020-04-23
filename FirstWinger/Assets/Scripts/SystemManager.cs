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
    Player player;

    public Player Hero
    {
        get
        {
            if(!player)
            {
                Debug.LogError("Main Player is not setted!");
            }

            return player;
        }
    }

    GamePointAccumulator gamePointAccumulator = new GamePointAccumulator();

    public GamePointAccumulator GamePointAccumulator
    {
        get
        {
            return gamePointAccumulator;
        }
    }

    [SerializeField]
    EffectManager effectManager;

    public EffectManager EffectManager
    {
        get
        {
            return effectManager;
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
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
