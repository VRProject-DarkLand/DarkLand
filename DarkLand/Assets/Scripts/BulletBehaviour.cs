using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour{
    
    private float _velocity = 0f;
    private Vector3 _direction;
    private float _bulletRange;
    private float _distance = 0.0f;
    private GameObject _bulletHolePrefab;
    private float _distanceOfBulletHoleFromTarget = 0.01f;
    private int _attackDamage = 5;
    void Update(){
        //compute the distance(actually the jump) that the projectile will do in this frame
        //without appying it
        Vector3 delta = _velocity * Time.deltaTime * _direction;
        float deltaMagnitude = Vector3.Magnitude(delta);
        //Debug.Log("Distance in one frame " + deltaMagnitude);
        
        // make a new raycast at every frame. In this way it is possible to
        // check if the bullet, during this frame, has hit something that: either is the
        // precalculated target from the shooting behaviour of the gun, or
        // some movable object that came into the trajectory while the bullet has been travelling
        RaycastHit currentHit;
        //the raycast length has to be the bullet range shortened by the distance that it has already traversed
        Physics.Raycast(transform.position, _direction, out currentHit, _bulletRange - _distance, Settings.RAYCAST_MASK, QueryTriggerInteraction.Ignore);
        //hitting in this frame
        if(currentHit.transform != null && currentHit.distance < deltaMagnitude){
            Messenger.Broadcast(GameEvent.ENEMY_DAMAGED);
            
            Vector3 hitPoint = currentHit.point;
            GameObject hole = Instantiate(_bulletHolePrefab, currentHit.point + Vector3.ClampMagnitude(currentHit.normal, _distanceOfBulletHoleFromTarget), Quaternion.LookRotation(-currentHit.normal));
            hole.transform.SetParent(currentHit.transform, true);
            Debug.Log("Bullet hit " + currentHit.transform.name);
            currentHit.transform.gameObject.SendMessage("Hurt", _attackDamage, SendMessageOptions.DontRequireReceiver);
            Destroy(gameObject);
        }

        //never hitting, but going out of gun scope
        if(_distance > _bulletRange){
            //Debug.Log("Bullet lost - destroy");
            Destroy(gameObject);
        }
        // make the bullet travel by updating both distance from 
        // start point and position
        _distance += deltaMagnitude;
        transform.position = transform.position + delta;
    }
    // public IEnumerator printTravelTimeAfterOneSec(){
    //     yield return new WaitForSeconds(0.3f);
    //     Debug.Log("Travelled distance: " + _distance);
    // }
    public void SetVelocityRangeDirectionAndHitPoint(float velocity, float bulletRange, Vector3 direction) {
        _velocity = velocity;
        _bulletRange = bulletRange;
        _direction = direction;
    }
    public void SetBulletHolePrefab(GameObject bulletHolePrefab){
        _bulletHolePrefab = bulletHolePrefab;
    }
        
}
