using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UI.Image;
using UnityEngine.UIElements;
using Unity.AI.Navigation;
using UnityEngine.SceneManagement;



[RequireComponent(typeof(AudioSource))]
public class WaypointMover : MonoBehaviour, IDataPersistenceSave, IDamageableEntity{
    [System.Serializable]
    public class SpiderData{
        public int health;
        public Vector3 position;
        public Vector3 rotation;
        public List<string> spiderTriggersNames;
    }
    [SerializeField] private Waypoints waypoints;
    [SerializeField] private float moveSpeed = 3.5f;
    [SerializeField] private float distanceThreshold = 0.1f;
    [SerializeField] private float attackThreshold = 1.5f;
    [SerializeField] private GameObject _renderer;
    [SerializeField] private GameObject dropKey;
    [SerializeField] private GameObject dropLetter;
    [SerializeField] private GameObject dropAdrenaline;
    [SerializeField] private LightsLeverInteractable lightsLever;
    [SerializeField] private GameObject doorToClose;
    [SerializeField] private bool goBehindPlayer = false;
    private bool doorClosed = false;
    private NavMeshAgent navMeshAgent;
    private GameObject target;
    private Animator animator;
    private bool alive = false;
    private const float fearDistance = 8f;
    private bool isAttacking = false;
    [SerializeField] private int attackDamage = 20;
    private Transform currentWaypoint = null;
    private Vector3 startPosition;
    private Vector3 startRotation;
    private int _health = 20;
    private int fear = 15;
    [SerializeField] private SpiderTrigger sceneTrigger;
    private List<SpiderTrigger> spiderTriggers = new List<SpiderTrigger>();
    private float viewAngle = 30f;
    private const float distanceBehind = 0.5f;

    private AudioClip _hurtClip;
    AudioSource _source;
    // Start is called before the first frame update
    void Start(){
        target = GameObject.FindGameObjectWithTag(Settings.PLAYER_TAG);
        animator = GetComponentInChildren<Animator>();
        animator.enabled = true;
        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        transform.position = currentWaypoint.position;
        currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
        animator.SetBool("Idle", true);
        startPosition = transform.parent.position;
        if(_renderer == null){
            _renderer = GetComponentInChildren<Renderer>().gameObject;
        }
        startRotation = transform.parent.localEulerAngles;
        if(!Settings.LoadedFromSave || SceneManager.GetActiveScene().name == Settings.ASYLUM_NAME){
            spiderTriggers.Add(sceneTrigger);
        }
    }

