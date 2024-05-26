using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerGrapple : MonoBehaviour
{
    [Header("References")]
    private PlayerManager playerManager;
    private FirstPersonCamera fpsCamera;
    private GrapplingRope grapplingRope;
    [SerializeField] private Transform cam;
    public Transform gunTip;
    public LayerMask whatIsGrappleable;
    public LineRenderer lr;

    [Header("Grappling")]
    public float maxGrappleDistance;
    public float grappleDelayTime;
    public float overshootYAxis;

    private Vector3 grapplePoint;

    private bool isGrounded;

    public bool IsGrounded
    {
        set { isGrounded = value; }
    }

    public Vector3 GrapplePoint
    {
        get { return grapplePoint; }
    }
    private void Start()
    {
        playerManager = GetComponent<PlayerManager>();
        grapplingRope = GetComponentInChildren<GrapplingRope>();
        fpsCamera = FirstPersonCamera.Instance;
    }


    public bool IsGrappling()
    {
        return playerManager.ActiveGrapple;
    }

    public void Grapple(bool grappleKey, float grapplingCdTimer)
    {
        if (grappleKey) StartGrapple(grapplingCdTimer);
    }

    public void StartGrapple(float grapplingCdTimer)
    {
        if (grapplingCdTimer > 0) return;

        // called in checker in manager
        // playerManager.IsGrappleEnabled = false;

        playerManager.Freeze = true;

        float delay = isGrounded ? grappleDelayTime : 0.1f;

        RaycastHit hit;
        if (Physics.Raycast(cam.position, cam.forward, out hit, maxGrappleDistance, whatIsGrappleable))
        {
            grapplePoint = hit.point;
            Invoke(nameof(ThrowGrapple), delay);
        }
        else
        {
            grapplePoint = cam.position + cam.forward * maxGrappleDistance;
            Invoke(nameof(StopGrapple), delay);
        }

    }

    private void ThrowGrapple()
    {
        fpsCamera.Shake(1f);
        playerManager.Freeze = false;
        Vector3 lowestPoint = new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z);

        float grapplePointRelativeYPos = grapplePoint.y - lowestPoint.y;
        float highestPointOnArc = grapplePointRelativeYPos + overshootYAxis;

        if (grapplePointRelativeYPos < 0) highestPointOnArc = overshootYAxis;

        playerManager.JumpToPosition(grapplePoint, highestPointOnArc);
        
        Invoke(nameof(StopGrapple), 1f);
    }

    public void StopGrapple()
    {
        playerManager.StartGrappleCooldown();
        playerManager.Freeze = false;
    }
    
}
