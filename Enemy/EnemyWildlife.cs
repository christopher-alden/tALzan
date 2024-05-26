using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// bear class


public class EnemyWildlife : Enemy
{

    protected IWildlifeState currentState;
    public GameObject Tagged;
    
    
    protected Coroutine lastDistanceRoutine;
    //protected bool inSpawnPointBounds;

    protected Transform owner; //ini buat pet
    public Transform Owner
    {
        get { return owner; }
        set { owner = value; }
    }

    #region Animation
    

    public override void StartWalkAnimation()
    {
        if (!animation.IsPlaying("walk"))
        {
            animation["walk"].speed = 0.75f;
            animation.CrossFade("walk");
        }
    }

    public override void StartRunAnimation()
    {
        if (!animation.IsPlaying("run"))
        {
            animation["run"].speed = 1.1f;
            animation.CrossFade("run");
        }
    }

    public override void StartAttackAnimation()
    {
        if (!animation.IsPlaying("4LegsBiteAttack"))
        {
            animation["4LegsBiteAttack"].speed = 0.9f;
            animation["4LegsBiteAttack"].wrapMode = WrapMode.Once;
            animation.CrossFade("4LegsBiteAttack");
        }
    }

    public override void StartIdleAnimation()
    {
        if (!animation.IsPlaying("idle4Legs"))
        {
            animation["idle4Legs"].speed = 1.2f;
            animation.CrossFade("idle4Legs");
        }
    }

    public override void StartHurtAnimation()
    {
        if (!animation.IsPlaying("4LegsGetHit"))
        {
            //animation["4LegsGetHit"].speed = 1.2f;
            animation["4LegsGetHit"].wrapMode = WrapMode.Once;
            animation.CrossFade("4LegsGetHit");
        }

    }

    #endregion

    public void CheckStillTarget()
    {
        if (target == null || Tagged == null) return;
        //Debug.Log(Tagged);
        if (!CheckTag(Tagged))
        {
            Tagged = null;
            target = null;
        }
    }
    public void ChaseTarget(Transform target)
    {
        wayPointSet = false;
        StopLastDistanceRoutine();
        NMAgent.SetDestination(target.position);
        Run();
        //SurfaceAlignment();
        //transform.LookAt(target);
    }

    

    public void Patrol()
    {
        if (!wayPointSet)
        {
            wayPoint = FindWayPoint();
            StartLastDistanceRoutine();
            NMAgent.SetDestination(wayPoint);
            Walk();
        }

        //distanceToWayPoint = transform.position - wayPoint;
        
        // condition if sampe ke target
        if (NMAgent.remainingDistance < 5.0f)
        {
            StopLastDistanceRoutine();
            wayPointSet = false;
        }
    }

    public float avoidDistance = 10f;
    public void FollowOwner()
    {
        if (owner == null) return;


        Vector3 directionToOwner = (owner.position - NMAgent.transform.position).normalized;
        Vector3 adjustedDestination = owner.position - directionToOwner * avoidDistance;
        NMAgent.SetDestination(adjustedDestination);
        float remainingDistance = (adjustedDestination - transform.position).magnitude;

        if (remainingDistance > 27f)
        {
            Run();
        }
        else if (remainingDistance >= 15f && remainingDistance < 25f)
        {
            Walk();
        }
        else if (remainingDistance < 13f)
        {
            StopAndIdle();
        }
    }

    public void AttackTarget()
    {

        //Debug.Log("called AttackTarget");

        if (isAttackEnabled && isInPlayerHitbox)
        {
            //Debug.Log("Attack Target passed condition");
            if (wayPointSet)
            {
                wayPointSet = false;
                StopLastDistanceRoutine();
            }
            
            NMAgent.SetDestination(transform.position);
            NMAgent.updateRotation = false;
            RotateToTargetRoutine = StartCoroutine(RotateTowardsTarget(target));
            
            Attack();
            //Debug.Log("done attack");

            if(RotateToTargetRoutine!=null)StopCoroutine(RotateToTargetRoutine);
            NMAgent.updateRotation = true;
            isAttackEnabled = false;
            InvokeResetAttackCooldown();
        }
    }
    //todo:ganti
    public bool AttackInProgress;
    public void PetAttackTarget()
    {

        //Debug.Log("called PetAttackTarget");
        if (isAttackEnabled && NMAgent.remainingDistance < 20f)
        {
            AttackInProgress = true;
            isAttackEnabled = false;

            NMAgent.SetDestination(transform.position);
            NMAgent.updateRotation = false;
            RotateToTargetRoutine = StartCoroutine(RotateTowardsTarget(target));

            PetAttack();
            //Debug.Log("done attack");

            if (RotateToTargetRoutine != null) StopCoroutine(RotateToTargetRoutine);
            NMAgent.updateRotation = true;
            NMAgent.SetDestination(target.position);
            InvokeResetAttackCooldown();
        }
            AttackInProgress = false;
    }

    private IEnumerator LastDistanceRoutine()
    {
        float tolerance = 0.5f;
        while (true)
        {
            float initialDistance = NMAgent.remainingDistance;
            yield return new WaitForSeconds(1f);

            // condition if cannot stuck
            if (Mathf.Abs(NMAgent.remainingDistance - initialDistance) < tolerance)
            {
                //Debug.Log("reseted waypoint");
                // set false buat reset
                wayPointSet = false;
                StopLastDistanceRoutine();
                break;
            }
        }
    }

    // boiler plate
    public void StartLastDistanceRoutine()
    {
        if (lastDistanceRoutine != null)
        {
            StopCoroutine(lastDistanceRoutine);
        }
        lastDistanceRoutine = StartCoroutine(LastDistanceRoutine());
    }

    public void StopLastDistanceRoutine()
    {
        if (lastDistanceRoutine != null)
        {
            StopCoroutine(lastDistanceRoutine);
        }
    }


    public void SetState(IWildlifeState newState)
    {
        currentState = newState;
    }

    public override void Attack()
    {
        //Debug.Log("lagi attack");
        StartAttackAnimation();
        CheckSphereAttack();
    }

    public void PetAttack()
    {
        //Debug.Log("lagi attack");
        StartAttackAnimation();
        float radius = 15f;
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, radius);
        foreach (var collider in colliderArray)
        {
            if (collider.TryGetComponent(out Enemy enemy))
            {
                if(enemy.gameObject.CompareTag("Enemy")) enemy.TakeDamage(attackDmg);
                //Debug.Log(enemy.Health);
            }
        }
    }

    protected override void ChildStart()
    {
        InitGameManager();
        InitNavMesh();
        InitAnimation();
        InitHealthBar();
        target = gameManager.GetPlayerPosition();
        currentState = new WildlifeState(this);
    }

    protected override void ChildAwake()
    {
        // auto check

        InitParams();
        if(checkTargetInRangeRoutine != null)
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

    protected override void ChildUpdate()
    {
        currentState.UpdateState();
    }

    protected override void InitParams()
    {
        targetInSight = false;
        targetInRange = false;
        wayPointSet = false;
        //currentAnimation = AnimationState.idle;
        health = 200;
        attackDmg = 15;
        attackCd = 3;
    }

    
}
