using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WildlifeState : IWildlifeState
{
    private EnemyWildlife wildlife;
    public WildlifeState(EnemyWildlife wildlife)
    {
        this.wildlife = wildlife;
    }

    public void OnDeath()
    {

        wildlife.SetState(new PetState(wildlife));
    }

    public void OnTargetDetected(Transform target)
    {
        wildlife.ChaseTarget(target);
    }

    public void OnNoTarget()
    {
        wildlife.Patrol();
    }

    public void OnTargetInRange()
    {
        if (wildlife.isInPlayerHitbox)
        { 
            wildlife.AttackTarget();
        }
    }

    public void UpdateState()
    {


        if (wildlife.TargetInRange && wildlife.IsAttackEnabled)
        {
            //reset = false;
            OnTargetInRange();
        }
        else if (wildlife.TargetInSight && wildlife.CheckInSpawnBounds())
        {
            //Debug.Log("insight aja");
            OnTargetDetected(wildlife.Target);
        }
        else
        {
            OnNoTarget();
        }

        if (wildlife.Health <= 0)
        {
   
            OnDeath();
        }
        
    }

   
}
