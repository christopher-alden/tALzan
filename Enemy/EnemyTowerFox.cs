using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTowerFox : Enemy
{
    private AStar units;
    public AStar Units
    {
        get { return units; }
    }

    public override void Attack()
    {
        
    }

    public override void StartAttackAnimation()
    {
        
    }

    public override void StartHurtAnimation()
    {
        
    }

    public override void StartIdleAnimation()
    {
        
    }

    public override void StartRunAnimation()
    {
        
    }

    public override void StartWalkAnimation()
    {
        
    }

    protected override void ChildAwake()
    {
        
        units = GetComponent<AStar>();
    }

    protected override void ChildStart()
    {
        InitGameManager();

        InitParams();
    }
    public void DUAR()
    {
        //gameManager.OnKill(maxHealth);
        gameManager.DestroyObject(this.gameObject);
    }

    protected override void ChildUpdate()
    {
        if (health <= 0)
        {
            gameManager.OnKill(maxHealth);
            gameManager.DestroyObject(this.gameObject);
        }
        
    }
    private float maxHealth;

    protected override void InitParams()
    {
        //NMAgent.speed = gameManager.WaveLevel;
        units.moveSpeed = gameManager.WaveLevel;
        maxHealth = 100 + (20*gameManager.WaveLevel);
        health = 100 + (20 * gameManager.WaveLevel);
    }

}
