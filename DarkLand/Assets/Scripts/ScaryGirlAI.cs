using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(AudioSource))]
public class ScaryGirlAI : MonoBehaviour, IDataPersistenceSave, IDamageableEntity{
    [System.Serializable]
    public class ScaryGirlSavingData {
        public Vector3 position;
        public Vector3 rotation;
        public bool dead;
        public bool awaken;
        public List<string> scaryGirlTriggersNames;
    }

    #region SerializeField
        [SerializeField] private AudioClip attackingSound;
        [SerializeField] private AudioClip grunt;
        [SerializeField] private AudioClip scream;

        [SerializeField] private float attackThreshold = 2.5f;
        [SerializeField] private  bool chasing = false;
        [SerializeField] private float maxDistance = 10f;
        //save if the enemy has been awaken s.t. upon spawning
        [SerializeField] private ScaryGirlTrigger sceneScaryGirlTrigger;
        [SerializeField] private int attackDamage = 60;
        [SerializeField] private GameObject exitKey;
    #endregion
    
    #region PrivateAttributes
        private FPSInput target;
        private AudioSource audioSource;
        private Vector3 spawnPosition;
        private NavMeshAgent navMeshAgent;
        private float defaultSpeed;
        private Animator animator;
        private bool hitByTeddy = false;
        private bool dead = false;
        private bool inSight = false;
        //it can be activated without using the trigger
        private bool awaken = false;
        private bool reachable;
        private int fear = 60;
    #endregion

    bool isAttacking = false;
    List<ScaryGirlTrigger> scaryGirlTriggers = new List<ScaryGirlTrigger>();
    private bool lockedInPlace;

    // Start is called before the first frame update
    void Start()
    {
        target = GameObject.FindGameObjectWithTag(Settings.PLAYER_TAG).GetComponent<FPSInput>();
        spawnPosition = gameObject.transform.position;
        inSight = false;
        audioSource = GetComponent<AudioSource>();
        if(!Settings.LoadedFromSave || GameEvent.OpenedSceneDoor){
            scaryGirlTriggers.Add(sceneScaryGirlTrigger);
            ActivateNavMeshAndAnimator();
        }
    }

    private void ActivateNavMeshAndAnimator(){
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = true;
        Debug.Log("ENABLED: "+navMeshAgent.enabled);
        defaultSpeed = navMeshAgent.speed;
        animator = GetComponent<Animator>();
        animator.enabled = true;
    }

    public void WakeUp(){
        animator.SetBool("Alive", true);
        awaken = true;
    }

    public void Scream(){
        //2D sound, distance does not matter
        Managers.AudioManager.PlaySound(scream);
    }

    public void StartRunning(){
        chasing = true;
        inSight = true;
       Managers.Player.AddEnemy(GetInstanceID(),fear);
        StartCoroutine(FollowMe());
    }

    // private void SetAgentDestination(Vector3 point){
    //     NavMeshAgent
    // }

