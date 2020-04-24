using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDamage : MonoBehaviour
{
    enum DamageState : int
    {
        None = 0,
        SizeUp,
        Display,
        FadeOut,
    }

    [SerializeField]
    DamageState damageState = DamageState.None;

    const float SizeUpDuration = 0.1f;
    const float DisplayDuration = 0.5f;
    const float FadeOutDuration = 0.2f;

    [SerializeField]
    Text damageText;

    Vector3 CurrentVelocity;

    float DisplayStartTime;
    float FadeOutStartTime;

    public string FilePath
    {
        get;
        set;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        UpdateDamage();
    }

    public void ShowDamage(int damage, Color textColor)
    {
        damageText.text = damage.ToString();
        damageText.color = textColor;
        Reset();
        damageState = DamageState.SizeUp;
    }

    void Reset()
    {
        transform.localScale = Vector3.zero;
        Color newColor = damageText.color;
        newColor.a = 1.0f;
        damageText.color = newColor;
    }

    void UpdateDamage()
    {
        if (damageState == DamageState.None)
            return;

        switch (damageState)
        {
            case DamageState.SizeUp:
                transform.localScale = Vector3.SmoothDamp(transform.localScale, Vector3.one, ref CurrentVelocity, SizeUpDuration);

                if(transform.localScale == Vector3.one)
                {
                    damageState = DamageState.Display;
                    DisplayStartTime = Time.time;
                }
                break;
            case DamageState.Display:
                if(Time.time - DisplayStartTime > DisplayDuration)
                {
                    damageState = DamageState.FadeOut;
                    FadeOutStartTime = Time.time;
                }
                break;
            case DamageState.FadeOut:
                Color newColor = damageText.color;
                newColor.a = Mathf.Lerp(1, 0, (Time.time - FadeOutStartTime) / FadeOutDuration);
                damageText.color = newColor;

                if(newColor.a == 0)
                {
                    damageState = DamageState.None;
                    SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().DamageManager.Remove(this);
                }
                break;
        }
    }
}
