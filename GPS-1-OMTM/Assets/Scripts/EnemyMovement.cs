using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public float speed = 2f; // enemy movement speed
    public float detectionRange = 10f; // enemy detection range
    public float stoppingDistance = 1f; // distance which the enemy stops moving towards the target 
    public List<Transform> potentialTargets; // dist of potential targets (players, killdozer)

    private Transform closestTarget;

    void Update()
    {
        FindClosestTarget();
        MoveTowardsTarget();
    }

    void FindClosestTarget()
    {
        closestTarget = null; //start with no target
        float closestDistanceSqr = Mathf.Infinity;

        foreach (Transform target in potentialTargets) //for each target in the list
        {
            float distanceSqr = (target.position - transform.position).sqrMagnitude; //calculate distance between enemy position and target position
            if (distanceSqr < closestDistanceSqr && distanceSqr <= detectionRange * detectionRange) //if current target is closer than other targets and if target is within enemy detection range
            {
                //update closest target to current target
                closestDistanceSqr = distanceSqr;
                closestTarget = target;
            }
        }
    }

    void MoveTowardsTarget()
    {
        if (closestTarget != null) //if have a closest target
        {
            float distanceToTarget = Vector3.Distance(transform.position, closestTarget.position);

            if (distanceToTarget > stoppingDistance) //move if distance to the target is greater than stopping distance, otherwise stop
            {
                Vector3 direction = (closestTarget.position - transform.position).normalized;
                transform.position += direction * speed * Time.deltaTime;
            }
        }
    }

    // visualize enemy detection range in editor
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
