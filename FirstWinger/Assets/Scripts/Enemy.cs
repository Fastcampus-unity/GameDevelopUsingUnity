using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum State
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




    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    void UpdateSpeed()
    {
    }

    void UpdateMove()
    {
    }

    void Arrived()
    {

    }

    public void Appear(Vector3 targetPos)
    {
        TargetPosition = targetPos;
        CurrentSpeed = MaxSpeed;

        CurrentState = State.Appear;
    }

    void Disappear(Vector3 targetPos)
    {
        TargetPosition = targetPos;
        CurrentSpeed = 0.0f;

        CurrentState = State.Disappear;
    }
}
