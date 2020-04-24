using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class EnemyGenerateData
{
    public string FilePath; // 적 파일 패스
    public int MaxHP;       // 최대 HP
    public int Damage;      // 총알 데미지
    public int CrashDamage; // 충돌 데미지
    public float BulletSpeed;
    public int FireRemainCount;
    public int GamePoint;

    //
    public Vector3 GeneratePoint;    // 입장전 생성되는 위치
    public Vector3 AppearPoint;      // 입장시 도착 위치

    public Vector3 DisappearPoint;      // 퇴장시 목표 위치
}

public class Squadron : MonoBehaviour
{
    [SerializeField]
    EnemyGenerateData[] enemyGenerateDatas;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void GenerateAllData()
    {
        for(int i = 0; i < enemyGenerateDatas.Length; i++)
        {
            SystemManager.Instance.EnemyManager.GenerateEnemy(enemyGenerateDatas[i]);
        }
    }
}
