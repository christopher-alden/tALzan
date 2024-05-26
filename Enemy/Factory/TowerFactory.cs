using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerFactory : MonoBehaviour, IEnemyFactory
{
    public List<EnemyAssociation> AnimalPrefabs;


    public GameObject CreateEnemy<T>() where T : Enemy
    {
        string typeName = typeof(T).Name;

        foreach (var enemy in AnimalPrefabs)
        {
            if (enemy.enemyTypeName == typeName)
            {
                return GameObject.Instantiate(enemy.prefab);
            }
        }

        return null;
    }

    
}
