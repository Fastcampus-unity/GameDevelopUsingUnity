using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingSceneMain : BaseSceneMain
{
    const float NextSceneIntaval = 3.0f;
    const float TextUpdateIntaval = 0.15f;
    const string LoadingTextValue = "Loading...";

    [SerializeField]
    Text LoadingText;

    int TextIndex = 0;
    float LastUpdateTime;

    float SceneStartTime;
    bool NextSceneCall = false;

    protected override void OnStart()
    {
        SceneStartTime = Time.time;
    }

    protected override void UpdateScene()
    {
        base.UpdateScene();

        float currentTime = Time.time;
        if(currentTime - LastUpdateTime > TextUpdateIntaval)
        {

            LoadingText.text = LoadingTextValue.Substring(0, TextIndex + 1);

            TextIndex++;
            if(TextIndex >= LoadingTextValue.Length)
            {
                TextIndex = 0;
            }


            LastUpdateTime = currentTime;
        }
        //
        if(currentTime - SceneStartTime > NextSceneIntaval)
        {
            if(!NextSceneCall)
                GotoNextScene();
        }
    }

    void GotoNextScene()
    {
        NetworkConnectionInfo info = SystemManager.Instance.ConnectionInfo;
        if (info.Host)
        {
            Debug.Log("FW Start with host!");
            FWNetworkManager.singleton.StartHost();
        }
        else
        {
            Debug.Log("FW Start with client!");

            if (!string.IsNullOrEmpty(info.IPAddress))
                FWNetworkManager.singleton.networkAddress = info.IPAddress;

            if (info.Port != FWNetworkManager.singleton.networkPort)
                FWNetworkManager.singleton.networkPort = info.Port;

            FWNetworkManager.singleton.StartClient();
        }


        NextSceneCall = true;
    }

}
