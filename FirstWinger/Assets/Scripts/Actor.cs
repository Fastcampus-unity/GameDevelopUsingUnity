using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    [SerializeField]
    protected int MaxHP = 100;

    [SerializeField]
    protected int CurrentHP;

    [SerializeField]
    protected int Damage = 1;

    [SerializeField]
    protected int crashDamage = 100;

    [SerializeField]
    bool isDead = false;

    public bool IsDead
    {
        get
        {
            return isDead;
        }
    }

    protected int CrashDamage
    {
        get
        {
            return crashDamage;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    protected virtual void Initialize()
    {
        CurrentHP = MaxHP;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateActor();
    }

    protected virtual void UpdateActor()
    {

    }

    public virtual void OnBulletHited(int damage)
    {
        Debug.Log("OnBulletHited damage = " + damage);
        DecreaseHP(damage);
    }

    public virtual void OnCrash(int damage)
    {
        Debug.Log("OnCrash damage = " + damage);
        DecreaseHP(damage);
    }

    void DecreaseHP(int value)
    {
        if (isDead)
            return;

        CurrentHP -= value;

        if (CurrentHP < 0)
            CurrentHP = 0;

        if (CurrentHP == 0)
        {
            OnDead();
        }

    }

    protected virtual void OnDead()
    {
        Debug.Log(name + " OnDead");
        isDead = true;

    }


}
