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

    public virtual void OnBulletHited(Actor attacker, int damage)
    {
        Debug.Log("OnBulletHited damage = " + damage);
        DecreaseHP(attacker, damage);
    }

    public virtual void OnCrash(Actor attacker, int damage)
    {
        Debug.Log("OnCrash attacker = " + attacker.name + ", damage = " + damage);
        DecreaseHP(attacker, damage);
    }

    void DecreaseHP(Actor attacker, int value)
    {
        if (isDead)
            return;

        CurrentHP -= value;

        if (CurrentHP < 0)
            CurrentHP = 0;

        if (CurrentHP == 0)
        {
            OnDead(attacker);
        }

    }

    protected virtual void OnDead(Actor killer)
    {
        Debug.Log(name + " OnDead");
        isDead = true;

        SystemManager.Instance.EffectManager.GenerateEffect(1, transform.position);
    }


}
