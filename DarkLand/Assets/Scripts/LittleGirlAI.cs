using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class LittleGirlAI : MonoBehaviour
{
    [SerializeField] private GameObject target;
    [SerializeField] private float viewAngle;
    [SerializeField] private float viewRange;
    private Transform lastPoint = null;
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
    [SerializeField] private GameObject hidingPoints;
    private int numberOfHidingPoints;
    private void StopCounting()
    {
        if (!targetInput.IsHiding())
        {
            animator.SetBool("Moving", true);
            currentState = States.chasing;
            targetInput.toggleDetectedByLittleGirl(true);
            audioSource.PlayOneShot(foundYouClip, 0.3f);
        }
        else
        {
            currentState = States.hiding;
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
            if (Physics.Raycast(transform.position, toTarget, out RaycastHit hit, viewRange))
                if (hit.transform.root == target)
                    return true;
        return false;
    }
    // Start is called before the first frame update
    void Start() { 
    
        audioSource = GetComponent<AudioSource>();
        targetInput = target.GetComponent<FPSInput>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        numberOfHidingPoints = hidingPoints.transform.childCount;
        TeleportToRandomPoint();
}

    private void StartCounting()
    {
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
        Debug.Log("Sampling");
        int randomIndex = Random.Range(0, numberOfHidingPoints);
        Transform randomChild = hidingPoints.transform.GetChild(randomIndex);
        while (randomChild == lastPoint)
        {
            randomIndex = Random.Range(0, numberOfHidingPoints);
            randomChild = hidingPoints.transform.GetChild(randomIndex);
        }
        Debug.Log("Teleporting to: " + randomIndex);
        transform.position = randomChild.position;
        transform.rotation = randomChild.rotation;
        navMeshAgent.enabled = true;
    }
    // Update is called once per frame
    void Update()
    {
        if(currentState == States.idle && CanSeeTarget(target.transform, viewAngle, viewRange))
        {
            StartCounting();
        }
        else if(currentState == States.hiding)
        {
            TeleportToRandomPoint();
            currentState = States.idle;
            animator.SetBool("Moving", false);
            /*
            if (!moving)
            {
                Debug.Log("Sampling");
                int randomIndex = Random.Range(0, numberOfHidingPoints);
                Transform randomChild = hidingPoints.transform.GetChild(randomIndex);
                while(randomChild == lastPoint)
                {
                    randomIndex = Random.Range(0, numberOfHidingPoints);
                    randomChild = hidingPoints.transform.GetChild(randomIndex);
                }
                navMeshAgent.SetDestination(randomChild.position);
                moving = true;
                lastPoint = randomChild;
            }
            if (PathFinished())
            {
                Debug.Log("Going back idle");
                moving = false;
                transform.Rotate(0f, 180f, 0f);
                currentState = States.idle;
                animator.SetBool("Moving", false);
            }
            */
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
}
