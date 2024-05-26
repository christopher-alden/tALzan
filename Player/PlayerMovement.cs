using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    #region parameters

    // --- playerReference ---
    private Transform cam;
    private Rigidbody target;

    // --- movementParameters ---
    private float jumpSpeed;
    private float verticalSpeed;
    private float runSpeed;
    private float walkSpeed;
    private float e = 1f;

    public bool isGrounded;

    private LayerMask groundLayer;

    private string collidedObjectTag;

    #endregion


    #region setterGetters

    public Transform Cam
    {
        get { return cam; }
        set { cam = value; }
    }
    public Rigidbody Target
    {
        get { return target; }
        set { target = value; }
    }
    public float JumpSpeed
    {
        get { return jumpSpeed; }
        set { jumpSpeed = value; }
    }
    public float RunSpeed
    {
        get { return runSpeed; }
        set { runSpeed = value; }
    }
    public float WalkSpeed
    {
        get { return walkSpeed; }
        set { walkSpeed = value; }
    }
    public float E
    {
        get { return e; }
        set { e = value; }
    }
    public bool IsGrounded
    {
        get { return isGrounded; }
        set { isGrounded = value; }
    }

    public LayerMask GroundLayer
    {
        get { return groundLayer; }
        set { groundLayer = value; }
    }
    public string CollidedObjectTag
    {
        get { return collidedObjectTag; }
        set { collidedObjectTag = value; }
    }

    #endregion

    public void Movement(bool forwardKey, bool backwardKey, bool leftKey, bool rightKey, bool runKey)
    {
        float characterSpeed = runKey ? runSpeed : walkSpeed;

        float x = 0f;
        float y = 0f;

        if (forwardKey) y = 1f;
        if (backwardKey) y = -1f;
        if (leftKey) x = -1f;
        if (rightKey) x = 1f;

        Vector3 forward = Vector3.ProjectOnPlane(Camera.main.transform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.Cross(Vector3.up, forward);

        if (!isGrounded)
        {
            target.AddForce(Mathf.Abs(Physics.gravity.y) * Time.deltaTime * Vector3.down, ForceMode.Acceleration);
        }

        Vector3 desiredMoveDirection = forward * y + right * x;
        desiredMoveDirection.y = 0;

        Vector3 counterMovement = new Vector3(-target.velocity.x, 0, -target.velocity.z) * e;
        target.AddForce(desiredMoveDirection * characterSpeed + counterMovement);
    }

    public void MovementJump(bool jumpKey)
    {
        if (jumpKey && isGrounded)
        {
            target.AddForce(Vector3.up * jumpSpeed, ForceMode.Impulse);
            isGrounded = false;
        }
    }

    public void MovementClimb()
    {

    }

    

    public Vector3 CalculateJumpVelocity(Vector3 startPoint, Vector3 endPoint, float trajectoryHeight, float grappleSpeed)
    {
        float gravity = Physics.gravity.y;
        float displacementY = endPoint.y - startPoint.y;
        Vector3 displacementXZ = new Vector3(endPoint.x - startPoint.x, 0f, endPoint.z - startPoint.z);

        float horizontalSpeed = displacementXZ.magnitude / (Mathf.Sqrt(-2 * trajectoryHeight / gravity)
            + Mathf.Sqrt(2 * (displacementY - trajectoryHeight) / gravity));

        horizontalSpeed *= grappleSpeed;

        Vector3 directionXZ = displacementXZ.normalized;
        Vector3 velocityXZ = directionXZ * horizontalSpeed;

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * trajectoryHeight);

        return velocityXZ + velocityY;
    }


}
