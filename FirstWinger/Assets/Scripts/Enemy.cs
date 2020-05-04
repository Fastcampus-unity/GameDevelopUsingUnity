using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Enemy : Actor
{
    public enum State : int
    {
        None = -1,  // 사용전
        Ready = 0,  // 준비 완료
        Appear,     // 등장
        Battle,     // 전투중
        Dead,       // 사망
        Disappear,  // 퇴장
    }

    /// <summary>
    /// 현재 상태값
    /// </summary>
    [SerializeField]
    [SyncVar]
    State CurrentState = State.None;

    /// <summary>
    /// 최고 속도
    /// </summary>
    protected const float MaxSpeed = 10.0f;

    /// <summary>
    /// 최고 속도에 이르는 시간
    /// </summary>
    const float MaxSpeedTime = 0.5f;


    /// <summary>
    /// 목표점
    /// </summary>
    [SerializeField]
    [SyncVar]
    protected Vector3 TargetPosition;

    [SerializeField]
    [SyncVar]
    protected float CurrentSpeed;

    /// <summary>
    /// 방향을 고려한 속도 벡터
    /// </summary>
    [SyncVar]
    protected Vector3 CurrentVelocity;

    [SyncVar] 
    protected float MoveStartTime = 0.0f; // 이동시작 시간

    [SerializeField]
    protected Transform FireTransform;

    [SerializeField]
    [SyncVar]
    float BulletSpeed = 1;

    [SyncVar]
    protected float LastActionUpdateTime = 0.0f;

    [SerializeField]
    [SyncVar]
    protected int FireRemainCount = 1;

    [SerializeField]
    [SyncVar]
    int GamePoint = 10;

    [SyncVar]
    [SerializeField]
    string filePath;

    public string FilePath
    {
        get
        {
            return filePath;
        }
        set
        {
            filePath = value;
        }
    }

    [SyncVar]
    Vector3 AppearPoint;      // 입장시 도착 위치
    [SyncVar]
    Vector3 DisappearPoint;      // 퇴장시 목표 위치
    
    [SerializeField]
    [SyncVar]
    float ItemDropRate;     // 아이템 생성 확률

    [SerializeField]
    [SyncVar]
    int ItemDropID;         // 아이템 생성시 참조할 ItemDrop 테이블의 인덱스

    protected virtual int BulletIndex
    {
        get
        {
            return BulletManager.EnemyBulletIndex;
        }
    }

    protected override void Initialize()
    {
        base.Initialize();
        
        InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
        if(!((FWNetworkManager)FWNetworkManager.singleton).isServer)
        {
            transform.SetParent(inGameSceneMain.EnemyManager.transform);
            inGameSceneMain.EnemyCacheSystem.Add(FilePath, gameObject);
            gameObject.SetActive(false);
        }

        if(actorInstanceID != 0)
            inGameSceneMain.ActorManager.Regist(actorInstanceID, this);
    }

    // Update is called once per frame
    protected override void UpdateActor()
    {
        //
        switch (CurrentState)
        {
            case State.None:
                break;
            case State.Ready:
                UpdateReady();
                break;
            case State.Dead:
                break;
            case State.Appear:
            case State.Disappear:
                UpdateSpeed();
                UpdateMove();
                break;
            case State.Battle:
                UpdateBattle();
                break;
            default:
                Debug.LogError("Undefined State!");
                break;
        }
    }

    protected void UpdateSpeed()
    {
        // CurrentSpeed 에서 MaxSpeed 에 도달하는 비율을 흐른 시간많큼 계산
        CurrentSpeed = Mathf.Lerp(CurrentSpeed, MaxSpeed, (Time.time - MoveStartTime) / MaxSpeedTime);
    }

    void UpdateMove()
    {
        float distance = Vector3.Distance(TargetPosition, transform.position);
        if(distance == 0)
        {
            Arrived();
            return;
        }

        // 이동벡터 계산. 양 벡터의 차를 통해 이동벡터를 구한후 nomalized 로 단위벡터를 구한다. 속도를 곱해 현재 이동할 벡터를 계산
        CurrentVelocity = (TargetPosition - transform.position).normalized * CurrentSpeed;

        // 자연스러운 감속으로 목표지점에 도착할 수 있도록 계산
        // 속도 = 거리 / 시간 이므로 시간 = 거리/속도
        transform.position = Vector3.SmoothDamp(transform.position, TargetPosition, ref CurrentVelocity, distance / CurrentSpeed, MaxSpeed);
    }

    void Arrived()
    {
        CurrentSpeed = 0.0f;    // 도착했으므로 속도는 0
        if (CurrentState == State.Appear)
        {
            SetBattleState();
        }
        else // if (CurrentState == State.Disappear)
        {
            CurrentState = State.None;
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyManager.RemoveEnemy(this);
        }
    }

    protected virtual void SetBattleState()
    {
        CurrentState = State.Battle;
        LastActionUpdateTime = Time.time;
    }

    public void Reset(SquadronMemberStruct data)
    {
        // 정상적으로 NetworkBehaviour 인스턴스의 Update로 호출되어 실행되고 있을때
        //CmdReset(data);

        // MonoBehaviour 인스턴스의 Update로 호출되어 실행되고 있을때의 꼼수
        if (isServer)
        {
            RpcReset(data);        // Host 플레이어인경우 RPC로 보내고
        }
        else
        {
            CmdReset(data);        // Client 플레이어인경우 Cmd로 호스트로 보낸후 자신을 Self 동작
            if (isLocalPlayer)
                ResetData(data);
        }
    }

    void ResetData(SquadronMemberStruct data)
    {
        EnemyStruct enemyStruct = SystemManager.Instance.EnemyTable.GetEnemy(data.EnemyID);

        CurrentHP = MaxHP = enemyStruct.MaxHP;             // CurrentHP까지 다시 입력
        Damage = enemyStruct.Damage;                       // 총알 데미지
        crashDamage = enemyStruct.CrashDamage;             // 충돌 데미지
        BulletSpeed = enemyStruct.BulletSpeed;             // 총알 속도
        FireRemainCount = enemyStruct.FireRemainCount;     // 발사할 총알 갯수
        GamePoint = enemyStruct.GamePoint;                 // 파괴시 얻을 점수

        AppearPoint = new Vector3(data.AppearPointX, data.AppearPointY, 0);             // 입장시 도착 위치 
        DisappearPoint = new Vector3(data.DisappearPointX, data.DisappearPointY, 0);    // 퇴장시 목표 위치

        ItemDropRate = enemyStruct.ItemDropRate;    // 아이템 생성 확률
        ItemDropID = enemyStruct.ItemDropID;        // 아이템 Drop 테이블 참조 인덱스

        CurrentState = State.Ready;
        LastActionUpdateTime = Time.time;
        //
        isDead = false;      // Enemy는 재사용되므로 초기화시켜줘야 함
    }

    public void Appear(Vector3 targetPos)
    {
        TargetPosition = targetPos;
        CurrentSpeed = MaxSpeed;    // 나타날때는 최고 스피드로 설정

        CurrentState = State.Appear;
        MoveStartTime = Time.time;
    }

    void Disappear(Vector3 targetPos)
    {
        TargetPosition = targetPos;
        CurrentSpeed = 0.0f;           // 사라질때는 0부터 속도 증가

        CurrentState = State.Disappear;
        MoveStartTime = Time.time;
    }

    void UpdateReady()
    {
        if (Time.time - LastActionUpdateTime > 1.0f)
        {
            Appear(AppearPoint);
        }
    }

    protected virtual void UpdateBattle()
    {
        if(Time.time - LastActionUpdateTime > 1.0f)
        {
            if (FireRemainCount > 0)
            {
                Fire();
                FireRemainCount--;
            }
            else
            {
                Disappear(DisappearPoint);
            }

            LastActionUpdateTime = Time.time;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player)
        {
            if (!player.IsDead)
            {
                BoxCollider box = ((BoxCollider)other);
                Vector3 crashPos = player.transform.position + box.center;
                crashPos.x += box.size.x * 0.5f;

                player.OnCrash(CrashDamage, crashPos);
            }
        }
    }

    public void Fire()
    {
        Bullet bullet = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletManager.Generate(BulletIndex, FireTransform.position);
        if(bullet)
            bullet.Fire(actorInstanceID, -FireTransform.right, BulletSpeed, Damage);
    }

    protected override void OnDead()
    {
        base.OnDead();

        InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
        inGameSceneMain.GamePointAccumulator.Accumulate(GamePoint);
        inGameSceneMain.EnemyManager.RemoveEnemy(this);

        GenerateItem();

        CurrentState = State.Dead;
    }

    protected override void DecreaseHP(int value, Vector3 damagePos)
    {
        base.DecreaseHP(value, damagePos);

        Vector3 damagePoint = damagePos + Random.insideUnitSphere * 0.5f;
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().DamageManager.Generate(DamageManager.EnemyDamageIndex, damagePoint, value, Color.magenta);
    }

    [Command]
    public void CmdReset(SquadronMemberStruct data)
    {
        ResetData(data);
        base.SetDirtyBit(1);
    }

    [ClientRpc]
    public void RpcReset(SquadronMemberStruct data)
    {
        ResetData(data);
        base.SetDirtyBit(1);
    }

    void GenerateItem()
    {
        if (!isServer)
            return;

        // 아이템 생성 확율을 검사
        float ItemGen = Random.Range(0.0f, 100.0f);
        if (ItemGen > ItemDropRate)
            return;

        ItemDropTable itemDropTable = SystemManager.Instance.ItemDropTable;
        ItemDropStruct dropStruct = itemDropTable.GetDropData(ItemDropID);

        // 어느 아이템을 생성할 것인지 확율 검사
        ItemGen = Random.Range(0, dropStruct.Rate1 + dropStruct.Rate2 + dropStruct.Rate3);
        int ItemIndex = -1;

        if (ItemGen <= dropStruct.Rate1)     // 1번 아이템 비율보다 작은 경우
            ItemIndex = dropStruct.ItemID1;
        else if(ItemGen <= (dropStruct.Rate1 + dropStruct.Rate2))   // 2번 아이템 비율보다 작은 경우
            ItemIndex = dropStruct.ItemID2;
        else //if (ItemGen <= (dropStruct.Rate1 + dropStruct.Rate2 + dropStruct.Rate3)) // 3번 아이템 비율인 경우
            ItemIndex = dropStruct.ItemID3;

        Debug.Log("GenerateItem ItemIndex = " + ItemIndex);

        InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
        inGameSceneMain.ItemBoxManager.Generate(ItemIndex, transform.position);
    }
    //
    public void AddList()
    {
        if (isServer)
            RpcAddList();        // Host 플레이어인경우 RPC로 보내고
    }

    [ClientRpc]
    public void RpcAddList()
    {
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyManager.AddList(this);
        base.SetDirtyBit(1);
    }

    public void RemoveList()
    {
        if (isServer)
            RpcRemoveList();        // Host 플레이어인경우 RPC로 보내고
    }

    [ClientRpc]
    public void RpcRemoveList()
    {
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyManager.RemoveList(this);
        base.SetDirtyBit(1);
    }

}

