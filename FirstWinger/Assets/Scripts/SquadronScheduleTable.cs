using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

[System.Serializable]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct SquadronScheduleDataStruct
{
    public int index;
    public float GenerateTime;
    public int SquadronID;
}

public class SquadronScheduleTable : TableLoader<SquadronScheduleDataStruct>
{
    List<SquadronScheduleDataStruct> tableDatas = new List<SquadronScheduleDataStruct>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void AddData(SquadronScheduleDataStruct data)
    {
        tableDatas.Add(data);
    }

    public SquadronScheduleDataStruct GetScheduleData(int index)
    {
        if(index < 0 || index >= tableDatas.Count)
        {
            Debug.LogError("SquadronScheduleDataStruct Error! index = " + index);
            return default(SquadronScheduleDataStruct);
        }

        return tableDatas[index];
    }

    /// <summary>
    /// 외부에서 데이터 갯수에 접근할 수 있도록 추가함
    /// </summary>
    /// <returns>테이블의 스케쥴 데이터 갯수</returns>
    public int GetDataCount()
    {
        return tableDatas.Count;
    }
}
