using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField]
    Gage HPGage;

    [SerializeField]
    Player OwnerPlayer;

    Transform OwnerTransform;
    Transform SelfTransform;

    // Start is called before the first frame update
    void Start()
    {
        SelfTransform = transform;
    }

    public void Initialize(Player player)
    {
        OwnerPlayer = player;
        OwnerTransform = OwnerPlayer.transform;
    }

    // Update is called once per frame
    void Update()
    {
        UpdatePosition();
        UpdateHP();
    }

    void UpdatePosition()
    {
        if(OwnerTransform != null)
            SelfTransform.position = Camera.main.WorldToScreenPoint(OwnerTransform.position);
    }

    void UpdateHP()
    {
        if (OwnerPlayer != null)
        {
            if (!OwnerPlayer.gameObject.activeSelf)
                gameObject.SetActive(OwnerPlayer.gameObject.activeSelf);

            HPGage.SetHP(OwnerPlayer.HPCurrent, OwnerPlayer.HPMax);
        }
    }
}
