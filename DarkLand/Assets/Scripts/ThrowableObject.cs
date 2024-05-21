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

[RequireComponent(typeof(Rigidbody))]
public class ThrowableObject : IUsableObject
{
    //TrajectoryPredictor trajectoryPredictor;
    Rigidbody objectToThrow;
    [SerializeField, Range(0.0f, 50.0f)]
    float force = 10;

    //[SerializeField]
    //Transform StartPosition;

    void Start(){
        objectToThrow = gameObject.GetComponent<Rigidbody>();
    }

    public override bool IsDummy(){
        return false;
    }

    public override void Select(){
        gameObject.SetActive(true);
        gameObject.GetComponent<Rigidbody>().isKinematic = true;
        foreach(var c in GetComponents<Collider> ())
            c.enabled = false;
        gameObject.transform.localPosition = new Vector3(0.3f, -0.35f, 0.66f);
        //gameObject.transform.position = 
        //set torch visible
    }

    public override void Use(){
        
        GameObject copy = Instantiate(gameObject, gameObject.transform.parent);
        //copy.transform.localPosition += new Vector3(0, 0.2f,0.5f);
        copy.name = gameObject.name;
        
        foreach(var c in copy.GetComponents<Collider> ())
            c.enabled = true;
        copy.GetComponent<InteractableTrigger>().enabled = true;
        copy.GetComponent<Collectable>().enabled = true;
        //Rigidbody thrownObject = Instantiate(objectToThrow, StartPosition.position, Quaternion.identity);
        
        Rigidbody rb = copy.GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.velocity = gameObject.transform.parent.GetComponentInParent<CharacterController>().velocity;
        rb.AddForce(gameObject.transform.parent.forward * force, ForceMode.Impulse);
        
        copy.transform.SetParent(null, true);
        Managers.Inventory.ConsumeItem(gameObject.name);
        Debug.Log("Lu schicciai "+gameObject.name); 
        //Managers.UsableInventory.RemoveSelectable(gameObject);
        
    }

    public override void SecondaryUse()
    {
        ThrowableInfo info = new ThrowableInfo();
        info.mass = objectToThrow.mass;
        info.initialSpeed = force;
        info.initialPosition = gameObject.transform.position;
        info.direction = gameObject.transform.parent.forward;
        Messenger<ThrowableInfo>.Broadcast(GameEvent.PREDICT_TRAJECTORY, info);
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