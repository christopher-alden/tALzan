using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwables : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private int rotationSpeed;
    private Quaternion initialRotation;
    private Rigidbody rb;
    private bool targetHit;

    public int Damage
    {
        set { damage = value; }
        get { return damage; }
    }

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialRotation = transform.localRotation;
        Destroy(gameObject, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        float rotationDelta = rotationSpeed * Time.deltaTime;
        transform.Rotate(0f, 0f, rotationDelta);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (targetHit) return;
        else targetHit = true;

        if(collision.gameObject.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();
            if(enemy.CompareTag("Enemy")) enemy.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
