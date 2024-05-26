using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHitbox : MonoBehaviour
{
    private float health;
    private float attackDamage;
    private float attackCd;
    private bool isAttackEnabled;


    public float AttackDamage
    {
        set { attackDamage = value; }
        get { return attackDamage; }
    }
    public float AttackCd
    {
        set { attackCd = value; }
        get { return attackCd; }
    }


    #region Singleton
    private static PlayerHitbox instance;
    public static PlayerHitbox Instance
    {
        get { return instance; }
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    #endregion

    public float TakeDamage(float damage)
    {
        return damage;
    }
    public float InvokeAttack()
    {
        return attackDamage;
    }

    


}
