using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public abstract class Enemy : MonoBehaviour
{

    [SerializeField] protected float health;
    [SerializeField] protected float attackDmg;
    [SerializeField] protected float chaseSpeed = 17.0f;
    [SerializeField] protected float walkSpeed = 7.0f;
    [SerializeField] protected float attackCd;
    [SerializeField] protected bool isAttackEnabled=true;
    protected GameManager gameManager;

    public GameManager GameManager
    {
        get { return gameManager; }
    }

    public float Health
    {
        get { return health; }
        set { health = value; }
    }
   
    public bool IsAttackEnabled
    {
        get { return isAttackEnabled; }
    }
    
    public float ChaseSpeed
    {
        get { return chaseSpeed; }
    }
    public float WalkSpeed
    {
        get { return walkSpeed; }
    }

    #region NavMeshParameters

    public bool Visualize;
    protected NavMeshAgent NMAgent;

    [SerializeField]protected Transform target;
    
    [SerializeField] protected LayerMask isGround, isTarget;
    [SerializeField] protected float sightRange, attackRange, walkRange;
    protected Vector3 wayPoint, distanceToWayPoint, spawnPoint;
    [SerializeField] protected bool targetInSight, targetInRange, wayPointSet;
    protected Coroutine checkTargetInSightRoutine;
    protected Coroutine checkTargetInRangeRoutine;
    protected Coroutine RotateToTargetRoutine;

    // TODO: nanti apus ya
    public NavMeshAgent NavMeshAgent
    {
        get { return NMAgent; }
    }
    public Transform Target
    {
        get { return target; }
        set { target = value; }
    }

    public bool TargetInSight
    {
        get { return targetInSight; }
    }
    public bool TargetInRange
    {
        get { return targetInRange; }
    }

    public LayerMask IsTarget
    {
        set { isTarget = value; }
        get { return isTarget; }
    }

    public Vector3 WayPoint
    {
        set { wayPoint = value; }
        get { return wayPoint; }
    }

    public Vector3 DistanceToWayPoint
    {
        set { distanceToWayPoint = value; }
        get { return distanceToWayPoint; }
    }
    public bool WayPointSet
    {
        set { wayPointSet = value; }
        get { return wayPointSet; }
    }
    #endregion

    // = = = = = = = = = = = = = = = = = = = = = = = = = = = =

    public FloatingIndicator floatingIndicator;
    public ProgressBar healthBar;

    protected void InitHealthBar()
    {
        floatingIndicator = GetComponentInChildren<FloatingIndicator>();
        healthBar = GetComponentInChildren<ProgressBar>();
        healthBar.SetMaxValue(health);
        healthBar.SetValue(health);

    }

    public void UpdateHealthBar()
    {
        if(healthBar!=null) healthBar.SetValue(health);
    }

    #region Animation
    protected Animation animation;
    protected void InitAnimation()
    {
        animation = GetComponent<Animation>();

    }

    public abstract void StartWalkAnimation();

    public abstract void StartRunAnimation();

    public abstract void StartAttackAnimation();

    public abstract void StartIdleAnimation();

    public abstract void StartHurtAnimation();

    // boiler plate
    public void InvokeWalkAnimation()
    {
        Invoke(nameof(StartWalkAnimation), 0.1f);
    }

    public void InvokeRunAnimation()
    {
        Invoke(nameof(StartRunAnimation), 0.1f);
    }

    public void InvokeAttackAnimation()
    {
        Invoke(nameof(StartAttackAnimation), 0f);
    }

    public void InvokeIdleAnimation()
    {
        Invoke(nameof(StartIdleAnimation), 0.1f);
    }

    public void InvokeHurtAnimation()
    {
        Invoke(nameof(StartHurtAnimation), 0f);
    }


    #endregion

    public void CheckSphereAttack()
    {
        float radius = 15f;
        Collider[] colliderArray = Physics.OverlapSphere(transform.position, radius);
        foreach (var collider in colliderArray)
        {
            if (collider.TryGetComponent(out PlayerManager player))
            {
                player.TakeDamage(attackDmg);
                Debug.Log(player.Health);
            }
        }
    }
    #region NavMesh

    protected void InitNavMesh()
    {
        NMAgent = GetComponent<NavMeshAgent>();
        spawnPoint = new Vector3(transform.position.x, transform.position.y, z: transform.position.z);
        //NMAgent.updateRotation = false;
    }

    protected IEnumerator CheckTargetInSightRoutine()
    {
        while (true)
        {
            targetInSight = Physics.CheckSphere(transform.position, sightRange, isTarget);
            yield return new WaitForSeconds(0.2f);
        }
    }

    protected IEnumerator CheckTargetInRangeRoutine()
    {
        while (true)
        {
            targetInRange = Physics.CheckSphere(transform.position, attackRange, isTarget);
            yield return new WaitForSeconds(0.2f);
        }
    }

    // sisyphus
    public void SurfaceAlignment()
    {
        Ray ray = new Ray(transform.position, -transform.up);

        if (Physics.Raycast(ray, out RaycastHit hit, isGround))
        {
            // Adjust the look direction based on slope.
            float slopeAngle = Vector3.Angle(Vector3.up, hit.normal);
            Vector3 forwardOnSlope = Vector3.Cross(transform.right, hit.normal);
            Quaternion targetRotation = Quaternion.LookRotation(forwardOnSlope) * Quaternion.Euler(slopeAngle, 0, 0);

            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * 10f);  // Adjust speed as needed
        }
    }

    public IEnumerator RotateTowardsTarget(Transform target)
    {

        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        while (Quaternion.Angle(transform.rotation, targetRotation) > 0.05f)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, 5.0f * Time.deltaTime);
            yield return null;
        }


        transform.rotation = targetRotation;
    }

    #endregion

    public void InvokeResetAttackCooldown()
    {
        Invoke(nameof(ResetAttackCooldown), attackCd);
    }

    public void ResetAttackCooldown()
    {
        isAttackEnabled = true;
    }

    public void InitGameManager()
    {
        gameManager = GameManager.Instance;

    }
    public bool CheckInSpawnBounds()
    {
        //cek aja kalo target kejauhan biar ga bug, masih in bounds tapi ngeliat target
        if ((transform.position - spawnPoint).magnitude < walkRange) return true;
        return false;
    }

    public void StopAndIdle()
    {
        InvokeIdleAnimation();
        NMAgent.SetDestination(transform.position);
        NMAgent.isStopped = true;
        NMAgent.updateRotation = false;
    }

    public void Walk()
    {
        InvokeWalkAnimation();
        NMAgent.isStopped = false;
        NMAgent.updateRotation = true;
        NMAgent.speed = walkSpeed;
    }

    public void Run()
    {
        InvokeRunAnimation();
        NMAgent.isStopped = false;
        NMAgent.updateRotation = true;
        NMAgent.speed = chaseSpeed;
    }

    
    public bool CheckTag(GameObject tagged)
    {
        return tagged.CompareTag("Enemy");
    }

    #region Patrol

    protected Vector3 FindWayPoint()
    {
        float randomX = Random.Range(-walkRange, walkRange);
        float randomY = Random.Range(-walkRange, walkRange);

        Vector3 tentativeWayPoint = new Vector3(spawnPoint.x + randomX, transform.position.y, spawnPoint.z + randomY);

        NavMeshHit hit;
        if (!CheckInSpawnBounds()) return spawnPoint;
        if (NavMesh.SamplePosition(tentativeWayPoint, out hit, walkRange * 2, NavMesh.AllAreas))
        {
            wayPointSet = true;
            return hit.position;
        }
        return tentativeWayPoint;
    }

    #endregion


    #region Actions

    public void TakeDamage(float damage)
    {
        Debug.Log(gameObject.name + "health"+ health);
        health -= damage;
        if(NMAgent != null)NMAgent.isStopped = true;
        UpdateHealthBar();
        InvokeHurtAnimation();
        InvokeWalkAnimation();
    }

    public bool isInPlayerHitbox;
    #endregion

    #region AbstractMethods

    protected abstract void InitParams();

    protected abstract void ChildStart();

    protected abstract void ChildUpdate();

    protected abstract void ChildAwake();

    public abstract void Attack();

    #endregion

    #region UnityDefault

    private void Awake()
    {
        ChildAwake();
    }
    void Start()
    {
        ChildStart();
    }

    void Update()
    {
        //Debug.Log(gameObject.name + "health:" + health);
        ChildUpdate();
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.TryGetComponent(out PlayerManager player))
        {
            //Debug.Log(player.gameObject.name);

            isInPlayerHitbox = true;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.TryGetComponent(out PlayerManager player))
        {
            //Debug.Log("no more hitbox");
            isInPlayerHitbox = false;
        }
    }

    #endregion

    #region Gizmos
    //void OnDrawGizmos()
    //{
    //    if (Visualize)
    //    {
    //        //Gizmos.color = Color.red;
    //        //Gizmos.DrawSphere(wayPoint, 0.5f);

    //        //Gizmos.color = Color.blue;
    //        //Gizmos.DrawLine(transform.position, wayPoint);
    //        //UnityEditor.Handles.color = Color.green;
    //        //UnityEditor.Handles.DrawWireDisc(spawnPoint, Vector3.up, walkRange);
    //    }
    //}

    #endregion
}