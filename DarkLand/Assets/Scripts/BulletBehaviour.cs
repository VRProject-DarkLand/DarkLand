using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour{
    
    private float _velocity;
    private Vector3 _direction;
    private Vector3 _hitPoint;
    private float _hitDistance;
    private float distance = 0.0f;
    // Update is called once per frame
    void Update(){
        Vector3 delta = Vector3.ClampMagnitude(_velocity * Time.deltaTime * _direction, _velocity);
        distance += Vector3.Magnitude(delta);
        transform.position = transform.position +  delta;
        if(distance > _hitDistance){
            //check with a raycast if the bullet would hit right now and , if so, hit
        }
        if(distance > 100f){
            Debug.Log("Bullet lost - destroy");
            Destroy(gameObject);
        }
    }
    public void SetVelocityDirectionAndHitPoint(float velocity, Vector3 direction, Vector3 hitPoint) {
        _velocity = velocity;
        _direction = direction;
        _hitPoint = hitPoint;
        _hitDistance = Vector3.Distance(gameObject.transform.position, _hitPoint);
    }
    void OnTriggerEnter(){
        Debug.Log("Bullet hit - destroy");
        Destroy(gameObject);
    }
}
