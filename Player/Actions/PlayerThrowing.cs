using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerThrowing : MonoBehaviour
{
    #region parameters

    [Header("Refs")]
    [SerializeField] private Transform cam;
    [SerializeField] private Transform attackPoint;
    [SerializeField] private GameObject throwable;

    [Header("Settings")]
    [SerializeField] private int totalThrows;
    [SerializeField] private float throwCd;

    [Header("Throwing")]
    [SerializeField] private float throwForce;
    [SerializeField] private float throwUpwardForce;


    public bool ready;

    #endregion

    private void Start()
    {
        ready = true;
    }

    public void Throw(bool throwKey)
    {
        if(throwKey && ready && totalThrows > 0)
        {
            InitiateThrow();
        }
    }

    

    private void InitiateThrow()
    {
        ready = false;

        GameObject projectile = Instantiate(throwable, attackPoint.position, cam.rotation);

        Rigidbody projectileRb = projectile.GetComponent<Rigidbody>();

        Vector3 projectileForce = cam.transform.forward * throwForce + transform.up * throwUpwardForce;
        projectileRb.AddForce(projectileForce, ForceMode.Impulse);

        totalThrows--;

        Invoke(nameof(ResetThrow), throwCd);

    }

    private void ResetThrow()
    {
        ready = true;
    }
}
