using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public class Bullet : NetworkBehaviour
{
    const float LifeTime = 15.0f;    // 총알의 생존 시간

    [SyncVar]
    [SerializeField]
    protected Vector3 MoveDirection = Vector3.zero;

    [SyncVar]
    [SerializeField]
    protected float Speed = 0.0f;

    [SyncVar]
    protected bool NeedMove = false; // 이동플래그

    [SyncVar]
    protected float FiredTime;

    [SyncVar]
    bool Hited = false; // 부딛혔는지 플래그

    [SyncVar]
    [SerializeField]
    protected int Damage = 1;


    [SyncVar]
    [SerializeField]
    int OwnerInstanceID;


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

    // Start is called before the first frame update
    void Start()
    {
        if (!((FWNetworkManager)FWNetworkManager.singleton).isServer)
        {
            InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
            transform.SetParent(inGameSceneMain.BulletManager.transform);
            inGameSceneMain.BulletCacheSystem.Add(FilePath, gameObject);
            gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ProcessDisappearCondition())
            return;

        UpdateTransform();
    }

    protected virtual void UpdateTransform()
    {
        UpdateMove();
    }

    protected virtual void UpdateMove()
    {
        if (!NeedMove)
            return;


        Vector3 moveVector = MoveDirection.normalized * Speed * Time.deltaTime;
        moveVector = AdjustMove(moveVector);
        transform.position += moveVector;

    }

    void InternelFire(int ownerInstanceID, Vector3 direction, float speed, int damage)
    {
        OwnerInstanceID = ownerInstanceID;
        MoveDirection = direction;
        Speed = speed;
        Damage = damage;

        NeedMove = true;
        FiredTime = Time.time;
    }

    public virtual void Fire(int ownerInstanceID, Vector3 direction, float speed, int damage)
    {
        // 정상적으로 NetworkBehaviour 인스턴스의 Update로 호출되어 실행되고 있을때
        //CmdFire(ownerInstanceID, direction, speed, damage);

        // MonoBehaviour 인스턴스의 Update로 호출되어 실행되고 있을때의 꼼수
        if (isServer)
        {
            RpcFire(ownerInstanceID, direction, speed, damage);        // Host 플레이어인경우 RPC로 보내고
        }
        else
        {
            CmdFire(ownerInstanceID,  direction, speed, damage);        // Client 플레이어인경우 Cmd로 호스트로 보낸후 자신을 Self 동작
            if (isLocalPlayer)
                InternelFire(ownerInstanceID, direction, speed, damage);
        }
    }

    [Command]
    public void CmdFire(int ownerInstanceID, Vector3 direction, float speed, int damage)
    {
        InternelFire(ownerInstanceID, direction, speed, damage);
        base.SetDirtyBit(1);
    }

    [ClientRpc]
    public void RpcFire(int ownerInstanceID, Vector3 direction, float speed, int damage)
    {
        InternelFire(ownerInstanceID, direction, speed, damage);
        base.SetDirtyBit(1);
    }

    protected Vector3 AdjustMove(Vector3 moveVector)
    {
        // 레이캐스트 힛 초기화
        RaycastHit hitInfo;

        if (Physics.Linecast(transform.position, transform.position + moveVector, out hitInfo))
        {
            int colliderLayer = hitInfo.collider.gameObject.layer;
            if (colliderLayer != LayerMask.NameToLayer("Enemy") && colliderLayer != LayerMask.NameToLayer("Player"))
                return moveVector;

            Actor actor = hitInfo.collider.GetComponentInParent<Actor>();
            if (actor && actor.IsDead)
                return moveVector;

            moveVector = hitInfo.point - transform.position;
            OnBulletCollision(hitInfo.collider);
        }
        return moveVector;
    }

    protected virtual bool OnBulletCollision(Collider collider)
    {
        if (Hited)
            return false;

        if (collider.gameObject.layer == LayerMask.NameToLayer("EnemyBullet")
            || collider.gameObject.layer == LayerMask.NameToLayer("PlayerBullet"))
        {
            return false;
        }

        Actor owner = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ActorManager.GetActor(OwnerInstanceID);
        if (owner == null)  // 호스트나 클라이언트중 한쪽이 끊어졌을때 발생할 수 있음
            return false;

        Actor actor = collider.GetComponentInParent<Actor>();
        if (actor == null)
            return false;

        if (actor.IsDead || actor.gameObject.layer == owner.gameObject.layer)
            return false;

        actor.OnBulletHited(Damage, transform.position);

        //Collider myCollider = GetComponentInChildren<Collider>();
        //myCollider.enabled = false;

        Hited = true;
        NeedMove = false;
        //
        GameObject go = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EffectManager.GenerateEffect(EffectManager.BulletDisappearFxIndex, transform.position);
        go.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        Disappear();

        return true;
    }

    private void OnTriggerEnter(Collider other)
    {
        int colliderLayer = other.gameObject.layer;
        if (colliderLayer != LayerMask.NameToLayer("Enemy") && colliderLayer != LayerMask.NameToLayer("Player"))
            return;

        OnBulletCollision(other);
    }

    bool ProcessDisappearCondition()
    {
        if (transform.position.x > 15.0f || transform.position.x < -15.0f
            || transform.position.y > 15.0f || transform.position.y < -15.0f)
        {
            Disappear();
            return true;
        }
        else if (Time.time - FiredTime > LifeTime)
        {
            Disappear();
            return true;
        }

        return false;
    }

    protected void Disappear()
    {
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().BulletManager.Remove(this);
    }

    [ClientRpc]
    public void RpcSetActive(bool value)
    {
        this.gameObject.SetActive(value);
        base.SetDirtyBit(1);
    }

    public void SetPosition(Vector3 position)
    {
        // 정상적으로 NetworkBehaviour 인스턴스의 Update로 호출되어 실행되고 있을때
        //CmdSetPosition(position);

        // MonoBehaviour 인스턴스의 Update로 호출되어 실행되고 있을때의 꼼수
        if (isServer)
        {
            RpcSetPosition(position);        // Host 플레이어인경우 RPC로 보내고
        }
        else
        {
            CmdSetPosition(position);        // Client 플레이어인경우 Cmd로 호스트로 보낸후 자신을 Self 동작
            if (isLocalPlayer)
                transform.position = position;
        }
    }

    [Command]
    public void CmdSetPosition(Vector3 position)
    {
        this.transform.position = position;
        base.SetDirtyBit(1);
    }

    [ClientRpc]
    public void RpcSetPosition(Vector3 position)
    {
        this.transform.position = position;
        base.SetDirtyBit(1);
    }
}
