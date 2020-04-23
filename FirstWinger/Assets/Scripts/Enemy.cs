using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    State CurrentState = State.None;

    /// <summary>
    /// 최고 속도
    /// </summary>
    const float MaxSpeed = 10.0f;

    /// <summary>
    /// 최고 속도에 이르는 시간
    /// </summary>
    const float MaxSpeedTime = 0.5f;


    /// <summary>
    /// 목표점
    /// </summary>
    [SerializeField]
    Vector3 TargetPosition;

    [SerializeField]
    float CurrentSpeed;

    /// <summary>
    /// 방향을 고려한 속도 벡터
    /// </summary>
    Vector3 CurrentVelocity;

    float MoveStartTime = 0.0f; // 이동시작 시간

    [SerializeField]
    Transform FireTransform;

    [SerializeField]
    GameObject Bullet;

    [SerializeField]
    float BulletSpeed = 1;


    float LastBattleUpdateTime = 0.0f;

    [SerializeField]
    int FireRemainCount = 1;

    [SerializeField]
    int GamePoint = 10;

    // Update is called once per frame
    protected override void UpdateActor()
    {
        //
        switch (CurrentState)
        {
            case State.None:
            case State.Ready:
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

    void UpdateSpeed()
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
            CurrentState = State.Battle;
            LastBattleUpdateTime = Time.time;
        }
        else // if (CurrentState == State.Disappear)
        {
            CurrentState = State.None;
        }
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

    void UpdateBattle()
    {
        if(Time.time - LastBattleUpdateTime > 1.0f)
        {
            if (FireRemainCount > 0)
            {
                Fire();
                FireRemainCount--;
            }
            else
            {
                Disappear(new Vector3(-15.0f, transform.position.y, transform.position.z));
            }

            LastBattleUpdateTime = Time.time;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Player player = other.GetComponentInParent<Player>();
        if (player)
        {
            if (!player.IsDead)
                player.OnCrash(this, CrashDamage);
        }
    }

    public override void OnCrash(Actor attacker, int damage)
    {
        base.OnCrash(attacker, damage);
    }

    public void Fire()
    {
        GameObject go = Instantiate(Bullet);

        Bullet bullet = go.GetComponent<Bullet>();
        bullet.Fire(this, FireTransform.position, -FireTransform.right, BulletSpeed, Damage);
    }

    protected override void OnDead(Actor killer)
    {
        base.OnDead(killer);

        SystemManager.Instance.GamePointAccumulator.Accumulate(GamePoint);

        CurrentState = State.Dead;

    }
}

