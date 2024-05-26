using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tower : MonoBehaviour
{
    private int towerHp;
    private GameManager gameManager;


    public int TowerHp
    {
        get { return towerHp; }
        set { towerHp = value; }
    }

    public void InitStat()
    {
        towerHp = 10;
    }

    public void MinusTowerHp()
    {
        towerHp -= 1;
        gameManager.UpdateTowerBar(towerHp);
    }

    public void Start()
    {
        gameManager = GameManager.Instance;
        InitStat();
        gameManager.InitTowerBar(towerHp);
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Enemy") && collision.TryGetComponent(out EnemyTowerFox towerFox))
        {
            Debug.Log("enter colli");
            MinusTowerHp();
            towerFox.DUAR();
        }
    }
}
