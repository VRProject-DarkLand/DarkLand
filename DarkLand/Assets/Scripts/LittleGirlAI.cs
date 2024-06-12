using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class LittleGirlAI : MonoBehaviour, IDataPersistenceSave{
    [System.Serializable]
    public class LittleGirlSavingData{
        public Vector3 position;
        public Vector3 rotation;
    }
    private GameObject target;
    [SerializeField] private float viewAngle;
    [SerializeField] private float viewRange;
    private Vector3 lastPoint = new Vector3(0f,0f,0f);
    private NavMeshAgent navMeshAgent;
    private Animator animator;
    private FPSInput targetInput;
    private enum States {idle, counting, hiding, chasing};
    private States currentState = States.idle;
    private AudioSource audioSource;
    [SerializeField] private AudioClip countingClip;
    [SerializeField] private AudioClip foundYouClip;
    public delegate void AudioCallback();
    private bool moving = false;
    private int damage = 110;
    private int fear = 40;
    [SerializeField] private GameObject hidingPoints;
    private int numberOfHidingPoints;
    private void StopCounting()
    {
        if (!targetInput.IsHiding())
        {
            animator.SetBool("Moving", true);
            currentState = States.chasing;
            Managers.Player.AddFear(100);
            targetInput.toggleDetectedByLittleGirl(true);
            audioSource.PlayOneShot(foundYouClip, 0.3f);
        }
        else
        {
            Settings.canSave = true;
            currentState = States.hiding;
            Managers.Player.AddFear(-fear);
        }
        
    }
    public void PlaySoundWithCallback(AudioClip clip, AudioCallback callback)
    {
        audioSource.PlayOneShot(clip,0.3f);
        StartCoroutine(DelayedCallback(clip.length, callback));
    }
    private IEnumerator DelayedCallback(float time, AudioCallback callback)
    {
        yield return new WaitForSeconds(time);
        callback();
    }
    private bool CanSeeTarget(Transform target, float viewAngle, float viewRange){
        Vector3 toTarget = target.position - transform.position;
        if (Vector3.Angle(transform.forward, toTarget) <= viewAngle)
            if (Physics.Raycast(transform.position, toTarget, out RaycastHit hit, viewRange, Settings.RAYCAST_MASK, QueryTriggerInteraction.Ignore))
                if (hit.transform.root == target)
                    return true;
        return false;
    }
    // Start is called before the first frame update
    void Start() { 
        target = GameObject.FindGameObjectWithTag(Settings.PLAYER_TAG);
        audioSource = GetComponent<AudioSource>();
        targetInput = target.GetComponent<FPSInput>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.enabled = true;
        animator = GetComponent<Animator>();
        animator.enabled = true;
        numberOfHidingPoints = hidingPoints.transform.childCount;
        if(!Settings.LoadedFromSave){
            TeleportToRandomPoint();
        }
}

    private void StartCounting()
    {
        Settings.canSave = false;
        currentState = States.counting;
        PlaySoundWithCallback(countingClip, StopCounting);
    }

    private bool PathFinished()
    {
        return (!navMeshAgent.pathPending) && (navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance) && (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f);
    }
    private void TeleportToRandomPoint()
    {
        navMeshAgent.enabled = false;
        int randomIndex = Random.Range(0, numberOfHidingPoints);
        Transform randomChild = hidingPoints.transform.GetChild(randomIndex);
        transform.position = randomChild.position;
        transform.rotation = randomChild.rotation;
        navMeshAgent.enabled = true;
    }
    // Update is called once per frame
    void Update()
    {
        if(currentState == States.idle)
        {
            if(CanSeeTarget(target.transform, viewAngle, viewRange))
            {
                lastPoint = target.transform.position;
                //Debug.Log("Ti vidu " + lastPoint);
                Managers.Player.AddFear(fear);
                StartCounting();
            }
            else
            {
                TeleportCloserToPlayer();
            }
        }
        else if(currentState == States.hiding)
        {
            TeleportToRandomPoint();
            currentState = States.idle;
            animator.SetBool("Moving", false);

        }
        else if(currentState == States.chasing)
        {
            if (!moving)
            {
                navMeshAgent.SetDestination(target.transform.position);
                moving = true;
            }
            if (PathFinished())
            {
                Debug.Log("Path finished, hiding");
                moving = false;
                targetInput.Hurt(damage);
                currentState = States.hiding;
            }
        }
    }

    private void TeleportCloserToPlayer()
    {
        navMeshAgent.enabled = false;
        Transform closestHidingPoint = null;
        float closestDistanceSqr = Mathf.Infinity;

        foreach (Transform hidingPoint in hidingPoints.transform)
        {
            //Debug.Log("From: " + hidingPoint.position + " To: " + lastPoint + " = " + Vector3.Distance(hidingPoint.position, lastPoint));
            if (Vector3.Distance(hidingPoint.position,lastPoint) > 5.0f)
            {
                Vector3 directionToTarget = hidingPoint.position - target.transform.position;
                float dSqrToTarget = directionToTarget.sqrMagnitude;

                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    closestHidingPoint = hidingPoint;
                }
            }

        }

        if (closestHidingPoint != null && transform.position != closestHidingPoint.position)
        {
            transform.position = closestHidingPoint.position;
            transform.rotation = closestHidingPoint.rotation;
        }
        navMeshAgent.enabled = true;
    }

    public void SaveData(){
        if(!GameEvent.exitingCurrentScene){
            LittleGirlSavingData data = new LittleGirlSavingData();
            data.position = transform.position;
            data.rotation = transform.localEulerAngles;
            Settings.gameData.littleGirlsData.Add(data); 
        }
    }
    public void LoadFromData(LittleGirlSavingData data ){
        //Debug.Log("Setting parent position to " + data.position);
        transform.parent.position = data.position;
        transform.parent.localEulerAngles = data.rotation;
    
    }
}
