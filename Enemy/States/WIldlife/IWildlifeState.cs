using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IWildlifeState
{
    void UpdateState();

    // dont forget for death animation
    // switch to pet
    void OnDeath();

    // make a manager that returns the location of the enemy
    void OnTargetDetected(Transform target);

    //attack
    void OnTargetInRange();

    // patrol
    void OnNoTarget();
}
