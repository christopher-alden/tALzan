using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FirstPersonCamera : MonoBehaviour
{
    public Transform playerBody;
    public float mouseSensitivity = 100f;
    public float smoothTime = 0.125f;

    private float xRotation = 0f;
    private float currYVelocity = 0.0f;
    private float currXVelocity = 0.0f;
    private float smoothedX = 0f;
    private float smoothedY = 0f;

    private float amplitudeGain = 2.0f;
    private float frequency = 1.0f;

    public float bobbingTransitionDuration = 0.5f;
    public float fovTransitionDuration = 0.8f;

    private Coroutine bobbingRoutine;
    private Coroutine landingRoutine;

    private float targetFOV;
    private float initialFOV;
    private Coroutine fovTransitionCoroutine;
    [SerializeField] private AnimationCurve fovTransitionCurve;

    private CinemachineVirtualCamera cam;
    private CinemachineBasicMultiChannelPerlin noise;

    #region Singleton
    private static FirstPersonCamera instance;
    public static FirstPersonCamera Instance
    {
        get { return instance; }
    }
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    #endregion

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        cam = GetComponent<CinemachineVirtualCamera>();
        noise = cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
    }

    private void Update()
    {
        if (GameManager.Instance.isPaused) return;
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        smoothedX = Mathf.SmoothDamp(smoothedX, mouseX, ref currYVelocity, smoothTime);
        smoothedY = Mathf.SmoothDamp(smoothedY, mouseY, ref currXVelocity, smoothTime);

        xRotation -= smoothedY;
        xRotation = Mathf.Clamp(xRotation, -90f, 65f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        playerBody.Rotate(Vector3.up * smoothedX);
    }

    public void ChangeViewBobbing(bool runKey)
    {
        float targetFrequency = runKey ? 3f : frequency;
        float targetAmplitudeGain = runKey ? 2.5f : amplitudeGain;
        float transitionDuration = bobbingTransitionDuration;

        if (bobbingRoutine != null) StopCoroutine(bobbingRoutine);
        bobbingRoutine = StartCoroutine(BobTransition(targetFrequency, targetAmplitudeGain, transitionDuration));
    }

    private IEnumerator BobTransition(float targetFrequency, float targetAmplitudeGain, float transitionDuration)
    {
        float initialFrequency = noise.m_FrequencyGain;
        float initialAmplitudeGain = noise.m_AmplitudeGain;

        float elapsed = 0f;

        while (elapsed < transitionDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / transitionDuration;

            noise.m_FrequencyGain = Mathf.Lerp(initialFrequency, targetFrequency, t);
            noise.m_AmplitudeGain = Mathf.Lerp(initialAmplitudeGain, targetAmplitudeGain, t);

            yield return null; 
        }

        noise.m_FrequencyGain = targetFrequency;
        noise.m_AmplitudeGain = targetAmplitudeGain;
    }

    public void ChangeFOV(float fov)
    {

        if (fovTransitionCoroutine != null)
        {
            StopCoroutine(fovTransitionCoroutine);
        }

        targetFOV = fov;
        initialFOV = cam.m_Lens.FieldOfView;

        fovTransitionCoroutine = StartCoroutine(FOVTransition());
    }

    private IEnumerator FOVTransition()
    {
        float elapsedTime = 0f;

        while (elapsedTime < fovTransitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fovTransitionDuration);
            float currentFOV = Mathf.Lerp(initialFOV, targetFOV, fovTransitionCurve.Evaluate(t));
            cam.m_Lens.FieldOfView = currentFOV;
            yield return null;
        }

        cam.m_Lens.FieldOfView = targetFOV;
    }

    public void Shake(float endTimer)
    {
        Invoke(nameof(StartShake), 0f);
        Invoke(nameof(ResetShake), endTimer);
    }

    public void StartShake()
    {
        if (landingRoutine != null) StopCoroutine(landingRoutine);
        landingRoutine = StartCoroutine(BobTransition(6f, 6f, 0.6f));
    }

    public void ResetShake()
    {
        if (landingRoutine != null) StopCoroutine(landingRoutine);
        landingRoutine = StartCoroutine(BobTransition(frequency, amplitudeGain, 0.4f));
    }




}
