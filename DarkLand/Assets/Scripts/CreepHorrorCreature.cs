using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class CreepHorrorCreature : MonoBehaviour, IDamageableEntity
{
    // Start is called before the first frame update
      #region SerializeField
        [SerializeField] private AudioClip attackingSound;
        [SerializeField] private AudioClip stepSound;
        [SerializeField] private AudioClip scream;
        [SerializeField] private AudioClip deathSound;
        [SerializeField] private float attackThreshold = 3.5f;
        [SerializeField] private float maxDistance = 2000f;
        //save if the enemy has been awaken s.t. upon spawning
        [SerializeField] private FinalBossTrigger finalBossTrigger;
        [SerializeField] private int attackDamage = 60;
    #endregion
    
    #region PrivateAttributes
        private FPSInput target;
        private AudioSource audioSource;
        private Vector3 spawnPosition;
        private NavMeshAgent navMeshAgent;
        private float defaultSpeed;
        private Animator animator;
        private bool dead = false;
        //it can be activated without using the trigger
        private bool awaken = false;
        private bool charging = false;
        private bool charged = true;
        private bool running = false;
        private bool isVulnerable = false;
        private int health = 100;
        private Vector3 freezeTargetPosition; 
        [SerializeField] private GameObject item;
        private float _footStepSoundLength = 0.4f;
        private bool _step = true;
        private bool freeze = true;
        private Coroutine FollowMeCoroutine; 
        private Material creepMaterial;

    #endregion

    bool isAttacking = false;
    List<FinalBossTrigger> finalBossTriggers = new List<FinalBossTrigger>();

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag(Settings.PLAYER_TAG).GetComponent<FPSInput>();
        spawnPosition = gameObject.transform.position;
        creepMaterial = GetComponentInChildren<Renderer>().material;
        audioSource = GetComponent<AudioSource>();
        if(!Settings.LoadedFromSave){
            finalBossTriggers.Add(finalBossTrigger);
            ActivateNavMeshAndAnimator();
        }
    }

    private void ActivateNavMeshAndAnimator(){
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = true;
        defaultSpeed = navMeshAgent.speed;
        animator = GetComponent<Animator>();
        animator.enabled = true;
    }


    public void Roar(){
        //2D sound, distance does not matter
        Managers.AudioManager.PlaySound(scream);
    }


    public void WakeUp(){
        animator.SetBool("Alive", true);
        awaken = true;
    }

    public void Scream(){
        //2D sound, distance does not matter
        Managers.AudioManager.PlaySound(scream);
    }

    public void StartChasing(){
        animator.SetBool("Walk", false);
        animator.SetBool("Run", false);
        freeze = true;
        Managers.Player.AddEnemy(GetInstanceID(), 0);
        FollowMeCoroutine = StartCoroutine(FollowMe());
    }

    private IEnumerator ChargingRun(){
        yield return new WaitForSeconds(25f);
        charged = true;
    }

    private void Freeze(){
            navMeshAgent.SetDestination(transform.position);
            animator.SetBool("Walk", false);
            animator.SetBool("Attack", false);
            animator.SetBool("Run", false);
            freeze = true;
    }

    private void Run(bool reachable){
            Vector3 direction = target.transform.position- gameObject.transform.position;
            Vector3 freezeTargetPosition = target.transform.position+direction.normalized*8;
            animator.SetBool("Walk", false);
            animator.SetBool("Attack", false);
            animator.SetBool("Run", true);
            running = true;
            navMeshAgent.speed=6*defaultSpeed; 
            if(reachable)
                navMeshAgent.SetDestination(freezeTargetPosition);
            else 
                navMeshAgent.SetDestination(spawnPosition);
            
            freeze = false;
            _footStepSoundLength = 0.2f;
    }

    private void Walk(bool reachable){
        animator.SetBool("Walk", true);
        animator.SetBool("Attack", false);
        animator.SetBool("Run", false);
        isVulnerable = false;
        creepMaterial.color = Color.white;
        if(reachable)
            navMeshAgent.SetDestination(target.transform.position);
        else 
            navMeshAgent.SetDestination(spawnPosition);
        navMeshAgent.SetDestination(target.transform.position);
        navMeshAgent.speed=defaultSpeed; 
        running = false;
        freeze = false;
        _footStepSoundLength = 0.4f;
    }


    private IEnumerator WaitForFootSteps(){
        _step = false;
        yield return new WaitForSeconds(_footStepSoundLength);
        audioSource.PlayOneShot(stepSound,1f);
        _step = true;
    }

    private IEnumerator FollowMe(){
        audioSource.loop = false;
        audioSource.spatialBlend = 1;
        audioSource.pitch = 1f;
        audioSource.volume = 1f ;

        //audioSource.Play();
        charging = false;
        bool reachable = true;
        while(!dead){
             NavMeshPath navMeshPath = new NavMeshPath();
            if (navMeshAgent.CalculatePath(target.transform.position, navMeshPath) )
            {
                reachable = navMeshPath.status == NavMeshPathStatus.PathComplete;
            }
            if(!charging){
                charging = true;
                StartCoroutine(ChargingRun());
                Walk(reachable);
            }
            else if(charged){
                charged = false;
                Freeze();
                yield return new WaitForSeconds(1f);
                Run(reachable);
            }
            else if(running){
                if(navMeshAgent.remainingDistance < 1.5f){
                    Freeze();
                    //hit only if reachable
                    if(reachable){
                        isVulnerable = true;
                        creepMaterial.color = Color.red;
                    }
                    yield return new WaitForSeconds(2f);
                    charging = false;
                    Walk(reachable);
                }                
            }else{        
                Vector3 direction = target.transform.position- gameObject.transform.position;
                float distance = direction.magnitude;
                if(reachable)
                    navMeshAgent.SetDestination(target.transform.position);
                else 
                    navMeshAgent.SetDestination(spawnPosition);
                if(distance < attackThreshold && !isAttacking){
                    StartCoroutine(Attack());
                }
            }
            yield return null;
        }
    }

    private IEnumerator Attack(){
       
        animator.SetBool("Attack", true);
        isAttacking = true;
        yield return new WaitForSeconds(0.2f);
        RaycastHit hit;
        if(Physics.SphereCast(transform.position, 0.65f ,transform.forward , out hit, attackThreshold, 63 )){
            if(hit.collider.gameObject == target.gameObject)
            {
                target.Hurt(attackDamage);
            }
        }
        audioSource.Stop();
        audioSource.PlayOneShot(attackingSound,1f);
        yield return new WaitForSeconds(0.6f);
        animator.SetBool("Attack", false);
        yield return new WaitForSeconds(0.5f);
        audioSource.Play();

        isAttacking = false;
    }

    public void Die(){
        dead = true;
        Managers.Player.RemoveEnemy(GetInstanceID(), 0);
        foreach(FinalBossTrigger t in finalBossTriggers){
            t.RemoveFinalBoss(gameObject);
        }
        running = false;
        audioSource.PlayOneShot(deathSound);
        animator.SetBool("Dead", true);
        creepMaterial.color = Color.white;
        StopCoroutine(FollowMeCoroutine);
        //DropItems();
        
    }
    private void DropItems()
    {
        Debug.Log("DROPPED RADIO");
        GameObject dropped = Instantiate(item,transform.position - new Vector3(2f,2f,0f), Quaternion.identity,GameObject.Find("AllCollectables").transform);
        dropped.AddComponent<Rigidbody>();
        dropped.name = item.name;
        
    }

    public void AddFinalBossTrigger(FinalBossTrigger trigger){
        finalBossTriggers.Add(trigger);
        //Debug.Log("There");
    }
    // Update is called once per frame

    void OnTriggerEnter(Collider collider){
        if(!running)
            return;
        if(collider.tag == Settings.PLAYER_TAG){
            target.Hurt(attackDamage*2);
        }
    }

    public void Hurt(int damage)
    {
        if(isVulnerable){
            health-=damage;
            Messenger.Broadcast(GameEvent.ENEMY_DAMAGED);
        }
        if(health<=0 && !dead){
            
            Die();
        }
    }

    void Update(){
        Debug.Log("step e freeze: "+_step +" "+freeze);
        if(_step && !freeze){
            
            StartCoroutine(WaitForFootSteps());
        }
    }

    // public void SaveData(){
    //     ScaryGirlSavingData data = new ScaryGirlSavingData();
    //     data.position = transform.position;
    //     data.rotation = transform.localEulerAngles;
    //     data.dead = dead;
    //     data.awaken = awaken;
    //     data.scaryGirlTriggersNames = new List<string>();
    //     foreach(ScaryGirlTrigger t in scaryGirlTriggers){
    //         data.scaryGirlTriggersNames.Add(t.gameObject.name);
    //     }
    //     Settings.gameData.scaryGirlsData.Add(data); 
    // }
    // public void LoadFromData(ScaryGirlSavingData data ){
    //     transform.parent.position = data.position;
    //     transform.parent.localEulerAngles = data.rotation;
    //     dead = data.dead;  
    //     awaken = data.awaken;

    //     foreach(string t in data.scaryGirlTriggersNames){
    //         GameObject triggerObject = GameObject.Find(t);
    //         if(triggerObject != null){
    //             ScaryGirlTrigger trigger = triggerObject.GetComponent<ScaryGirlTrigger>();
    //             trigger.AddScaryGirl(gameObject);
    //             scaryGirlTriggers.Add(trigger);
    //         }
    //     }
    //     if(dead)
    //         Destroy(this.transform.parent.gameObject);
    //     else{
    //         ActivateNavMeshAndAnimator();
    //         if(awaken){
    //             WakeUp();
    //         }
    //     }


    // }


}
