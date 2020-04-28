﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : Actor
{
    const string PlayerHUDPath = "Prefabs/PlayerHUD";

    /// <summary>
    /// 이동할 벡터
    /// </summary>
    [SerializeField]
    [SyncVar]
    Vector3 MoveVector = Vector3.zero;

    [SerializeField]
    NetworkIdentity NetworkIdentity = null;

    /// <summary>
    /// 이동 속도
    /// </summary>
    [SerializeField]
    float Speed;

    [SerializeField]
    BoxCollider boxCollider;

    [SerializeField]
    Transform FireTransform;

    [SerializeField]
    float BulletSpeed = 1;

    InputController inputController = new InputController();

    [SerializeField]
    [SyncVar]
    bool Host = false;  // Host 플레이어인지 여부

    [SerializeField]
    Material ClientPlayerMaterial;

    protected override void Initialize()
    {
        base.Initialize();

        InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();

        if (isLocalPlayer)
            inGameSceneMain.Hero = this;

        if (isServer && isLocalPlayer)
        {
            Host = true;
            RpcSetHost();
        }

        Transform startTransform;
        if (Host)
            startTransform = inGameSceneMain.PlayerStartTransform1;
        else
        {
            startTransform = inGameSceneMain.PlayerStartTransform2;
            MeshRenderer meshRenderer = GetComponentInChildren<MeshRenderer>();
            meshRenderer.material = ClientPlayerMaterial;
        }

        SetPosition(startTransform.position);

        if(actorInstanceID != 0)
            inGameSceneMain.ActorManager.Regist(actorInstanceID, this);

        InitializePlayerHUD();
    }

    void InitializePlayerHUD()
    {
        InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
        GameObject go = Resources.Load<GameObject>(PlayerHUDPath);
        GameObject goInstance = Instantiate<GameObject>(go, inGameSceneMain.DamageManager.CanvasTransform);
        PlayerHUD playerHUD = goInstance.GetComponent<PlayerHUD>();
        playerHUD.Initialize(this);
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
        Debug.Log("OnStartClient");
    }

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Debug.Log("OnStartLocalPlayer");
    }

    protected override void UpdateActor()
    {
        UpdateInput();
        UpdateMove();
    }

    [ClientCallback]
    public void UpdateInput()
    {
        inputController.UpdateInput();
    }

    /// <summary>
    /// 이동벡터에 맞게 위치를 변경
    /// </summary>
    void UpdateMove()
    {
        if (MoveVector.sqrMagnitude == 0)
            return;

        // 정상적으로 NetworkBehaviour 인스턴스의 Update로 호출되어 실행되고 있을때
        //CmdMove(MoveVector);

        // MonoBehaviour 인스턴스의 Update로 호출되어 실행되고 있을때의 꼼수
        // 이경우 클라이언트로 접속하면 Command로 보내지지만 자기자신은 CmdMove를 실행 못함
        if (isServer)
        {
            RpcMove(MoveVector);        // Host 플레이어인경우 RPC로 보내고
        }
        else
        {
            CmdMove(MoveVector);        // Client 플레이어인경우 Cmd로 호스트로 보낸후 자신을 Self 동작
            if(isLocalPlayer)
                transform.position += AdjustMoveVector(MoveVector);
        }
    }

    [Command]
    public void CmdMove(Vector3 moveVector)
    {
        this.MoveVector = moveVector;
        transform.position += AdjustMoveVector(this.MoveVector);
        base.SetDirtyBit(1);
        this.MoveVector = Vector3.zero; // 타 플레이어가 보낸경우 Update를 통해 초기화 되지 않으므로 사용후 바로 초기화
    }

    [ClientRpc]
    public void RpcMove(Vector3 moveVector)
    {
        this.MoveVector = moveVector;
        transform.position += AdjustMoveVector(this.MoveVector);
        base.SetDirtyBit(1);
        this.MoveVector = Vector3.zero; // 타 플레이어가 보낸경우 Update를 통해 초기화 되지 않으므로 사용후 바로 초기화
    }

    /// <summary>
    /// 이동 방향에 맞게 이동벡터를 계산
    /// </summary>
    /// <param name="moveDirection"></param>
    public void ProcessInput(Vector3 moveDirection)
    {
        if (!isLocalPlayer)
            return;

        MoveVector = moveDirection * Speed * Time.deltaTime;

    }

    Vector3 AdjustMoveVector(Vector3 moveVector)
    {
        Transform mainBGQuadTransform = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().MainBGQuadTransform;

        Vector3 result = Vector3.zero;

        result = boxCollider.transform.position + boxCollider.center + moveVector;

        if (result.x - boxCollider.size.x * 0.5f < -mainBGQuadTransform.localScale.x * 0.5f)
            moveVector.x = 0;

        if (result.x + boxCollider.size.x * 0.5f > mainBGQuadTransform.localScale.x * 0.5f)
            moveVector.x = 0;

        if (result.y - boxCollider.size.y * 0.5f < -mainBGQuadTransform.localScale.y * 0.5f)
            moveVector.y = 0;

        if (result.y + boxCollider.size.y * 0.5f > mainBGQuadTransform.localScale.y * 0.5f)
            moveVector.y = 0;

        return moveVector;
    }

    private void OnTriggerEnter(Collider other)
    {
        Enemy enemy = other.GetComponentInParent<Enemy>();
        if (enemy)
        {
            if(!enemy.IsDead)
            {
                BoxCollider box = ((BoxCollider)other);
                Vector3 crashPos = enemy.transform.position + box.center;
                crashPos.x += box.size.x * 0.5f;

                enemy.OnCrash(CrashDamage, crashPos);
            }
        }
    }

    public void Fire()
    {
        if(Host)
        {
            Bullet bullet = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletManager.Generate(BulletManager.PlayerBulletIndex);
            bullet.Fire(actorInstanceID, FireTransform.position, FireTransform.right, BulletSpeed, Damage);
        }
        else
        {
            CmdFire(actorInstanceID, FireTransform.position, FireTransform.right, BulletSpeed, Damage);
        }
    }

    [Command]
    public void CmdFire(int ownerInstanceID, Vector3 firePosition, Vector3 direction, float speed, int damage)
    {
        Bullet bullet = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletManager.Generate(BulletManager.PlayerBulletIndex);
        bullet.Fire(ownerInstanceID, firePosition, direction, speed, damage);
        base.SetDirtyBit(1);
    }


    protected override void DecreaseHP(int value, Vector3 damagePos)
    {
        base.DecreaseHP(value, damagePos);

        Vector3 damagePoint = damagePos + Random.insideUnitSphere * 0.5f;
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().DamageManager.Generate(DamageManager.PlayerDamageIndex, damagePoint, value, Color.red);
    }

    protected override void OnDead()
    {
        base.OnDead();
        gameObject.SetActive(false);
    }

    [ClientRpc]
    public void RpcSetHost()
    {
        Host = true;
        base.SetDirtyBit(1);
    }

}
