using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneMain : BaseSceneMain
{
    public void OnStartButton()
    {
        Debug.Log("OnStartButton");

        SceneController.Instance.LoadScene(SceneNameConstants.LoadingScene);
        //SceneController.Instance.LoadSceneAdditive(SceneNameConstants.LoadingScene);
    }

}
