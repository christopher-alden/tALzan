using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDragon : Enemy
{
    public Animator animator;
    public override void Attack()
    {
        animator.SetBool("walk", false);
        animator.SetBool("attack", true);
        GameManager.Instance.playerReference.TakeDamage(attackDmg);
        Invoke(nameof(invokeAttackFalse), 0.2f);

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
        
        
    }

    public void InitAnimator()
    {
        animator = GetComponentInChildren<Animator>();
    }

    protected override void ChildStart()
    {
        InitGameManager();
        InitAnimator();
        InitNavMesh();
        InitParams();
        InitBossBar();
    }

    public ProgressBar bossBar;

    private void InitBossBar()
    {
        bossBar.SetMaxValue(maxHealth);
        bossBar.SetValue(health);
    }


    void CheckForTarget()
    {
        
        if (target)
        {
            //Debug.Log(target.gameObject);
            if (checkTargetInRangeRoutine != null)
            {
                StopCoroutine(checkTargetInRangeRoutine);
            }
            checkTargetInRangeRoutine = StartCoroutine(CheckTargetInRangeRoutine());

            if (checkTargetInSightRoutine != null)
            {
                StopCoroutine(checkTargetInSightRoutine);
            }
            checkTargetInSightRoutine = StartCoroutine(CheckTargetInSightRoutine());
        }
        else
        {
            if (checkTargetInRangeRoutine != null)
            {
                StopCoroutine(checkTargetInRangeRoutine);
            }
            if (checkTargetInSightRoutine != null)
            {
                StopCoroutine(checkTargetInSightRoutine);
            }
        }
    }


    public void ChaseTarget(Transform target)
    {
        animator.SetBool("walk", true);
        //Debug.Log("chasing");
        wayPointSet = false;
        //StopLastDistanceRoutine();
        NMAgent.SetDestination(target.position);
        //Run();
        //SurfaceAlignment();
        //transform.LookAt(target);
    }

    public void AttackTarget()
    {

        //Debug.Log("called AttackTarget");

        if (isAttackEnabled )
        {
            //Debug.Log("Attack Target passed condition");
            //if (wayPointSet)
            //{
            //    wayPointSet = false;
            //    StopLastDistanceRoutine();
            //}

            NMAgent.SetDestination(transform.position);
            NMAgent.updateRotation = false;
            RotateToTargetRoutine = StartCoroutine(RotateTowardsTarget(target));

            //if (isInPlayerHitbox)
            //{

            //}
            Attack();
            isAttackEnabled = false;
            InvokeResetAttackCooldown();
            //Debug.Log("done attack");

            if (RotateToTargetRoutine != null) StopCoroutine(RotateToTargetRoutine);
            NMAgent.updateRotation = true;
            

        }
    }
    
    public void invokeAttackFalse()
    {
        animator.SetBool("attack", false);
    }

    protected override void ChildUpdate()
    {
        NMAgent.isStopped = false;
        
        CheckForTarget();
        bossBar.SetValue(health);
        if (!CheckInSpawnBounds())
        {
            NMAgent.SetDestination(spawnPoint);
        }
        //if (!target) return;
        if (TargetInRange && IsAttackEnabled)
        {
            //reset = false;
            AttackTarget();
        }
        else if (TargetInSight && CheckInSpawnBounds())
        {
            //Debug.Log("insight aja");
            ChaseTarget(target);
        }
        if (!target)
        {
            Debug.Log("ini partrol");
        }
    }
    private float maxHealth;
    protected override void InitParams()
    {
        targetInSight = false;
        targetInRange = false;
        wayPointSet = false;
        isAttackEnabled = true;
        health = 1000;
        maxHealth = 1000;
        attackDmg = 50;
        attackCd = 5;
        //NMAgent.SetDestination(gameManager.GetPlayerPosition().position);
    }
}
