using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AStar : MonoBehaviour
{
    PathFinder pathFinder;
    public int targetIndex;
    public Vector3[] currentPath;
    private Transform enemyTransform;
    public float moveSpeed = 1f;
    

    public void Go(Node.Quadrant quadrant)
    {
        pathFinder = PathFinder.Instance;
        enemyTransform = transform;

        StartFollowingPath(pathFinder.QuadrantWaypoints[quadrant]);
    }

    public void StartFollowingPath(Vector3[] path)
    {
        if (path == null || path.Count() == 0) return;

        currentPath = path;
        
        targetIndex = 0;

        
        StartCoroutine(FollowPath());
    }

    IEnumerator FollowPath()
    {
        Vector3 currentWaypoint = currentPath[targetIndex];
        float rotationSpeed = 3f;

        while (true)
        {
            
            Quaternion targetRotation = Quaternion.LookRotation(currentWaypoint - enemyTransform.position);

           
            enemyTransform.rotation = Quaternion.Slerp(enemyTransform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            if (Vector3.Distance(enemyTransform.position, currentWaypoint) < 0.1f)
            {
                targetIndex++;
                if (targetIndex >= currentPath.Count()) yield break;

                currentWaypoint = currentPath[targetIndex];
            }

            enemyTransform.position = Vector3.MoveTowards(enemyTransform.position, currentWaypoint, moveSpeed * Time.deltaTime);
            yield return null;
        }
    }

}
