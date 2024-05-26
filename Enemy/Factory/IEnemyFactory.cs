using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyFactory
{
    GameObject CreateEnemy<T>() where T : Enemy;
}
