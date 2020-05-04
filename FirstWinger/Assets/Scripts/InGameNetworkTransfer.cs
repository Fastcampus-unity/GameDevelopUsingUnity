using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public enum GameState : int
{
    None = 0,
    Ready,
    Running,
    NoInput,
    End,
}

[System.Serializable]
public class InGameNetworkTransfer : NetworkBehaviour
{
    /// <summary>
    /// 게임을 시작하기전 대기시간
    /// </summary>
    const float GameReadyIntaval = 0.5f;

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
        CountingStartTime = Time.time;
        currentGameState = GameState.Ready;

        InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
        inGameSceneMain.EnemyManager.Prepare();
        inGameSceneMain.BulletManager.Prepare();
        inGameSceneMain.ItemBoxManager.Prepare();

    }

    [ClientRpc]
    public void RpcShowWarningUI()
    {
        PanelManager.GetPanel(typeof(WarningPanel)).Show();
        currentGameState = GameState.NoInput;
    }

    [ClientRpc]
    public void RpcSetRunningState()
    {
        currentGameState = GameState.Running;
    }

    [ClientRpc]
    public void RpcGameEnd(bool success)
    {
        // 게임을 종료상태로 만들어 입력을 막는다
        currentGameState = GameState.End;
        GameEndPanel gameEndPanel = PanelManager.GetPanel(typeof(GameEndPanel)) as GameEndPanel;
        gameEndPanel.ShowGameEnd(success);
    }

    public void SetGameStateEnd()
    {
        currentGameState = GameState.End;
    }
}
