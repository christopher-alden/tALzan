using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FloatingIndicator : MonoBehaviour
{
    private Transform cam;
    [SerializeField] private GameObject healthUI;
    [SerializeField] private GameObject petIndicator;
    [SerializeField] private Transform player;
    [SerializeField] private float range = 5f;
    private bool UISwitch = false;

    private void Start()
    {
        cam = Camera.main.transform;
        player = GameManager.Instance.GetPlayerPosition();
        SetHealthVisible(true);
        SetPetVisible(false);
    }

    private void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - cam.transform.position);

        float distance = Vector3.Distance(player.position, transform.position);

        bool isInRange = distance <= range;
        SetVisible(UISwitch, isInRange);
    }
    public void SetVisible(bool UI, bool isVisible)
    {
        healthUI.SetActive(!UI && isVisible);
        petIndicator.SetActive(UI && isVisible);
    }


    public void SetHealthVisible(bool isVisible)
    {
        healthUI.SetActive(isVisible);
    }

    public void SetPetVisible(bool isVisible)
    {
        petIndicator.SetActive(isVisible);
    }

    public void SwitchInidcator()
    {
        UISwitch = !UISwitch;
    }
}
