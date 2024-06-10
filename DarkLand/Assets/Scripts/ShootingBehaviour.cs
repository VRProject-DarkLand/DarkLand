using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ShootingBehaviour : MonoBehaviour
{
    public enum ShootingMode{
        Auto = 0,
        Single = 1
    }
    private Camera _camera;
    [SerializeField] private float _rechargeTime;
    [SerializeField] private float _bulletsSpread = 0.01f;

    [SerializeField] private ShootingMode shootingMode = ShootingMode.Single;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private float _bulletsSpeed = 50f;
    [SerializeField] private float _bulletRange = 100f;
    private Animator _animator;
    [SerializeField] private GameObject _bulletSource;
    [SerializeField] private AudioClip _shootAudioClip;
    [SerializeField] private GameObject bulletHolePrefab;
    private void Start(){
        _camera = Camera.main;
    }
    public void Shoot(){
        if(_animator == null){
            _animator = gameObject.GetComponent<Animator>();
        }
       if(!_animator.GetBool("shooting")){
            RaycastHit hitTowardsBulletSource;
            Vector3 toCheckDirection = _bulletSource.transform.position - _camera.transform.position;
            float cameraToBulletSourceDistance = Vector3.Distance(_bulletSource.transform.position, _camera.transform.position);
            Physics.Raycast(_camera.transform.position, toCheckDirection, out hitTowardsBulletSource, cameraToBulletSourceDistance, Settings.RAYCAST_MASK, QueryTriggerInteraction.Ignore);
            //do not shoot if the pistol is behind some object
            //e.g. there is an object between the camera and the bullet source (maybe a wall)
            if(hitTowardsBulletSource.transform != null){
                Debug.Log("Cannot shoot, something is between camera and bullet source");
                return;
            }
            //now it is possible to shoot
            if(shootingMode == ShootingMode.Single){
                _animator.SetBool("shooting", true);
                StartCoroutine(ResetShooting());
                //Debug.Log("Shooting in single mode");
            }
            else if(shootingMode == ShootingMode.Auto){
                _animator.SetBool("shooting", true);
                StartCoroutine(ResetShooting());
                //Debug.Log("Shooting in auto mode");
            }
            Vector3 point = new Vector3(_camera.pixelWidth/2, _camera.pixelHeight/2, 0);
            Vector3 worldPoint = _camera.ScreenToWorldPoint(point);
            // RaycastHit hit;
            // Physics.Raycast(worldPoint, _camera.gameObject.transform.forward, out hit, _bulletRange, Settings.RAYCAST_MASK, QueryTriggerInteraction.Ignore);
            // float hitDistance = hit.transform != null ? hit.distance : _bulletRange;

            //create bullet from the center of the gun, direct it towards the hit point and then
            //set velocity and direction inside the BulletBehaviour script
            GameObject projectile = Instantiate(bulletPrefab, _bulletSource.transform.position, Quaternion.identity);
            projectile.GetComponent<BulletBehaviour>().SetBulletHolePrefab(bulletHolePrefab);
            //compute bullet dir as the forward direction of the camera plus some bias that is 
            //a random value between the maximum amount of spread on both x and y local axis
            Vector3 BulletDir =_camera.gameObject.transform.forward;
            BulletDir.x += Random.Range(-_bulletsSpread, _bulletsSpread);
            BulletDir.y += Random.Range(-_bulletsSpread, _bulletsSpread);
            BulletBehaviour bulletBehaviour = projectile.GetComponent<BulletBehaviour>();
            bulletBehaviour.SetVelocityRangeDirectionAndHitPoint(_bulletsSpeed, _bulletRange, Vector3.Normalize(BulletDir));
            Quaternion lookRotation = Quaternion.LookRotation(BulletDir);
            // Apply the rotation to the projectile
            projectile.transform.rotation = lookRotation;
            //play shoot sound
            Managers.AudioManager.PlaySound(_shootAudioClip, 1f);
        }
    }
    // private IEnumerator SphereIndicator(Vector3 pos){
    //     GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
    //     sphere.transform.position = pos;
    //     sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
    //     yield return new WaitForSeconds(10);
        
    //     Destroy(sphere);
    // }
    private IEnumerator ResetShooting(){
        yield return new WaitForSeconds(_rechargeTime);
        _animator.SetBool("shooting", false);
    }
}
