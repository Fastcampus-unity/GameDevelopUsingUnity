using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameEndPanel : BasePanel
{
    [SerializeField]
    GameObject Success;

    [SerializeField]
    GameObject Fail;

    public override void InitializePanel()
    {
        base.InitializePanel();
        Close();
    }

    public void ShowGameEnd(bool success)
    {
        base.Show();

        if(success)
        {
            Success.SetActive(true);
            Fail.SetActive(false);
        }
        else
        {
            Success.SetActive(false);
            Fail.SetActive(true);
        }
    }

    public void OnOK()
    {
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().GotoTitleScene();
    }

}
