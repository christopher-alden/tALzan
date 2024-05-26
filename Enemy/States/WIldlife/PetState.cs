using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PetState : IWildlifeState
{
    private EnemyWildlife wildlife;
    
    public PetState(EnemyWildlife wildlife)
    {
        this.wildlife = wildlife;
        wildlife.gameObject.tag = "Pets";
        wildlife.gameObject.layer = LayerMask.NameToLayer("Pets");
        wildlife.Health = 200;
        wildlife.Owner = wildlife.GameManager.GetPlayerPosition();
        wildlife.Target = null;
        wildlife.IsTarget = LayerMask.NameToLayer("Enemy");
        wildlife.WayPointSet = false;
        wildlife.InvokeWalkAnimation();
        wildlife.GameManager.OnKill(wildlife.Health);
        wildlife.GameManager.PetCount++;
        wildlife.GameManager.SetPetText();
        wildlife.GameManager.AddPet(this.wildlife);
        wildlife.UpdateHealthBar();
        wildlife.floatingIndicator.SwitchInidcator();
    }
    //target will be enemy
    //owner will be tarzan
    public void OnDeath()
    {
        GameObject.Destroy(wildlife.gameObject);
    }

    public void OnTargetDetected(Transform target)
    {
        wildlife.ChaseTarget(target);
    }

    public void OnNoTarget()
    {
        wildlife.FollowOwner();
    }

    public void OnTargetInRange()
    {
        //Debug.Log("masuk on range");
        wildlife.AttackInProgress = true;
        wildlife.PetAttackTarget();
    }

    public void UpdateState()
    {
        wildlife.CheckStillTarget();
        //Debug.Log("remaining distance: " + wildlife.NavMeshAgent.remainingDistance);
        if (GameManager.Instance.IsInWave && GameManager.Instance.towerEnemyList.Count>0)
        {
            wildlife.Target = GameManager.Instance.towerEnemyList[0].transform;
        }
        if (!wildlife.AttackInProgress && wildlife.Target != null && wildlife.NavMeshAgent.remainingDistance < 20f)
        {
            OnTargetInRange();
        }
        else if (wildlife.Target != null)
        {
            OnTargetDetected(wildlife.Target);
        }
        else if(wildlife.Target == null)
        {
            OnNoTarget();
        }
    }

    
}
