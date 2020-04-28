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
            if (!player)
            {
                Debug.LogWarning("Main Player is not setted!");
            }

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

    [SerializeField]
    Transform playerStartTransform1;
    public Transform PlayerStartTransform1
    {
        get
        {
            return playerStartTransform1;
        }
    }

    [SerializeField]
    Transform playerStartTransform2;
    public Transform PlayerStartTransform2
    {
        get
        {
            return playerStartTransform2;
        }
    }


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

    public void GameStart()
    {
        NetworkTransfer.RpcGameStart();
    }

}
