using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingBook : MonoBehaviour
{
    public float floatStrength = 1f;
    public float floatFrequency = 1.5f;
    public float rotationSpeed = 30f;

    private Vector3 initialPosition;
    private Quaternion initialRotation;

    private void Start()
    {
        initialPosition = transform.position;
        initialRotation = transform.localRotation;
    }

    private void Update()
    {

        float newY = initialPosition.y + (Mathf.Sin(Time.time * floatFrequency) * floatStrength);
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);


        float rotationAngle = Time.time * rotationSpeed;
        Quaternion targetRotation = initialRotation * Quaternion.Euler(0f, 0f, rotationAngle);
        transform.localRotation = Quaternion.RotateTowards(transform.localRotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}
