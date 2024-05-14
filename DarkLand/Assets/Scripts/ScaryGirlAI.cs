using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ScaryGirlAI : MonoBehaviour
{
    [SerializeField] private GameObject target;
    private NavMeshAgent navMeshAgent;
    private float defaultSpeed;
    private bool running = false;
    private Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        defaultSpeed = navMeshAgent.speed;
        animator = GetComponent<Animator>();
    }

    public void WakeUp(){
        animator.SetBool("Alive", true);
    }

    public void Scream(){
        Debug.Log("Chi gridata");
        GetComponent<AudioSource>().Play();
    }

    public void StartRunning(){
        running = true;
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Fuori");
        if (running){
            navMeshAgent.SetDestination(target.transform.position);
            Debug.Log("Dentro");
            if (navMeshAgent.isOnOffMeshLink){
                navMeshAgent.speed = 1.2f;
            }
            else{
                navMeshAgent.speed = defaultSpeed;
            }
        }
    }
}
