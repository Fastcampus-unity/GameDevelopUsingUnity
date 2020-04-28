using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bomb : Bullet
{
    [SerializeField]
    Rigidbody selfRigidbody;

    [SerializeField]
    Vector3 Force;

    protected override void UpdateMove()
    {
        if (!NeedMove)
            return;
    }

    public override void Fire(int ownerInstanceID, Vector3 firePosition, Vector3 direction, float speed, int damage)
    {
        base.Fire(ownerInstanceID, firePosition, direction, speed, damage);

        selfRigidbody.velocity = Vector3.zero;  // Force를 초기화
        AddForce(Force);
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
                selfRigidbody.AddForce(force);
        }
    }

    [Command]
    public void CmdAddForce(Vector3 force)
    {
        selfRigidbody.AddForce(force);
        base.SetDirtyBit(1);
    }

    [ClientRpc]
    public void RpcAddForce(Vector3 force)
    {
        selfRigidbody.AddForce(force);
        base.SetDirtyBit(1);
    }
}
