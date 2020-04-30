using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class InGameSceneMain : BaseSceneMain
{
    public GameState CurrentGameState
    {
        get
        {
            return NetworkTransfer.CurrentGameState;
        }
    }

    [SerializeField]
    Player player;

    public Player Hero
    {
        get
        {
            return player;
        }
        set
        {
            player = value;
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

    [SerializeField]
    EnemyManager enemyManager;

    public EnemyManager EnemyManager
    {
        get
        {
            return enemyManager;
        }
    }

    [SerializeField]
    BulletManager bulletManager;
    public BulletManager BulletManager
    {
        get
        {
            return bulletManager;
        }
    }

    [SerializeField]
    DamageManager damageManager;
    public DamageManager DamageManager
    {
        get
        {
            return damageManager;
        }
    }

    [SerializeField]
    ItemBoxManager itemBoxManager;
    public ItemBoxManager ItemBoxManager
    {
        get
        {
            return itemBoxManager;
        }
    }

    PrefabCacheSystem enemyCacheSystem = new PrefabCacheSystem();
    public PrefabCacheSystem EnemyCacheSystem
    {
        get
        {
            return enemyCacheSystem;
        }
    }

    PrefabCacheSystem bulletCacheSystem = new PrefabCacheSystem();
    public PrefabCacheSystem BulletCacheSystem
    {
        get
        {
            return bulletCacheSystem;
        }
    }

    PrefabCacheSystem effectCacheSystem = new PrefabCacheSystem();
    public PrefabCacheSystem EffectCacheSystem
    {
        get
        {
            return effectCacheSystem;
        }
    }

    PrefabCacheSystem damageCacheSystem = new PrefabCacheSystem();
    public PrefabCacheSystem DamageCacheSystem
    {
        get
        {
            return damageCacheSystem;
        }
    }

    PrefabCacheSystem itemBoxCacheSystem = new PrefabCacheSystem();
    public PrefabCacheSystem ItemBoxCacheSystem
    {
        get
        {
            return itemBoxCacheSystem;
        }
    }


    [SerializeField]
    SquadronManager squadronManager;

    public SquadronManager SquadronManager
    {
        get
        {
            return squadronManager;
        }
    }


    [SerializeField]
    Transform mainBGQuadTransform;

    public Transform MainBGQuadTransform
    {
        get
        {
            return mainBGQuadTransform;
        }
    }

    [SerializeField]
    InGameNetworkTransfer inGameNetworkTransfer;


    InGameNetworkTransfer NetworkTransfer
    {
        get
        {
            return inGameNetworkTransfer;
        }
    }

    ActorManager actorManager = new ActorManager();

    public ActorManager ActorManager
    {
        get
        {
            return actorManager;
        }
    }

    [SerializeField]
    int BossEnemyID;

    [SerializeField]
    Vector3 BossGeneratePos;

    [SerializeField]
    Vector3 BossAppearPos;

    public void GameStart()
    {
        NetworkTransfer.RpcGameStart();
    }

    public void ShowWarningUI()
    {
        NetworkTransfer.RpcShowWarningUI();
    }
    

    public void SetRunningState()
    {
        NetworkTransfer.RpcSetRunningState();
    }


    public void GenerateBoss()
    {
        SquadronMemberStruct data = new SquadronMemberStruct();
        data.EnemyID = BossEnemyID;
        data.GeneratePointX = BossGeneratePos.x;
        data.GeneratePointY = BossGeneratePos.y;
        data.AppearPointX = BossAppearPos.x;
        data.AppearPointY = BossAppearPos.y;
        data.DisappearPointX = -15.0f;
        data.DisappearPointY = 0.0f;

        EnemyManager.GenerateEnemy(data);
    }
}
