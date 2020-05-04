using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Boss : Enemy
{
    const float FireTransformRotationStart = -30.0f;    // 발사방향 회전시 초기값
    const float FireTransformRotationInterval = 15.0f;  // 발사방향 회전시 간격
    const float ActionUpdateInterval = 1.0f;

    [SyncVar]
    bool needBattleMove = false;

    [SerializeField]
    float BattleMoveMax;

    Vector3 BattleMoveStartPos;

    int FireRemainCountPerOnetime;

    [SyncVar]
    float BattleMoveLength;

    //
    [SyncVar]
    [SerializeField]
    Vector3 CurrentFireTransformRotation;

    protected override int BulletIndex
    {
        get
        {
            return BulletManager.BossBulletIndex;
        }
    }

    /// <summary>
    /// 미사일 발사 위치
    /// </summary>
    [SerializeField]
    Transform [] MissileFireTransforms;

    Player[] players;

    Player[] Players
    {
        get
        {
            if(players == null)
                players = GameObject.FindObjectsOfType<Player>();
            return players;
        }
    }

    /// <summary>
    /// 미사일을 발사하기 위한 플래그
    /// </summary>
    bool SpecialAttack = false;

    /// <summary>
    /// 미사일 발사시 사용할 속도
    /// </summary>
    [SerializeField]
    float MissileSpeed = 1;

    protected override void SetBattleState()
    {
        base.SetBattleState();
        BattleMoveStartPos = transform.position;
        FireRemainCountPerOnetime = FireRemainCount;

        // 회전값을 초기화
        CurrentFireTransformRotation.z = FireTransformRotationStart;
        Quaternion quat = Quaternion.identity;
        quat.eulerAngles = CurrentFireTransformRotation;
        FireTransform.localRotation = quat;

    }

    protected override void UpdateBattle()
    {
        if(needBattleMove)
        {
            UpdateBattleMove();
        }
        else
        {
            if (Time.time - LastActionUpdateTime > ActionUpdateInterval)
            {
                if (FireRemainCountPerOnetime > 0)
                {
                    if(SpecialAttack)
                        FireChase();
                    else
                    {
                        Fire();
                        RotateFireTransform();
                    }

                    FireRemainCountPerOnetime--;
                }
                else
                {
                    SetBattleMove();
                }

                LastActionUpdateTime = Time.time;
            }
        }
    }

    void SetBattleMove()
    {
        if (!isServer)
            return;

        // 랜덤한 방향으로 이동을 시작하기 위한 부분
        float halfPingpongHeight = 0.0f;
        float rand = Random.value;
        if (rand < 0.5f)
            halfPingpongHeight = BattleMoveMax * 0.5f;
        else
            halfPingpongHeight = -BattleMoveMax * 0.5f;
        // 랜덤한 거리를 이동하기 위한 부분
        float newBattleMoveLength = Random.Range(BattleMoveMax, BattleMoveMax * 3.0f);  //  BattleMoveMax 의 1배~ 3배 사이의 거리를 이동

        RpcSetBattleMove(halfPingpongHeight, newBattleMoveLength);        // Host 플레이어인경우 RPC로 보낸다
    }

    [ClientRpc]
    public void RpcSetBattleMove(float halfPingpongHeight, float newBattleMoveLength)
    {
        needBattleMove = true;
        TargetPosition = BattleMoveStartPos;
        TargetPosition.y += halfPingpongHeight;

        CurrentSpeed = 0.0f;           // 사라질때는 0부터 속도 증가
        MoveStartTime = Time.time;

        BattleMoveLength = newBattleMoveLength;

        base.SetDirtyBit(1);
    }

    void UpdateBattleMove()
    {
        UpdateSpeed();

        Vector3 oldPosition = transform.position;
        float distance = Vector3.Distance(TargetPosition, transform.position);
        if (distance == 0)
        {
            if (isServer)
                RpcChangeBattleMoveTarget();        // Host 플레이어인경우 RPC로 보낸다
        }

        transform.position = Vector3.SmoothDamp(transform.position, TargetPosition, ref CurrentVelocity, distance / CurrentSpeed, MaxSpeed * 0.2f);

        BattleMoveLength -= Vector3.Distance(oldPosition, transform.position);
        if (BattleMoveLength <= 0)
            SetBattleFire();

    }

    [ClientRpc]
    public void RpcChangeBattleMoveTarget()
    {
        if(TargetPosition.y > BattleMoveStartPos.y)
            TargetPosition.y = BattleMoveStartPos.y - BattleMoveMax * 0.5f;
        else
            TargetPosition.y = BattleMoveStartPos.y + BattleMoveMax * 0.5f;

        base.SetDirtyBit(1);
    }

    void SetBattleFire()
    {
        if (isServer)
            RpcSetBattleFire();        // Host 플레이어인경우 RPC로 보낸다
    }

    [ClientRpc]
    public void RpcSetBattleFire()
    {
        needBattleMove = false;
        MoveStartTime = Time.time;
        FireRemainCountPerOnetime = FireRemainCount;
        // 회전값을 초기화
        CurrentFireTransformRotation.z = FireTransformRotationStart;
        Quaternion quat = Quaternion.identity;
        quat.eulerAngles = CurrentFireTransformRotation;
        FireTransform.localRotation = quat;
        SpecialAttack = !SpecialAttack;     // 일반 공격과 미사일 공격을 번갈아 가면서 하도록 플래그 반전

        base.SetDirtyBit(1);
    }

    void RotateFireTransform()
    {
        if (isServer)
            RpcRotateFireTransform();        // Host 플레이어인경우 RPC로 보낸다
    }

    [ClientRpc]
    public void RpcRotateFireTransform()
    {
        CurrentFireTransformRotation.z += FireTransformRotationInterval;
        Quaternion quat = Quaternion.identity;
        quat.eulerAngles = CurrentFireTransformRotation;
        FireTransform.localRotation = quat;

        base.SetDirtyBit(1);
    }

    /// <summary>
    /// 미사일 발사 메소드
    /// </summary>
    public void FireChase()
    {
        // 살아있는 플레이어만 리스트에 추린다
        List<Player> alivePlayer = new List<Player>();
        for(int i = 0; i < Players.Length; i++)
        {
            if(!Players[i].IsDead)
            {
                alivePlayer.Add(Players[i]);
            }
        }

        // 플레이어 중 랜덤한 타겟을 선택
        int index = Random.Range(0, alivePlayer.Count);
        int targetInstanceID = alivePlayer[index].ActorInstanceID;

        // 미사일을 추적모드로 발사
        Transform missileFireTransform = MissileFireTransforms[ MissileFireTransforms.Length - FireRemainCountPerOnetime ];
        GuidedMissile missile = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletManager.Generate(BulletManager.GuidedMissileIndex, missileFireTransform.position) as GuidedMissile;
        if (missile)
        {
            missile.FireChase(targetInstanceID, actorInstanceID, missileFireTransform.right, MissileSpeed, Damage);
        }
    }

    protected override void OnDead()
    {
        base.OnDead();

        if (isServer)
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().OnGameEnd(true);
    }
}