    // Update is called once per frame
    void Update() {
        if (alive && currentWaypoint != null) {
            animator.SetBool("Idle", false);
            if(!doorClosed && doorToClose != null) closeDoor();
            transform.position = Vector3.MoveTowards(transform.position, currentWaypoint.position, moveSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, currentWaypoint.position) < distanceThreshold) {
                currentWaypoint = waypoints.GetNextWaypoint(currentWaypoint);
                if (currentWaypoint != null) {
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
                    navMeshAgent.angularSpeed = 3000;
                }
            }
        }
        else if(navMeshAgent != null && alive){
            
            if (alive && !isAttacking){
                Vector3 goTo = goBehindPlayer ? target.transform.position - target.transform.forward * distanceBehind : target.transform.position;
                navMeshAgent.SetDestination(goTo);
                animator.SetBool("Idle", false);
                Vector3 ignoreY = new Vector3(goTo.x, transform.position.y, goTo.z);
                Vector3 toTarget = ignoreY - transform.position;
                if (Vector3.Angle(transform.forward, toTarget) <= viewAngle)
                {
                    float distance = Vector3.Distance(target.transform.position, transform.position);
                    if(distance <= fearDistance)
                    {
                        Managers.Player.AddEnemy(GetInstanceID(),fear);
                    }else{
                        Managers.Player.RemoveEnemy(GetInstanceID(),fear);
                    }
                    //Debug.Log("Ti vidu");
                    if ( distance < attackThreshold && !isAttacking)
                    {
                        transform.LookAt(target.transform.position);
                        StartCoroutine(Attack());
                    }
                }

            }
        }
    }

    private void closeDoor()
    {
        OpenDoubleDoor openDoubleDoor = doorToClose.GetComponentInChildren<OpenDoubleDoor>();
        NavMeshLink link = doorToClose.GetComponentInChildren<NavMeshLink>();
        InteractableTrigger interactableTrigger = doorToClose.GetComponentInChildren<InteractableTrigger>();
        interactableTrigger.enabled = false;
        link.enabled = false;
        if (openDoubleDoor.IsOpened())
        {
            openDoubleDoor.Interact();
        }
        doorClosed = true;
    }
    private void OpenDoor()
    {
        NavMeshLink link = doorToClose.GetComponentInChildren<NavMeshLink>();
        InteractableTrigger interactableTrigger = doorToClose.GetComponentInChildren<InteractableTrigger>();
        interactableTrigger.enabled = true;
        link.enabled = true;
    }
    private IEnumerator Attack(){
        animator.SetBool("Attack", true);
        navMeshAgent.SetDestination(transform.position);
        isAttacking = true;
        yield return new WaitForSeconds(0.6f);
        RaycastHit hit;
        //Debug.DrawRay(transform.position + new Vector3(0f,0.5f,0f), transform.forward, Color.magenta,2f);
        if(Physics.Raycast(transform.position + new Vector3(0f, 0.5f, 0f), transform.forward , out hit, 3, ~LayerMask.GetMask("Ignore Raycast"))){
            if(hit.collider.gameObject == target)
            {
                hit.collider.gameObject.SendMessage("Hurt", attackDamage, SendMessageOptions.DontRequireReceiver);
            }
        }
        animator.SetBool("Attack", false);
        isAttacking = false;
        yield return new WaitForSeconds(3f);
    }
    public void Hurt(int damage){
        _source.PlayOneShot(_hurtClip);
        _health -= damage;
        Messenger.Broadcast(GameEvent.ENEMY_DAMAGED);
        //Debug.Log("spider damaged Current life " + _health);

        if(_health <= 0){
            //Debug.Log("spider died");
            Die();
        }

    }
    public void AddSpiderTrigger(SpiderTrigger trigger){
        spiderTriggers.Add(trigger);
    }
    private IEnumerator disappear()
    {
        Renderer currentRendered = _renderer.GetComponent<Renderer>();
        Color start = currentRendered.material.color;
        Color end = new Color32(255, 255, 255, 0);
        float count = 0f;
        while (count < 1.0f)
        {
            currentRendered.material.color = Color.Lerp(start, end, count);
            count += Time.deltaTime/5f;
            //Debug.Log(count);
            yield return null;
        }
        if(GameObject.Find("Spider 1")  == null || GameObject.Find("Spider 2") == null) {
            if(dropAdrenaline != null && dropKey != null && dropLetter != null) {
                DropItems();
            }
            if(doorToClose != null)
                OpenDoor();
        }

        Destroy(transform.parent.gameObject);
    }

    private void DropItems()
    {
        GameObject key = Instantiate(dropKey, transform.position, Quaternion.identity, GameObject.Find("AllCollectables").transform);
        key.name = "Mortuary room key";
        GameObject letter = Instantiate(dropLetter, transform.position, Quaternion.identity, GameObject.Find("AllCollectables").transform);
        letter.name = "Spider Letter";
        GameObject adrenaline = Instantiate(dropAdrenaline, transform.position, Quaternion.identity, GameObject.Find("AllCollectables").transform);
        adrenaline.name = "Adrenaline";
        lightsLever.Interact();
    }

    public void Die(){
        Managers.Player.RemoveEnemy(GetInstanceID(),fear);
        _source.Stop();
        foreach(SpiderTrigger t in spiderTriggers){
            t.RemoveSpider(gameObject);
        }
        navMeshAgent.SetDestination(transform.position);
        animator.SetBool("Dead", true);
        alive = false;
        StartCoroutine(disappear());
    }
    public void WakeUp(){
        alive = true;
        _source = GetComponent<AudioSource>();
        _source.clip = ResourceLoader.GetSound(Settings.AudioSettings.SPIDER_SOUND);
        _hurtClip = ResourceLoader.GetSound(Settings.AudioSettings.SPIDER_HURT_SOUND);
        _source.loop = true;
        _source.volume = 1f;
        _source.spatialBlend = 1;
        _source.Play();
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

    public void SaveData(){
        if(!GameEvent.exitingCurrentScene){
            SpiderData data = new SpiderData();
            data.health = _health;
            data.position = startPosition;
            data.rotation = startRotation;
            Settings.gameData.spidersData.Add(data);
            data.spiderTriggersNames = new List<string>();
            foreach(SpiderTrigger t in spiderTriggers){
                data.spiderTriggersNames.Add(t.gameObject.name);
            }
        }
    }
    public void LoadFromData(SpiderData data){
        _health = data.health;
        transform.parent.position = data.position;
        transform.parent.localEulerAngles = data.rotation;        
        foreach(string t in data.spiderTriggersNames){
            GameObject triggerObject = GameObject.Find(t);
            if(triggerObject != null){
                SpiderTrigger trigger = triggerObject.GetComponent<SpiderTrigger>();
                trigger.AddSpider(gameObject);
                spiderTriggers.Add(trigger);
            }
        }
    }
}