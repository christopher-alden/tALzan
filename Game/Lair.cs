using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lair : MonoBehaviour
{
    [SerializeField] private EnemyDragon dragon;
    GameManager gameManager;


    private void InitSemuanyaAjaYa()
    {
        gameManager = GameManager.Instance;
    }

    private void OnTriggerStay(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player") )
        {
            dragon.Target = gameManager.GetPlayerPosition();
            gameManager.StartBossUIFadeIn();
        }
    }
    private void OnTriggerExit(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Debug.Log("Exit");
            dragon.Target = null;
            gameManager.StartBossUIFadeOut();
        }
    }

    private void Start()
    {
        InitSemuanyaAjaYa();
        gameManager.InitBossBar(dragon.Health);
    }
}
