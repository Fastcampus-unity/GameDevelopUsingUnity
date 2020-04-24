using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LadingSceneMain : BaseSceneMain
{
    const float TextUpdateIntaval = 0.15f;
    const string LoadingTextValue = "Loading...";

    [SerializeField]
    Text LoadingText;

    int TextIndex = 0;
    float LastUpdateTime;

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
    }
}
