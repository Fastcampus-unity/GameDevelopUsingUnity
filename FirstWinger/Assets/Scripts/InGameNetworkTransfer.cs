using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public enum GameState : int
{
    None = 0,
    Ready,
    Running,
    End,
}

[System.Serializable]
public class InGameNetworkTransfer : NetworkBehaviour
{
    const float GameReadyIntaval = 3.0f;

    [SyncVar]
    GameState currentGameState = GameState.None;
    public GameState CurrentGameState
    {
        get
        {
            return currentGameState;
        }
    }

    [SyncVar]
    float CountingStartTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float currentTime = Time.time;

        if (currentGameState == GameState.Ready)
        {
            if (currentTime - CountingStartTime > GameReadyIntaval)
            {
                SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().SquadronManager.StartGame();
                currentGameState = GameState.Running;
            }
        }
    }

    [ClientRpc]
    public void RpcGameStart()
    {

        Debug.Log("RpcGameStart");
        CountingStartTime = Time.time;
        currentGameState = GameState.Ready;

        InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
        inGameSceneMain.EnemyManager.Prepare();
        inGameSceneMain.BulletManager.Prepare();

    }

}
