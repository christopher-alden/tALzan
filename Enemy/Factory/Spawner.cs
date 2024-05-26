using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private IEnemyFactory enemyFactory;

    private void Start()
    {
        enemyFactory = GetComponent<TowerFactory>();
    }

    public void SpawnFox(Node.Quadrant quadrant)
    {
        GameObject fox = enemyFactory.CreateEnemy<EnemyTowerFox>();
        GameManager.Instance.AddToTowerEnemyList(fox);
        fox.transform.position = transform.position;
        if (!fox.TryGetComponent<EnemyTowerFox>(out var foxClass)) Debug.Log("ini null");
        else foxClass.Units.Go(quadrant);
    }
}

