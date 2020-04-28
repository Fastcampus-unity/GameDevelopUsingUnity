using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

[System.Serializable]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct EnemyStruct
{
    public int      index;
    [MarshalAs(UnmanagedType.ByValTStr, SizeConst = MarshalTableConstant.charBufferSize)]
    public string   FilePath;
    public int      MaxHP;
    public int      Damage;
    public int      CrashDamage;
    public int      BulletSpeed;
    public int      FireRemainCount;
    public int      GamePoint;
    public float    ItemDropRate;
    public int      ItemDropID;
}

public class EnemyTable : TableLoader<EnemyStruct>
{
    Dictionary<int, EnemyStruct> tableDatas = new Dictionary<int, EnemyStruct>();

    // Start is called before the first frame update
    void Start()
    {
        Load();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void AddData(EnemyStruct data)
    {
        tableDatas.Add(data.index, data);
    }

    public EnemyStruct GetEnemy(int index)
    {
        if(!tableDatas.ContainsKey(index))
        {
            Debug.LogError("GetEnemy Error! index = " + index);
            return default(EnemyStruct);
        }

        return tableDatas[index];
    }
}
