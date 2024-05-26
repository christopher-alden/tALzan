using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCManager : MonoBehaviour
{
    private bool isInRange;
    private bool playerWantsInteraction;
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
    }
    void CheckMauInteractGa()
    {

        if (gameManager.IsInWave) return;
        if (isInRange && playerWantsInteraction)
        {
            gameManager.StartTowerUIFadeIn();
            StartNewWave();
            gameManager.IsInWave = true;
        }

    }

    void StartNewWave()
    {
        Debug.Log("started new wave in 20 sec");
        gameManager.InvokeSpawnTowerEnemy();
    }

    // Update is called once per frame
    void Update()
    {
        CheckMauInteractGa();
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.TryGetComponent(out PlayerManager player))
        {
            isInRange = true;
            playerWantsInteraction = player.IsInteracting;
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") && collision.TryGetComponent(out PlayerManager player))
        {
            isInRange = false;
            player.IsInteracting = false;
            playerWantsInteraction = false;
        }
    }
}
