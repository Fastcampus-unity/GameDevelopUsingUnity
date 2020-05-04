using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bomb : Bullet
{
    const float MaxRotateTime = 30.0f;

    /// <summary>
    /// 최종 회전값
    /// </summary>
    const float MaxRotateZ = 90.0f;

    [SerializeField]
    Rigidbody selfRigidbody;

    [SerializeField]
    Vector3 Force;

    [SyncVar]
    float RotateStartTime = 0.0f; // 회전을 시작한 시간

    [SyncVar]
    [SerializeField]
    float CurrentRotateZ;

    Vector3 currentEulerAngles = Vector3.zero;

    [SerializeField]
    SphereCollider ExplodeArea;

    protected override void UpdateTransform()
    {
        if (!NeedMove)
            return;

        if (CheckScreenBottom())
            return;


        UpdateRotate();
    }

    bool CheckScreenBottom()
    {
        Transform mainBGQuadTransform = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().MainBGQuadTransform;

        if (transform.position.y < -mainBGQuadTransform.localScale.y * 0.5f)
        {
            Vector3 newPos = transform.position;
            newPos.y = -mainBGQuadTransform.localScale.y * 0.5f;
            StopForExplosion(newPos);
            Explode();
            return true;
        }

        return false;
    }

    void StopForExplosion(Vector3 stopPos)
    {
        transform.position = stopPos;

        selfRigidbody.useGravity = false;   // 중력 사용을 해제
        selfRigidbody.velocity = Vector3.zero;  // Force를 초기화
        NeedMove = false;
    }

    void UpdateRotate()
    {
        CurrentRotateZ = Mathf.Lerp(CurrentRotateZ, MaxRotateZ, (Time.time - RotateStartTime) / MaxRotateTime);
        currentEulerAngles.z = -CurrentRotateZ;

        Quaternion rot = Quaternion.identity;
        rot.eulerAngles = currentEulerAngles;
        transform.localRotation = rot;
    }

    public override void Fire(int ownerInstanceID, Vector3 direction, float speed, int damage)
    {
        base.Fire(ownerInstanceID, direction, speed, damage);

        AddForce(Force);
    }

    void InternelAddForce(Vector3 force)
    {
        selfRigidbody.velocity = Vector3.zero;  // Force를 초기화
        selfRigidbody.AddForce(force);
        RotateStartTime = Time.time;
        CurrentRotateZ = 0.0f;
        transform.localRotation = Quaternion.identity;
        selfRigidbody.useGravity = true;    // 중력 사용을 다시 활성화
        ExplodeArea.enabled = false;
    }

    public void AddForce(Vector3 force)
    {
        // 정상적으로 NetworkBehaviour 인스턴스의 Update로 호출되어 실행되고 있을때
        //CmdAddForce(force);

        // MonoBehaviour 인스턴스의 Update로 호출되어 실행되고 있을때의 꼼수
        if (isServer)
        {
            RpcAddForce(force);        // Host 플레이어인경우 RPC로 보내고
        }
        else
        {
            CmdAddForce(force);        // Client 플레이어인경우 Cmd로 호스트로 보낸후 자신을 Self 동작
            if (isLocalPlayer)
                InternelAddForce(force);
        }
    }

    [Command]
    public void CmdAddForce(Vector3 force)
    {
        InternelAddForce(force);
        base.SetDirtyBit(1);
    }

    [ClientRpc]
    public void RpcAddForce(Vector3 force)
    {
        InternelAddForce(force);
        base.SetDirtyBit(1);
    }

    void InternalExplode()
    {
        Debug.Log("InternalExplode is called");
        GameObject go = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EffectManager.GenerateEffect(EffectManager.BombExplodeFxIndex, transform.position);
        //
        ExplodeArea.enabled = true;
        List<Enemy> targetList = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyManager.GetContainEnemies(ExplodeArea);
        for(int i = 0; i < targetList.Count; i++)
        {
            if (targetList[i].IsDead)
                continue;

            targetList[i].OnBulletHited(Damage, targetList[i].transform.position);
        }

        //
        if (gameObject.activeSelf)
            Disappear();
    }

    void Explode()
    {
        // 정상적으로 NetworkBehaviour 인스턴스의 Update로 호출되어 실행되고 있을때
        //CmdExplode();

        // MonoBehaviour 인스턴스의 Update로 호출되어 실행되고 있을때의 꼼수
        if (isServer)
        {
            RpcExplode();        // Host 플레이어인경우 RPC로 보내고
        }
        else
        {
            CmdExplode();        // Client 플레이어인경우 Cmd로 호스트로 보낸후 자신을 Self 동작
            if (isLocalPlayer)
                InternalExplode();
        }
    }

    [Command]
    public void CmdExplode()
    {
        InternalExplode();
        base.SetDirtyBit(1);
    }

    [ClientRpc]
    public void RpcExplode()
    {
        InternalExplode();
        base.SetDirtyBit(1);
    }

    protected override bool OnBulletCollision(Collider collider)
    {
        if (!base.OnBulletCollision(collider))
        {
            return false;
        }

        Explode();
        return true;
    }
}