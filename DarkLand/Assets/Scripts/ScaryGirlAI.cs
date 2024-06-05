using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class ScaryGirlAI : MonoBehaviour, IDataPersistenceSave{
    [System.Serializable]
    public class ScaryGirlSavingData {
        public Vector3 position;
        public Vector3 rotation;
        public bool dead;
        public bool awaken;
    }
    private GameObject target;

    [SerializeField] private NavMeshSurface surface;
    private Vector3 spawnPosition;
    private NavMeshAgent navMeshAgent;
    private float defaultSpeed;
    [SerializeField] private float attackThreshold = 2.5f;
    private bool inSight = false;
     bool isAttacking = false;
    [SerializeField] private  bool chasing = false;
    [SerializeField] private float maxDistance = 10f;
    private Animator animator;
    [SerializeField] private int attackDamage = 60;
    private bool hitByTeddy = false;
    private bool dead = false;
    //save if the enemy has been awaken s.t. upon spawning
    //it can be activated without using the trigger
    private bool awaken = false;
    
    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag(Settings.PLAYER_TAG);
        spawnPosition = gameObject.transform.position;
        inSight = false;
       
    }

    public void WakeUp(){
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = true;
        defaultSpeed = navMeshAgent.speed;
        animator = GetComponent<Animator>();
        animator.enabled = true;
        animator.SetBool("Alive", true);
        awaken = true;
    }

    public void Scream(){
        GetComponent<AudioSource>().Play();
    }

    public void StartRunning(){
        chasing = true;
        inSight = true;
        GameEvent.chasingSet.Add(GetInstanceID());
        StartCoroutine(FollowMe());
    }

private IEnumerator FollowMe(){
        navMeshAgent.SetDestination(spawnPosition);
        while(!dead){
            if (hitByTeddy)
            {
                navMeshAgent.SetDestination(transform.position);
                animator.SetBool("HitByTeddy", true);
                yield return new WaitForSeconds(6f);
                hitByTeddy = false;
                animator.SetBool("HitByTeddy", false);
                if (dead) break;
            }
            Vector3 startRaycast = gameObject.transform.position+new Vector3(0,1.5f,0);
            RaycastHit hit;
            Vector3 direction = target.transform.position-startRaycast;
            //Debug.Log(Vector3.Distance(target.transform.position,transform.position));
            float distance = direction.magnitude;
            if(GameEvent.isHiding){
                if(!inSight)
                    chasing = false;
                else{ 
                    if(distance  < attackThreshold && !isAttacking){
                        StartCoroutine(Attack());
                    }
                    navMeshAgent.SetDestination(target.transform.position);
                    yield return null;
                }
            }else{
                if ( Physics.Raycast(startRaycast, direction, out hit, maxDistance )){
                    Debug.DrawRay(startRaycast, direction, Color.yellow);
                    if(hit.collider.gameObject.tag == Settings.PLAYER_TAG){
                        inSight = true;
                        chasing = true;
                        if(distance  < attackThreshold && !isAttacking){
                            StartCoroutine(Attack());
                        }
                        GameEvent.chasingSet.Add(GetInstanceID());
                        navMeshAgent.speed=2*defaultSpeed;    
                        navMeshAgent.SetDestination(target.transform.position);
                        if (navMeshAgent.isOnOffMeshLink){
                            navMeshAgent.speed = 2.5f;
                        }
                        
                        yield return new WaitForSeconds(0.2f);
                    }else {
                        inSight = false;
                        if ( chasing && distance<10f){
                            navMeshAgent.speed=2*defaultSpeed;
                            navMeshAgent.SetDestination(target.transform.position);
                            if (navMeshAgent.isOnOffMeshLink){
                                navMeshAgent.speed = 2.5f;
                            }
                            yield return null;
                        }else 
                            chasing = false;
                    }
                }else{
                    chasing = false;
                }
            }
          
            if(!chasing){
                GameEvent.chasingSet.Remove(GetInstanceID());
                //maybe sample position for random point in navmesh
                navMeshAgent.speed=defaultSpeed;
                if(Vector3.Distance(navMeshAgent.destination,transform.position)<2.5f){
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
            }else{
               
            }
        }
        yield return null;
    }

    private IEnumerator Attack(){
       
        animator.SetBool("Attack", true);
        isAttacking = true;
        yield return new WaitForSeconds(0.2f);
        RaycastHit hit;
        if(Physics.SphereCast(transform.position, 0.65f ,transform.forward , out hit, attackThreshold, 63 )){
            if(hit.collider.gameObject == target)
            {
                Debug.Log("Ti scasciai "+Time.frameCount );
                hit.collider.gameObject.SendMessage("Hurt", attackDamage, SendMessageOptions.DontRequireReceiver);
            }
        }
        yield return new WaitForSeconds(0.6f);
        animator.SetBool("Attack", false);
        yield return new WaitForSeconds(0.5f);
        

        isAttacking = false;
    }
     public void HitByTeddyBear()
    {
        hitByTeddy = true;
        Debug.Log("Hit by teddy");
    }

    public void Die()
    {
        animator.SetBool("Dead", true);
        dead = true;
        
    }
    // Update is called once per frame
    void Update()
    {
    }

    public void SaveData(){
        ScaryGirlSavingData data = new ScaryGirlSavingData();
        data.position = transform.position;
        data.rotation = transform.localEulerAngles;
        data.dead = dead;
        data.awaken = awaken;
        Settings.gameData.scaryGirlsData.Add(data); 
    }
    public void LoadFromData(ScaryGirlSavingData data ){
        transform.parent.position = data.position;
        transform.parent.localEulerAngles = data.rotation;
        dead = data.dead;  
        awaken = data.awaken;
        if(dead)
            Destroy(this.transform.parent.gameObject);
        else{
            if(awaken)
                WakeUp();
        }
    
    }
}
