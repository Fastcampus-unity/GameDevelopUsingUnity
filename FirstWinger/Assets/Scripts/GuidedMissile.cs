using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GuidedMissile : Bullet
{
    /// <summary>
    /// 타겟 추적시 방향을 전환하는 정도
    /// </summary>
    const float ChaseFector = 1.5f;

    /// <summary>
    /// 타겟 추적을 시작하는 시간(발사시간이 기준)
    /// </summary>
    const float ChasingStartTime = 0.7f;

    /// <summary>
    /// 타겟 추적을 종료하는 시간(발사시간이 기준)
    /// </summary>
    const float ChasingEndTime = 4.5f;

    /// <summary>
    /// 목표 Actor의 ActorInstanceID
    /// </summary>
    [SyncVar]
    [SerializeField]
    int TargetInstanceID;

    /// <summary>
    /// 이동 벡터
    /// </summary>
    [SerializeField]
    Vector3 ChaseVector;

    [SerializeField]
    Vector3 rotateVector = Vector3.zero;

    /// <summary>
    /// 방향회전을 뒤집기 위한 플래그
    /// </summary>
    [SerializeField]
    bool FlipDirection = true;  // 디폴트 상태가 Left 받향일 경우 true


    bool needChase = true;

    public void FireChase(int targetInstanceID, int ownerInstanceID, Vector3 direction, float speed, int damage)
    {
        if (!isServer)
            return;

        RpcSetTargetInstanceID(targetInstanceID);        // Host 플레이어인경우 RPC
        base.Fire(ownerInstanceID, direction, speed, damage);
    }

    [ClientRpc]
    public void RpcSetTargetInstanceID(int targetInstanceID)
    {
        TargetInstanceID = targetInstanceID;
        base.SetDirtyBit(1);
    }

    protected override void UpdateTransform()
    {
        UpdateMove();
        UpdateRotate();
    }

    protected override void UpdateMove()
    {
        if (!NeedMove)
            return;

        Vector3 moveVector = MoveDirection.normalized * Speed * Time.deltaTime;
        // 타겟을 추적하기 위한 계산
        float deltaTime = Time.time - FiredTime;

        if (deltaTime > ChasingStartTime && deltaTime < ChasingEndTime)
        {
            Actor target = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().ActorManager.GetActor(TargetInstanceID);
            if (target != null)
            {
                // 현재 위치에서 타겟까지 벡터
                Vector3 targetVector = target.transform.position - transform.position;

                // 이동 벡터와 타겟 벡터의 사이의 벡터를 계산
                ChaseVector =  Vector3.Lerp(moveVector.normalized, targetVector.normalized, ChaseFector * Time.deltaTime);

                // 이동 벡터에 추적벡터를 더하고 스피드에 따른 길이를 다시 계산
                moveVector += ChaseVector.normalized;
                moveVector = moveVector.normalized * Speed * Time.deltaTime;

                // 수정 계산된 이동벡터를 필드에 저장해서 다음 UpdateMove에서 사용가능하게 한다
                MoveDirection = moveVector.normalized;
            }
        }

        moveVector = AdjustMove(moveVector);
        transform.position += moveVector;

        // moveVector 방향으로 회전시키기 위한 계산
        rotateVector.z = Vector2.SignedAngle(Vector2.right, moveVector);
        if (FlipDirection)
            rotateVector.z += 180.0f;
    }

    void UpdateRotate()
    {
        Quaternion quat = Quaternion.identity;
        quat.eulerAngles = rotateVector;
        transform.rotation = quat;
        
    }
}
