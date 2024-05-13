using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointMover : MonoBehaviour
{
    [SerializeField] private Waypoints waypoints;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float distanceThreshold = 0.1f;
    private NavMeshAgent navMeshAgent;
    [SerializeField] private GameObject target;

    private bool alive = false;
    private Transform currentWaypoint = null;
    // Start is called before the first frame update
    void Start(){
        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        transform.position = currentWaypoint.position;
        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        GetComponentInChildren<Animator>().SetBool("Idle", true);
    }

    // Update is called once per frame
    void Update(){
        if (alive && currentWaypoint != null){
            GetComponentInChildren<Animator>().SetBool("Idle", false);
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position,currentWaypoint.position) < distanceThreshold){
                currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
                if (currentWaypoint != null){
                    transform.LookAt(currentWaypoint);
                }
                else
                {
                    navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
                    navMeshAgent.radius = 1;
                    navMeshAgent.height = 1;
                    navMeshAgent.speed = 3f;
                }
            }
        }
        else{
            //GetComponent<Animator>().SetBool("Idle", true);
            if (alive && IsInsideNavMesh(target.transform.position)){
                navMeshAgent.SetDestination(target.transform.position);
                NavMeshPath path = new NavMeshPath();
                navMeshAgent.CalculatePath(target.transform.position, path);
                Debug.Log(path.status);
                GetComponentInChildren<Animator>().SetBool("Idle", false);
                RotateTowardsDestination();
            }
            else
            {
                GetComponentInChildren<Animator>().SetBool("Idle", true);
            }
        }
    }
    public void WakeUp(){
        alive = true;
    }
    private bool IsInsideNavMesh(Vector3 position)
    {
        NavMeshHit hit;

        // Sample a position on the NavMesh closest to the given position
        if (NavMesh.SamplePosition(position, out hit, Mathf.Infinity, NavMesh.AllAreas))
        {
            // Check if the sampled position is close enough to the original position
            return Vector3.Distance(position, hit.position) < 5f;
        }

        return false;
    }
    private void RotateTowardsDestination()
    {
        // Calculate the direction vector from the current position to the target position
        Vector3 direction = (navMeshAgent.destination - transform.position).normalized;

        // If the direction vector is not zero (avoid division by zero)
        if (direction != Vector3.zero)
        {
            // Calculate the rotation to look towards the destination
            Quaternion rotation = Quaternion.LookRotation(direction);

            // Smoothly rotate towards the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 0.3f);
        }
    }
}