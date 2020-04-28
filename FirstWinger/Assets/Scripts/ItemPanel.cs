using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPanel : BasePanel
{
    [SerializeField]
    Text ItemCountText;

    Player hero = null;

    Player Hero
    {
        get
        {
            if (hero == null)
                hero = SystemManager.Instance.GetCurrentSceneMain<InGameSceneMain>().Hero;
            return hero;
        }
    }

    public override void UpdatePanel()
    {
        base.UpdatePanel();
        if (Hero != null)
        {
            ItemCountText.text = hero.ItemCount.ToString();
        }

    }
}
