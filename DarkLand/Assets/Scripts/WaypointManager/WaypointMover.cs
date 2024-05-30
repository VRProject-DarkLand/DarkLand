using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class WaypointMover : MonoBehaviour
{
    [SerializeField] private Waypoints waypoints;
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float distanceThreshold = 0.1f;
    [SerializeField] private float attackThreshold = 2.5f;
    private NavMeshAgent navMeshAgent;
    [SerializeField] private GameObject target;
    private Animator animator;
    private bool alive = false;

    private bool isAttacking = false;
    [SerializeField] private int attackDamage = 20;
    private Transform currentWaypoint = null;
    // Start is called before the first frame update
    void Start(){
        animator = GetComponentInChildren<Animator>();
        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        transform.position = currentWaypoint.position;
        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
       animator.SetBool("Idle", true);
    }

    // Update is called once per frame
    void Update(){
        if (alive && currentWaypoint != null){
           animator.SetBool("Idle", false);
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position,currentWaypoint.position) < distanceThreshold){
                currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
                if (currentWaypoint != null){
                    transform.LookAt(currentWaypoint);
                }
                else
                {
                    navMeshAgent = gameObject.AddComponent<NavMeshAgent>();
                    navMeshAgent.acceleration = 30;
                    navMeshAgent.stoppingDistance = 0.5f;
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
               animator.SetBool("Idle", false);
                RotateTowardsDestination();
                if(Vector3.Distance(target.transform.position, transform.position) < attackThreshold && !isAttacking){
                    StartCoroutine(Attack());
                }
            }
            else
            {
               animator.SetBool("Idle", true);
            }
        }
    }

    private IEnumerator Attack(){
        animator.SetBool("Attack", true);
        navMeshAgent.speed = 0;
        isAttacking = true;
        yield return new WaitForSeconds(0.75f);
        RaycastHit hit;
        if(Physics.SphereCast(transform.position, 1.2f ,transform.forward , out hit, attackThreshold, 63 )){
            if(hit.collider.gameObject == target)
            {
                Debug.Log("Ti scasciai "+Time.frameCount );
                hit.collider.gameObject.SendMessage("Hurt", attackDamage, SendMessageOptions.DontRequireReceiver);
            }
        }
        navMeshAgent.speed = moveSpeed;
        yield return new WaitForSeconds(3f);
        animator.SetBool("Attack", false);

        isAttacking = false;
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