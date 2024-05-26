using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    #region dependencies

    private PlayerMovement playerMovement;
    private PlayerAnimation playerAnimation;
    private FirstPersonCamera fpsCamera;
    private PlayerThrowing playerThrowing;
    private PlayerGrapple playerGrapple;
    private PlayerHitbox playerHitbox;
    private GameManager gameManager;
    private PlayerClimbing playerClimbing;

    #endregion

    #region playerReferences

    [Header("Player Reference")]
    [SerializeField] private Transform cam;
    [SerializeField] private Rigidbody rb;
    private Animator animator;
    private Collider playerCollider;

    #endregion

    #region inputParameters

    private bool forwardKey;
    private bool backwardKey;
    private bool leftKey;
    private bool rightKey;
    private bool jumpKey;
    private bool runKey;
    private bool leftClick;
    private bool rightClick;
    private bool throwKey;
    private bool interactKey;

    #endregion

    #region movementParameters

    [Header("Movement")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float e;
    [SerializeField] private float jumpSpeed;
    private float runSpeed;
    private float walkSpeed;

    [Header("Ground")]
    [SerializeField] private LayerMask groundLayer;

    // --- status ---
    [SerializeField] private bool isGrounded;

    #endregion

    #region grappleParameters

    [Header("Cooldown")]
    [SerializeField] private float grappleCd;
    [SerializeField] private float grappleSpeed;
    private float grappleCdTimer;
    private bool isGrappleEnabled;
    private bool freeze;
    private bool activeGrapple;
    private bool enableMovementOnNextTouch;
    private Vector3 velocityToSet;


    public bool ActiveGrapple
    {
        get { return activeGrapple; }
    }
    public bool IsGrappleEnabled
    {
        get { return isGrappleEnabled; }
        set { isGrappleEnabled = value; }
    }

    public bool Freeze
    {
        get { return freeze; }
        set { freeze = value; }
    }

    #endregion

    #region status
    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;
    [SerializeField] protected float attackDmg;
    [SerializeField] protected float attackCd;
    [SerializeField] protected bool alreadyAttacked;
    [SerializeField] protected float xp;
    [SerializeField] protected float xpThreshold;
    [SerializeField] protected int level;

    public int Level
    {
        get { return level; }
        set { level = value; }
    }

    public float Xp
    {
        get { return xp; }
        set { xp = value; }
    }

    public float XpThreshold
    {
        get { return xpThreshold; }
        set { xpThreshold = value; }
    }

    public float MaxHealth
    {
        get { return maxHealth; }
        set { maxHealth = value; }
    }
    public float Health
    {
        get { return health; }
        set { health = value; }
    }

    #endregion


    // = = = = = = = = = = = = = = = = = = = = = = = = = = = =

    #region initializers

    private void InitMovement()
    {
        runSpeed = moveSpeed * 2;
        walkSpeed = moveSpeed;
        playerMovement = GetComponent<PlayerMovement>();
        playerMovement.Cam = cam;
        playerMovement.Target = rb;
        playerMovement.WalkSpeed = walkSpeed;
        playerMovement.RunSpeed = runSpeed;
        playerMovement.JumpSpeed = jumpSpeed;
        playerMovement.E = e;
        playerMovement.GroundLayer = groundLayer;
    }

    private void InitClimbing()
    {
        playerClimbing = GetComponent<PlayerClimbing>();
        playerClimbing.rb = rb;
        playerClimbing.orientation = cam.transform;
        //playerClimbing.whatIsWall = LayerMask.NameToLayer("Tree");
    }

    private void InitAnimation()
    {
        playerAnimation.Animator = animator;
        playerAnimation.IsGrounded = isGrounded;
    }

    private void InitGrapple()
    {
        enableMovementOnNextTouch = false;
    }

    private void InitStatus()
    {
        maxHealth = 200;
        health = 200;
        xp = 0;
        xpThreshold = 200;
        attackDmg = 50;
        attackCd = 0.5f;
        level = 1;

    }

    
    private void InitHitbox()
    {
        playerHitbox = PlayerHitbox.Instance;
        playerHitbox.AttackDamage = attackDmg;
        playerHitbox.AttackCd = attackCd;

    }

    #endregion
    
    #region statusStatus

    public void CheckLevelUp()
    {
        if(xp >= xpThreshold)
        {
            LevelUp();
            Debug.Log("new xp :" + xp + "| new xpThreshold: " + xpThreshold);
        }
    }

    public void LevelUp()
    {
        level++;
        xp -= xpThreshold;
        if (xp < 0) xp = 0;
        xpThreshold += 50;
        maxHealth += (20 * level);
        attackDmg += 5;
        health = maxHealth;
        gameManager.SetLevelText();
        gameManager.InitHealthBar(maxHealth);
        gameManager.UpdateHealthBar(health);
        gameManager.InitXpBar(xpThreshold);
        gameManager.UpdateXpBar(xp);
    }


    #endregion

    #region groundedStatus

    public void SetIsGrounded(bool value)
    {
        isGrounded = value;
        playerMovement.IsGrounded = value;
        playerAnimation.IsGrounded = value;
        playerGrapple.IsGrounded = value;
    }

    public bool IsGrounded()
    {
  
        float extraHeight = 0.1f;
        Vector3 bottomOfPlayer = new Vector3(transform.position.x, playerCollider.bounds.min.y - extraHeight, transform.position.z);

        bool grounded = Physics.CheckSphere(bottomOfPlayer, playerCollider.bounds.extents.x, 1 << 7);

        return grounded;
    }


    #endregion

    #region grappleStatus

    private void Grapple()
    {
        if (rightClick && isGrappleEnabled && !activeGrapple)
        {
            OnGrappleStart();
        }
        //if (activeGrapple)
        //{
        //    //playerAnimation.PlsStop();
        //    //playerAnimation.AnimateGrapple(activeGrapple);
        //}
        
    }

    public void CheckIsGrappleEnabled()
    {
        // cooldown is called in controller

        if (grappleCdTimer > 0)
        {
            grappleCdTimer -= Time.deltaTime;
            isGrappleEnabled = false;
        }
        else isGrappleEnabled = true;
    }

    private void CheckHasGrapplingEnded()
    {
        if (enableMovementOnNextTouch)
        {
            enableMovementOnNextTouch = false;
            playerGrapple.StopGrapple();
            OnGrappleEnd();
        }
    }

    #endregion

    #region dragStatus

    public void noDrag()
    {
        rb.drag = 0;
        rb.angularDrag = 0;
    }

    public void yesDrag()
    {
        rb.drag = 0.3f;
        rb.angularDrag = 0.5f;
    }

    #endregion

    #region attackStatus

    public void ResetAlreadyAttacked()
    {
        SetAlreadyAttacked(false);
    }

    public void SetAlreadyAttacked(bool value)
    {
        alreadyAttacked = value;
    }

    #endregion

    public void TakeDamage(float damage)
    {
        fpsCamera.Shake(0.7f);
        health -= playerHitbox.TakeDamage(damage);
        //// tembak ja deh
        Invoke(nameof(UpdateHealthBarWithDelay), 0.3f);
    }

    private void UpdateHealthBarWithDelay()
    {
        gameManager.UpdateHealthBar(health);
    }

    public Transform GetPosition()
    {
        return gameObject.transform;
    }

    protected bool isInteracting;
    public bool IsInteracting
    {
        get { return isInteracting; }
        set { isInteracting = value; }
    }
    void CheckIsInteracting()
    {
        if (interactKey) isInteracting = !isInteracting; 
    }

    // = = = = = = = = = = = = = = = = = = = = = = = = = = = =

    
    #region camera

    private void CheckViewBobbing()
    {
        //values taken from animator to make it sync
        bool toggle = animator.GetBool("isRunning");
        fpsCamera.ChangeViewBobbing(toggle);
    }

    #endregion

    #region grapple 

    public void StartGrappleCooldown()
    {
        // called outside of manager
        grappleCdTimer = grappleCd;
    }

    private void SetVelocity()
    {
        enableMovementOnNextTouch = true;

        rb.velocity = velocityToSet;

        // called here for the velocity feel

        fpsCamera.ChangeFOV(65);
    }

    public void JumpToPosition(Vector3 targetPosition, float trajectoryHeight)
    {
        activeGrapple = true;

        velocityToSet = playerMovement.CalculateJumpVelocity(transform.position, targetPosition, trajectoryHeight, grappleSpeed);

        Invoke(nameof(SetVelocity), 0.1f);
        
        Invoke(nameof(OnGrappleEnd), 3f);
    }

    public void OnGrappleStart()
    {
        noDrag();
        // fov change in controller if raycast hit
        // active grapple is on if the player is flying
        playerGrapple.StartGrapple(grappleCdTimer);
    }

    public void OnGrappleEnd()
    {
        // the cooldown and movement freezing is controlled
        // in the controller because state is unrelated
        // case freeze can be reused

        yesDrag();
        activeGrapple = false;
        fpsCamera.ChangeFOV(50);
        // grapple shake coroutine called in controller

    }

    #endregion

    #region climbing

    private void CheckClimbing()
    {
        playerClimbing.WallCheck();
        playerClimbing.ClimbCheck();
        if(isGrounded && !isFloating)
        {
            playerClimbing.ResetClimbTimer();
            return;
        }
        if (playerClimbing.climbing) playerClimbing.ClimbingMovement();
    }


    #endregion

    protected bool isFloating;
    protected float floatingThreshold = 1.6f;
    protected Coroutine FloatingRoutine;

    private void StartFloatingRoutine()
    {
        if (FloatingRoutine != null) return;
        else StartCoroutine(FloatingTimer());
    }

    private void CheckIsCharacterFloating()
    {
        if (!isGrounded)
        {
            StartFloatingRoutine();
        }
        else if(isGrounded && isFloating)
        {
            if (FloatingRoutine != null) StopCoroutine(FloatingRoutine);
            isFloating = false;
            OnCharacterLanding();
        }
    }

    private void OnCharacterLanding()
    {
        fpsCamera.Shake(0.25f);
    }

    public void InvokeShake(float endTimer)
    {
        fpsCamera.Shake(endTimer);
    }

    IEnumerator FloatingTimer()
    {
        float timer = 0.0f;

        while (true)
        {
            if (!isGrounded)
            {
                timer += Time.deltaTime;

                if (timer >= floatingThreshold)
                {
                    isFloating = true;
                }
            }
            else
            {
                timer = 0.0f;
            }

            
            yield return null; // Wait for the next frame.
        }
    }






    // = = = = = = = = = = = = = = = = = = = = = = = = = = = =

    #region inputHandler

    private void HandleInput()
    {
        forwardKey = Input.GetKey(KeyCode.W);
        backwardKey = Input.GetKey(KeyCode.S);
        leftKey = Input.GetKey(KeyCode.A);
        rightKey = Input.GetKey(KeyCode.D);
        jumpKey = Input.GetKeyDown(KeyCode.Space);
        runKey = Input.GetKey(KeyCode.LeftShift);
        throwKey = Input.GetKeyDown(KeyCode.X);
        interactKey = Input.GetKeyDown(KeyCode.F);
        leftClick = Input.GetMouseButtonDown(0);
        rightClick = Input.GetMouseButtonDown(1);
    }

    #endregion

    private void PerformChecks()
    {
        SetIsGrounded(IsGrounded());
        CheckIsGrappleEnabled();
        CheckClimbing();
        CheckViewBobbing();
        CheckIsCharacterFloating();
        CheckIsInteracting();
        CheckLevelUp();
    }

    void AttackHitCheck()
    {
        if (!alreadyAttacked)
        {
            float radius = 3.5f;
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, radius);
            foreach (var collider in colliderArray)
            {
                if (collider.TryGetComponent(out Enemy enemy))
                {
                    SetAlreadyAttacked(true);
                    if (enemy.gameObject.CompareTag("Enemy"))
                    {
                        enemy.TakeDamage(playerHitbox.InvokeAttack());
                        gameManager.NotifyPetsTarget(enemy);
                    }
                    Invoke(nameof(ResetAlreadyAttacked), attackCd);
                    Debug.Log(enemy.gameObject.name + "health:" + enemy.Health);
                }
            }
        }
    }

    private void CheckIsAlive()
    {
        if (health <= 0)
        {
            Debug.Log("Mati kau");
        }
    }
    
    #region unityDefault

    private void Start()
    {
        playerCollider = GetComponent<Collider>();
        playerMovement = GetComponent<PlayerMovement>();
        playerAnimation = GetComponent<PlayerAnimation>();
        fpsCamera = FirstPersonCamera.Instance;
        playerThrowing = GetComponentInChildren<PlayerThrowing>();
        playerGrapple = GetComponent<PlayerGrapple>();
        animator = GetComponent<Animator>();
        gameManager = GameManager.Instance;

        InitStatus();
        InitGrapple();
        InitMovement();
        InitClimbing();
        InitAnimation();
        InitHitbox();
        gameManager.InitHealthBar(maxHealth);
        gameManager.InitXpBar(XpThreshold);
        gameManager.UpdateXpBar(xp);
        gameManager.SetLevelText();

    }


    private void Update()
    {
        HandleInput();
        PerformChecks();
        Grapple();
        playerAnimation.AnimateGrapple(rightClick, isGrappleEnabled, activeGrapple);
        playerAnimation.AnimateFalling(isFloating, rightClick, isGrappleEnabled);

        if (freeze)
        {
            rb.velocity = Vector3.zero;
            return;
        }
        if (activeGrapple) return;
        playerMovement.MovementJump(jumpKey);

        playerAnimation.AnimateMovement(forwardKey, backwardKey, leftKey, rightKey, jumpKey, runKey);
        playerAnimation.AnimateCombat(leftClick);
        playerThrowing.Throw(throwKey);
        
    }

    private void FixedUpdate()
    {
        if (freeze)
        {
            rb.velocity = Vector3.zero;
            return;
        }
        if (activeGrapple) return;
        playerMovement.Movement(forwardKey, backwardKey, leftKey, rightKey, runKey);

    }


    private void OnCollisionEnter(Collision collision)
    {
        CheckHasGrapplingEnded();
    }

    #endregion

}
