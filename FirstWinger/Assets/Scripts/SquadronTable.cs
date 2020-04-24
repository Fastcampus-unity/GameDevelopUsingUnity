using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;

[System.Serializable]
[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
public struct SquadronMemberStruct
{
    public int index;
    public int EnemyID;
    public float GeneratePointX;
    public float GeneratePointY;
    public float AppearPointX;
    public float AppearPointY;
    public float DisappearPointX;
    public float DisappearPointY;
}

public class SquadronTable : TableLoader<SquadronMemberStruct>
{
    List<SquadronMemberStruct> tableDatas = new List<SquadronMemberStruct>();

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected override void AddData(SquadronMemberStruct data)
    {
        tableDatas.Add(data);
    }

    public SquadronMemberStruct GetSquadronMember(int index)
    {
        if (index < 0 || index >= tableDatas.Count)
        {
            Debug.LogError("GetSquadronMember Error! index = " + index);
            return default(SquadronMemberStruct);
        }

        return tableDatas[index];
    }

    public int GetCount()
    {
        return tableDatas.Count;
    }
}
