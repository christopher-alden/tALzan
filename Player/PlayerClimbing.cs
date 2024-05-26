using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClimbing : MonoBehaviour
{
    [Header("References")]
    public Transform orientation;
    public Rigidbody rb;
    public LayerMask whatIsWall;


    [Header("Climbing")]
    public float climbSpeed;
    public float maxClimbTime;
    public float climbTimer;

    public bool climbing;

    [Header("Detection")]
    public float detectionLength;
    public float sphereCastRadius;
    public float maxWallLookAngle;
    public float wallLookAngle;

    private RaycastHit frontWallHit;
    public bool wallFront;

    public void ClimbCheck()
    {
        if (wallFront && Input.GetKey(KeyCode.Space) && wallLookAngle < maxWallLookAngle)
        {
            if (!climbing && climbTimer > 0) StartClimbing();
            if (climbTimer > 0) climbTimer -= Time.deltaTime;
            if (climbTimer < 0) StopClimbing();

           
        }
        else
        {
            if (climbing) StopClimbing();
        }
    }


    public void ResetClimbTimer()
    {
        climbTimer = maxClimbTime;
    }
    public void WallCheck()
    {
        Vector3 offsetStartPosition = transform.position - orientation.forward * 0.1f;
        wallFront = Physics.SphereCast(offsetStartPosition, sphereCastRadius, orientation.forward, out frontWallHit, detectionLength + 0.1f, whatIsWall);


        if (wallFront)
        {
            wallLookAngle = Vector3.Angle(orientation.forward, -frontWallHit.normal);
        }
        else
        {
            wallLookAngle = 0f;
        }
    }


    public void StartClimbing()
    {
        climbing = true;

    }

    


    public void ClimbingMovement()
    {
        rb.velocity = new Vector3(rb.velocity.x, climbSpeed, rb.velocity.z);
    }

    public void StopClimbing()
    {
        climbing = false;

    }
}