    private IEnumerator FollowMe(){
        navMeshAgent.SetDestination(spawnPosition);
        audioSource.clip = grunt;
        audioSource.loop = true;
        audioSource.spatialBlend = 1;
        audioSource.pitch = 2;
        audioSource.Play();
        while (!dead)
        {
            if (hitByTeddy)
            {
                audioSource.Stop();
                navMeshAgent.SetDestination(transform.position);
                Managers.Player.RemoveEnemy(GetInstanceID(), fear);
                animator.SetBool("HitByTeddy", true);
                yield return new WaitForSeconds(6f);
                animator.SetBool("HitByTeddy", false);
                hitByTeddy = false;
                audioSource.Play();
                if (dead) break;
            }
            if (!lockedInPlace)
            {
                Vector3 startRaycast = gameObject.transform.position + new Vector3(0, 1.5f, 0);
                RaycastHit hit;
                Vector3 direction = target.transform.position - startRaycast;
                //Debug.Log(Vector3.Distance(target.transform.position,transform.position));
                float distance = direction.magnitude;
                if (GameEvent.isHiding)
                {
                    if (!inSight)
                        chasing = false;
                    else
                    {
                        if (distance < attackThreshold && !isAttacking)
                        {
                            StartCoroutine(Attack());
                        }
                        navMeshAgent.SetDestination(target.transform.position);
                        yield return null;
                    }
                }
                else
                {
                    float detectionDistance = maxDistance;
                    if (target.crouch)
                    {
                        detectionDistance /= 4;
                    }
                    if (reachable && Physics.Raycast(startRaycast, direction, out hit, detectionDistance, Settings.RAYCAST_MASK, QueryTriggerInteraction.Ignore))
                    {
                        if (hit.collider.gameObject.tag == Settings.PLAYER_TAG)
                        {
                            inSight = true;
                            chasing = true;
                            if (distance < attackThreshold && !isAttacking)
                            {
                                StartCoroutine(Attack());
                            }
                            Managers.Player.AddEnemy(GetInstanceID(), fear);
                            navMeshAgent.speed = 2 * defaultSpeed;
                            navMeshAgent.SetDestination(target.transform.position);
                            if (navMeshAgent.isOnOffMeshLink)
                            {
                                navMeshAgent.speed = 2.5f;
                            }

                            yield return new WaitForSeconds(0.2f);
                        }
                        else
                        {
                            inSight = false;
                            if (reachable && chasing && distance < 10f)
                            {
                                navMeshAgent.speed = 2 * defaultSpeed;
                                navMeshAgent.SetDestination(target.transform.position);
                                if (navMeshAgent.isOnOffMeshLink)
                                {
                                    navMeshAgent.speed = 2.5f;
                                }
                                yield return null;
                            }
                            else
                                chasing = false;
                        }
                    }
                    else
                    {
                        chasing = false;
                    }
                }

                NavMeshPath navMeshPath = new NavMeshPath();
                if (navMeshAgent.CalculatePath(target.transform.position, navMeshPath))
                {
                    reachable = navMeshPath.status == NavMeshPathStatus.PathComplete;
                    chasing = false;
                }

                if (!chasing)
                {
                    Managers.Player.RemoveEnemy(GetInstanceID(), fear);
                    //maybe sample position for random point in navmesh
                    navMeshAgent.speed = defaultSpeed;
                    if (Vector3.Distance(navMeshAgent.destination, transform.position) < 2.5f)
                    {
                        Vector3 randomDirection = Random.insideUnitSphere * 25f;
                        randomDirection += transform.position;
                        NavMeshHit hit1;
                        NavMesh.SamplePosition(randomDirection, out hit1, 25f, ~LayerMask.GetMask("SpiderNavMesh"));
                        Vector3 finalPosition = hit1.position;
                        navMeshAgent.SetDestination(finalPosition);
                    }
                    if (distance < maxDistance)
                    {
                        yield return new WaitForSeconds(0.2f);
                    }
                    else if (distance < 2 * maxDistance)
                        yield return new WaitForSeconds(3f);
                    else
                        yield return new WaitForSeconds(6f);
                }
                else
                {

                }
            }
        }
        audioSource.Stop();
        yield return null;
    }

    private IEnumerator Attack(){
       
        animator.SetBool("Attack", true);
        isAttacking = true;
        yield return new WaitForSeconds(0.2f);
        RaycastHit hit;
        if(Physics.SphereCast(transform.position, 0.2f ,transform.forward , out hit, attackThreshold, ~LayerMask.GetMask("Ignore Raycast"), QueryTriggerInteraction.Ignore )){
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
     public void HitByTeddyBear()
    {
        hitByTeddy = true;
    }

    public void Die(){
        foreach(ScaryGirlTrigger t in scaryGirlTriggers){
            t.RemoveScaryGirl(gameObject);
        }
        animator.SetBool("Dead", true);
        dead = true;
        DropItems();
        
    }
    private void DropItems()
    {
        GameObject key = Instantiate(exitKey,transform.position - new Vector3(2f,0f,0f), Quaternion.identity,GameObject.Find("AllCollectables").transform);
        key.name = "Exit Key";
        //Destroy(gameObject.transform.parent.gameObject);
    }

    public void AddScaryGirlTrigger(ScaryGirlTrigger trigger){
        scaryGirlTriggers.Add(trigger);
        //Debug.Log("There");
    }
    // Update is called once per frame
    void Update()
    {
    }

    public void SaveData(){
        if(!GameEvent.exitingCurrentScene){
            ScaryGirlSavingData data = new ScaryGirlSavingData();
            data.position = transform.position;
            data.rotation = transform.localEulerAngles;
            data.dead = dead;
            data.awaken = awaken;
            data.scaryGirlTriggersNames = new List<string>();
            foreach(ScaryGirlTrigger t in scaryGirlTriggers){
                data.scaryGirlTriggersNames.Add(t.gameObject.name);
            }
            Settings.gameData.scaryGirlsData.Add(data); 
        }
    }
    public void LoadFromData(ScaryGirlSavingData data ){
        transform.parent.position = data.position;
        transform.parent.localEulerAngles = data.rotation;
        dead = data.dead;  
        awaken = data.awaken;
        
        foreach(string t in data.scaryGirlTriggersNames){
            GameObject triggerObject = GameObject.Find(t);
            if(triggerObject != null){
                ScaryGirlTrigger trigger = triggerObject.GetComponent<ScaryGirlTrigger>();
                trigger.AddScaryGirl(gameObject);
                scaryGirlTriggers.Add(trigger);
            }
        }
        if(dead)
            Destroy(this.transform.parent.gameObject);
        else{
            ActivateNavMeshAndAnimator();
            if(awaken){
                WakeUp();
            }
        }
        
    
    }

    public void Hurt(int damage){
    }

    internal void lockInPlace()
    {
        lockedInPlace = true;
    }
}
