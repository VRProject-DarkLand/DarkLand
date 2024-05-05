using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    [SerializeField] private Waypoints waypoints;
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float distanceThreshold = 0.1f;

    private bool alive = false;
    private Transform currentWaypoint = null;
    // Start is called before the first frame update
    void Start(){
        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        transform.position = currentWaypoint.position;
        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        GetComponent<Animator>().SetBool("Idle", true);
    }

    // Update is called once per frame
    void Update(){
        if (alive && currentWaypoint != null){
            GetComponent<Animator>().SetBool("Idle", false);
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position,currentWaypoint.position) < distanceThreshold){
                currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
                if (currentWaypoint != null){
                    transform.rotation = Quaternion.LookRotation(transform.position - currentWaypoint.position);
                }
            }
        }
        else{
            GetComponent<Animator>().SetBool("Idle", true);
            if(alive){
                transform.rotation = Quaternion.LookRotation(transform.position - GameObject.FindWithTag("Player").transform.position);
                transform.rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
            }
        }
    }
    public void WakeUp(){
        alive = true;
    }
}
