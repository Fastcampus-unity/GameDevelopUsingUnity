using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WarningPanel : BasePanel
{
    const float BGColorVariationRate = 0.8f;
    const float MaxMoveTime = 5.0f;
    const float StayTime = 2.0f;
    //
    [SerializeField]
    Image BackgroundImage;
    float BGImageColorMax;
    //
    enum WarningUIPhase : int
    {
        In,
        Stay,
        Out,
        End,
    }

    WarningUIPhase Phase = WarningUIPhase.In; 

    [SerializeField]
    RectTransform CanvasRectTransform;

    [SerializeField]
    RectTransform TextBackRectTransform;

    float MoveStartTime;
    float CurrentPosX;

    public override void InitializePanel()
    {
        base.InitializePanel();

        BGImageColorMax = BackgroundImage.color.a;
        Color color = BackgroundImage.color;
        color.a = 0.0f;
        BackgroundImage.color = color;

        //
        Vector2 position = TextBackRectTransform.anchoredPosition;
        CurrentPosX = position.x = CanvasRectTransform.sizeDelta.x;
        TextBackRectTransform.anchoredPosition = position;

        MoveStartTime =  Time.time;
        Close();
    }

    public override void UpdatePanel()
    {
        base.UpdatePanel();
        //
        UpdateColor();
        UpdateMove();

    }
    
    void UpdateColor()
    {
        Color color = BackgroundImage.color;
        color.a = Mathf.PingPong(Time.time * BGColorVariationRate, BGImageColorMax);
        BackgroundImage.color = color;
    }

    void UpdateMove()
    {
        if (Phase == WarningUIPhase.End)
            return;

        // 이동하기 위한 부분
        Vector2 position = TextBackRectTransform.anchoredPosition;
        switch(Phase)
        {
            case WarningUIPhase.In:
                CurrentPosX = Mathf.Lerp(CurrentPosX, 0.0f, (Time.time - MoveStartTime) / MaxMoveTime);
                position.x = CurrentPosX;
                TextBackRectTransform.anchoredPosition = position;
                break;
            case WarningUIPhase.Out:
                CurrentPosX = Mathf.Lerp(CurrentPosX, -CanvasRectTransform.sizeDelta.x, (Time.time - MoveStartTime) / MaxMoveTime);
                position.x = CurrentPosX;
                TextBackRectTransform.anchoredPosition = position;
                break;
        }

        // 페이즈 전환을 위한 부분
        switch (Phase)
        {
            case WarningUIPhase.In:
                if (CurrentPosX < 1.0f)
                {
                    Phase = WarningUIPhase.Stay;
                    MoveStartTime = Time.time;
                    OnPhaseStay();
                }
                break;
            case WarningUIPhase.Stay:
                if((Time.time - MoveStartTime) > StayTime)
                {
                    Phase = WarningUIPhase.Out;
                    MoveStartTime = Time.time;
                }
                break;
            case WarningUIPhase.Out:
                if (CurrentPosX < -CanvasRectTransform.sizeDelta.x + 1.0f)
                {
                    Phase = WarningUIPhase.End;
                    OnPhaseEnd();
                }
                break;
        }
    }

    void OnPhaseStay()
    {
        SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().GenerateBoss();
    }

    void OnPhaseEnd()
    {
        Close();

        if (((FWNetworkManager)FWNetworkManager.singleton).isServer)
            SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().SetRunningState();
    }

}
