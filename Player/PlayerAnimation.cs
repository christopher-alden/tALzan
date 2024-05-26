using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    [SerializeField] private bool isGrounded;
    bool isRunningHash;

    private float nextEnabledTime = 0.5f;
    public static int noOfClicks = 0;
    float lastClickedTime = 0;
    float maxComboDelay = 0.7f;

    public Animator Animator
    {
        get { return animator; }
        set { animator = value; }
    }
    public bool IsGrounded
    {
        get { return isGrounded; }
        set { isGrounded = value; }
    }

    private void Start()
    {
        isRunningHash = Animator.GetBool("isRunning");
    }


    public void AnimateMovement(bool forwardKey, bool backwardKey, bool leftKey, bool rightKey, bool jumpKey, bool runKey)
    {
        animator.SetBool("isWalking", forwardKey);

        if(runKey && forwardKey)
        {
            animator.SetBool("isRunning", true);
        }
        if ((runKey && !forwardKey) || !runKey)
        {
            animator.SetBool("isRunning", false);
        }
    }

    public void AnimateCombat(bool leftClick)
    {
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.6f && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            animator.SetBool("hit1", false);
        }
        if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.6f && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack2"))
        {
            animator.SetBool("hit2", false);
        }
        if (noOfClicks >= 2 && lastClickedTime>0.5f) noOfClicks = 0;
        if (Time.time - lastClickedTime > maxComboDelay)
        {
            noOfClicks = 0;
        }
        if (Time.time > nextEnabledTime)
        {
            if (leftClick)
            {
                onClick();

            }
        }
    }

    public void AnimateGrapple(bool rightClick, bool isGrappleEnabled, bool activeGrapple)
    {
        if (rightClick)
        {
            animator.SetBool("isGrappling", true);
            return;
            //PlsStop();
        }
        if (activeGrapple)
        {
            animator.SetBool("isGrappling", false);
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            return;
        }
        if(!activeGrapple && !isGrappleEnabled)
        {
            animator.SetBool("isGrappling", false);
        }
    }

    public void AnimateFalling(bool isFloating, bool rightClick, bool isGrappleEnabled)
    {
        if (isFloating)
        {
            animator.SetBool("isWalking", false);
            animator.SetBool("isRunning", false);
            if (rightClick && isGrappleEnabled)
            {
                animator.SetBool("isFloating", false);
                return;
            }
            animator.SetBool("isFloating", true);
        }
        else if (!isFloating)
        {
            animator.SetBool("isFloating", false);
            return;
        }
    }

    public void onClick()
    {
        lastClickedTime = Time.time;
        noOfClicks++;
        if (noOfClicks == 1)
        {
            animator.SetBool("hit1", true);
        }
        noOfClicks = Mathf.Clamp(noOfClicks, 0, 2);
        if (noOfClicks >= 2 && animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.4f && animator.GetCurrentAnimatorStateInfo(0).IsName("Attack1"))
        {
            animator.SetBool("hit1", false);
            animator.SetBool("hit2", true);
        }
    }

    public void PlsStop()
    {
        animator.SetBool("isRunning", false);
        animator.SetBool("isWalking", false);
        animator.SetBool("hit1", false);
        animator.SetBool("hit2", false);
    }

}
