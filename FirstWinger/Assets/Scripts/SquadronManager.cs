using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadronManager : MonoBehaviour
{
    float GameStartedTime;

    int ScheduleIndex;

    [SerializeField]
    SquadronTable[] squadronDatas;

    [SerializeField]
    SquadronScheduleTable squadronScheduleTable;

    bool running = false;

    bool AllSquadronGenerated = false;
    bool ShowWarningUICalled = false;

    // Start is called before the first frame update
    void Start()
    {
        squadronDatas = GetComponentsInChildren<SquadronTable>();
        for(int i = 0; i < squadronDatas.Length; i++)
            squadronDatas[i].Load();

        squadronScheduleTable.Load();
    }

    // Update is called once per frame
    void Update()
    {
        if (!AllSquadronGenerated)
            CheckSquadronGeneratings();
        else if(!ShowWarningUICalled)
        {
            InGameSceneMain inGameSceneMain = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>();
            if (SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyManager.GetEnemyListCount() == 0)
            {
                inGameSceneMain.ShowWarningUI();
                ShowWarningUICalled = true;
            }
        }
    }

    public void StartGame()
    {

        GameStartedTime = Time.time;
        ScheduleIndex = 0;
        running = true;
        Debug.Log("Game Started!");
    }

    void CheckSquadronGeneratings()
    {
        if (!running)
            return;

        SquadronScheduleDataStruct data = squadronScheduleTable.GetScheduleData(ScheduleIndex);

        if (Time.time - GameStartedTime >= data.GenerateTime)
        {
            GenerateSquadron(squadronDatas[data.SquadronID]);
            ScheduleIndex++;

            if (ScheduleIndex >= squadronScheduleTable.GetDataCount())
            {
                OnAllSquadronGenerated();
                return;
            }
        }
    }

    void GenerateSquadron(SquadronTable table)
    {
        Debug.Log("GenerateSquadron : " + ScheduleIndex);

        for(int i = 0; i < table.GetCount(); i++)
        {
            SquadronMemberStruct squadronMember = table.GetSquadronMember(i);
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().EnemyManager.GenerateEnemy(squadronMember);
        }
    }

    void OnAllSquadronGenerated()
    {
        Debug.Log("AllSquadronGenerated");
        running = false;

        AllSquadronGenerated = true;
   }



}
