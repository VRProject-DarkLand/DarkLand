using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ScaryGirlAI : MonoBehaviour
{
    [SerializeField] private GameObject target;
    private Vector3 spawnPosition;
    private NavMeshAgent navMeshAgent;
    private float defaultSpeed;
    [SerializeField] private  bool chasing = false;
    [SerializeField] private float maxDistance = 30f;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        defaultSpeed = navMeshAgent.speed;
        animator = GetComponent<Animator>();
        spawnPosition = gameObject.transform.position;
    }

    public void WakeUp(){
        animator.SetBool("Alive", true);
    }

    public void Scream(){
        Debug.Log("Chi gridata");
        GetComponent<AudioSource>().Play();
    }

    public void StartRunning(){
        chasing = true;
    }


    // Update is called once per frame
    void Update()
    {
        RaycastHit hit ;
        Vector3 direction = target.transform.position-transform.position;
        Debug.Log(Vector3.Distance(target.transform.position,transform.position));
        if(GameEvent.isHiding){
            chasing = false;
        }
        if (chasing && Vector3.Distance(target.transform.position,transform.position)<15f){
            navMeshAgent.speed=2*defaultSpeed;
            navMeshAgent.SetDestination(target.transform.position);
            if (navMeshAgent.isOnOffMeshLink){
                navMeshAgent.speed = 2.5f;
            }
        }
        else if (Physics.Raycast(transform.position, direction, out hit, maxDistance )){
            if(hit.collider.gameObject.tag == Settings.PLAYER_TAG){
                chasing = true;
                navMeshAgent.speed=2*defaultSpeed;    
                navMeshAgent.SetDestination(target.transform.position);
                if (navMeshAgent.isOnOffMeshLink){
                    navMeshAgent.speed = 2.5f;
                }
            }else {
                chasing = false;
            }
        }
        if(!chasing){
                //maybe sample position for random point in navmesh
            navMeshAgent.speed=defaultSpeed;
            navMeshAgent.SetDestination(spawnPosition);
        }
    }
}
