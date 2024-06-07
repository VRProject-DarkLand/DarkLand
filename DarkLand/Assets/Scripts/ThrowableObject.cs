using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ThrowableInfo{
    public Vector3 direction;
    public Vector3 initialPosition;
    public float initialSpeed;
    public float mass;
    public float drag = 0;

}
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(Rigidbody))]
public class ThrowableObject : IUsableObject
{
    //TrajectoryPredictor trajectoryPredictor;
    Rigidbody objectToThrow;
    [SerializeField, Range(0.0f, 50.0f)]  float force = 300;
    [SerializeField] private AudioClip collisionSound;
    [SerializeField] private bool hasAudio = true;
    private AudioSource audioSource;
    private bool _isAiming = false;
    //[SerializeField]
    //Transform StartPosition;
    private Camera _camera;
    void Start(){
        objectToThrow = gameObject.GetComponent<Rigidbody>();
        objectToThrow.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        if(collisionSound == null)
            collisionSound = ResourceLoader.GetSound("objectDropped");
        audioSource = GetComponent<AudioSource>();
        audioSource.spatialBlend = 1f;
        audioSource.playOnAwake = false;
    }

    public override void Collected()
    {
        base.Collected();
        objectToThrow.isKinematic = true;
    }
    public override bool IsDummy(){
        return false;
    }

    public override void Select(){
        gameObject.SetActive(true);
        
        foreach(var c in GetComponents<Collider> ()){
            c.enabled = false;
        }
        Position();
        //gameObject.transform.position = 
        //set torch visible
        _camera = Camera.main;
    }

    
    public override void Position()
    {
        gameObject.transform.localPosition = new Vector3(0.3f, -0.35f, 0.66f);
        gameObject.transform.localEulerAngles = Vector3.zero;
    }

    public override void Use(){
        RaycastHit hitTowardsTrowableObj;
        Vector3 toCheckDirection = transform.position -  _camera.transform.position;
        float cameraToThrowable = Vector3.Distance(transform.position, _camera.transform.position);
        Physics.Raycast(_camera.transform.position, toCheckDirection, out hitTowardsTrowableObj, cameraToThrowable, Settings.RAYCAST_MASK, QueryTriggerInteraction.Ignore);
        //do not thow an object if there is something bwtween the player and the object
        if(hitTowardsTrowableObj.transform != null){
            Debug.Log("Cannot throw, something is between camera and the object");
            Debug.Log("Hit " + hitTowardsTrowableObj.transform.name);
            return;
        }
        GameObject copy = Instantiate(gameObject, gameObject.transform.parent);
        //Debug.Log("THROW AND SET PARENT");
        foreach(Renderer r in copy.GetComponentsInChildren<Renderer>()){
                    r.gameObject.layer = LayerMask.NameToLayer("Default");
        }
        //copy.transform.localPosition += new Vector3(0, 0.2f,0.5f);
        copy.name = gameObject.name;
        
        foreach(var c in copy.GetComponents<Collider> ())
            c.enabled = true;
        copy.GetComponent<InteractableTrigger>().enabled = true;
        Collectable collectable = copy.GetComponent<Collectable>();
        collectable.enabled = true;
        collectable.Collected = false;
        //Rigidbody thrownObject = Instantiate(objectToThrow, StartPosition.position, Quaternion.identity);
        
        Rigidbody rb = copy.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.velocity = gameObject.transform.parent.GetComponentInParent<CharacterController>().velocity;
        rb.AddForce(gameObject.transform.parent.forward * force, ForceMode.Impulse);
        rb.AddRelativeTorque(new Vector3(1,-1,1) * force/10f, ForceMode.Impulse);
        if(useSound == null){
            Managers.AudioManager.PlaySound(ResourceLoader.GetSound("ThrowingSound"));
        }
        //copy.transform.SetParent(null, true);
        Managers.Inventory.ConsumeItem(gameObject.name);

        //Managers.UsableInventory.RemoveSelectable(gameObject);
        copy.transform.parent = Managers.Persistence.GetAllCollectablesContainer().transform;
        
    }
    public override void Deselect(){
        UndoSecondaryUse();
        base.Deselect();
    }
    public override void SecondaryUse()
    {
        ThrowableInfo info = new ThrowableInfo();
        info.mass = objectToThrow.mass;
        info.initialSpeed = force;
        info.initialPosition = gameObject.transform.position;
        info.direction = gameObject.transform.parent.forward;
        Messenger<ThrowableInfo>.Broadcast(GameEvent.PREDICT_TRAJECTORY, info);
        _isAiming = true;
    }

    void OnCollisionEnter(Collision collision){
        if(!hasAudio)
            return;
        if(collision.relativeVelocity.magnitude > 2)
            audioSource.PlayOneShot(collisionSound);
    }

    public override void UndoSecondaryUse()
    {
        if(_isAiming){
            Messenger.Broadcast(GameEvent.CANCEL_TRAJECTORY);
        }
        _isAiming = false;
    }
    // void Update()
    // {
    //     Predict();
    // }

    // void Predict()
    // {
    //     trajectoryPredictor.PredictTrajectory(ProjectileData());
    // }

    // ProjectileProperties ProjectileData()
    // {
    //     ProjectileProperties properties = new ProjectileProperties();
    //     Rigidbody r = objectToThrow.GetComponent<Rigidbody>();

    //     properties.direction = StartPosition.forward;
    //     properties.initialPosition = StartPosition.position;
    //     properties.initialSpeed = force;
    //     properties.mass = r.mass;
    //     properties.drag = r.drag;

    //     return properties;
    // }

    // void ThrowObject(InputAction.CallbackContext ctx)
    // {
    //     Rigidbody thrownObject = Instantiate(objectToThrow, StartPosition.position, Quaternion.identity);
    //     thrownObject.AddForce(StartPosition.forward * force, ForceMode.Impulse);
    // }
}