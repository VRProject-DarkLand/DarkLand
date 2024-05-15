using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class ScaryGirlAI : MonoBehaviour
{
    [SerializeField] private GameObject target;

    [SerializeField] private NavMeshSurface surface;
    private Vector3 spawnPosition;
    private NavMeshAgent navMeshAgent;
    private float defaultSpeed;
    private bool inSight;
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
        inSight = false;
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
        StartCoroutine(FollowMe());
    }

    private IEnumerator FollowMe(){
        navMeshAgent.SetDestination(spawnPosition);
        while(true){
            Debug.Log("YEAH");
            Vector3 startRaycast = gameObject.transform.position+new Vector3(0,1.5f,0);
            RaycastHit hit ;
            Vector3 direction = target.transform.position-startRaycast;
            //Debug.Log(Vector3.Distance(target.transform.position,transform.position));
            float distance = direction.magnitude;
            if(GameEvent.isHiding ){
                chasing = false;
            }
            if ( chasing && distance<15f){
                navMeshAgent.speed=2*defaultSpeed;
                navMeshAgent.SetDestination(target.transform.position);
                if (navMeshAgent.isOnOffMeshLink){
                    navMeshAgent.speed = 2.5f;
                }
                yield return null;
            }
            else if ( Physics.Raycast(startRaycast, direction, out hit, maxDistance )){
                Debug.DrawRay(startRaycast, direction, Color.yellow);
                if(hit.collider.gameObject.tag == Settings.PLAYER_TAG){
                    chasing = true;
                    Debug.Log("FIG");
                    navMeshAgent.speed=2*defaultSpeed;    
                    navMeshAgent.SetDestination(target.transform.position);
                    if (navMeshAgent.isOnOffMeshLink){
                        navMeshAgent.speed = 2.5f;
                    }
                    
                    yield return  new WaitForSeconds(0.5f);
                }else {
                    chasing = false;
                }
            }else{
                chasing = false;
            }
            if(!chasing){
                    //maybe sample position for random point in navmesh
                navMeshAgent.speed=defaultSpeed;
                if(Vector3.Distance(navMeshAgent.destination,transform.position)<3f){
                    Vector3 randomDirection = Random.insideUnitSphere * 25f;
                    randomDirection += transform.position;
                    NavMeshHit hit1;
                    NavMesh.SamplePosition(randomDirection, out hit1, 25f, ~LayerMask.GetMask("SpiderNavMesh"));
                    Vector3 finalPosition = hit1.position;
                    navMeshAgent.SetDestination(finalPosition);
                }
                if(distance<maxDistance)
                {
                    yield return new WaitForSeconds(0.2f);
                }else if(distance <2*maxDistance)
                    yield return new WaitForSeconds(3f);
                else 
                    yield return new WaitForSeconds(6f);
            }
        }   
    }
    // Update is called once per frame
    void Update()
    {
    }
}
