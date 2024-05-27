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
    [SerializeField] private float _gunRange;
    [SerializeField] private float _bulletsSpread;

    [SerializeField] private ShootingMode shootingMode = ShootingMode.Single;
    [SerializeField] private GameObject bulletPrefab;
    private Animator _animator;
    private AudioSource _audioSource;
    [SerializeField] private AudioClip _shootAudioClip;
    private void Start(){
        _camera = Camera.main;
        _audioSource = GetComponent<AudioSource>();
    }
    public void Shoot(){
        if(_animator == null){
            _animator = gameObject.GetComponent<Animator>();
        }
       if(!_animator.GetBool("shooting")){
            
            //_timeSinceLastShoot = 0.0f;
            if(shootingMode == ShootingMode.Single){
                _animator.SetBool("shooting", true);
                StartCoroutine(ResetShooting());
                Debug.Log("Shooting in single mode");
            }
            else if(shootingMode == ShootingMode.Auto){
                Debug.Log("Shooting in auto mode");
            }
            Vector3 point = new Vector3(_camera.pixelWidth/2, _camera.pixelHeight/2, 0);
            Vector3 worldPoint = _camera.ScreenToWorldPoint(point);
            RaycastHit hit;
            Physics.Raycast(worldPoint, _camera.gameObject.transform.forward, out hit);
            //if (Physics.Raycast(worldPoint, _camera.gameObject.transform.forward, out hit)){
                GameObject hitObject = hit.transform.gameObject;
                // Calculate spread along x and y axes based on the hit normal
                Vector3 spreadDirection = Vector3.Cross(hit.normal, _camera.gameObject.transform.forward).normalized;
                Vector3 spreadX = spreadDirection * Random.Range(-_bulletsSpread, _bulletsSpread);
                Vector3 spreadY = Vector3.Cross(hit.normal, spreadDirection).normalized * Random.Range(-_bulletsSpread, _bulletsSpread);
                
                // Add bias to the hit point based on the calculated spreads
                Vector3 hitPoint = hit.point + spreadX + spreadY;
                StartCoroutine(SphereIndicator(hitPoint));
                //create bullet from the center of the gun, direct it towards the hit point and then
                //set velocity and direction inside the BulletBehaviour script
                GameObject projectile = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                Vector3 BulletDir = hitPoint - projectile.transform.position;
                projectile.GetComponent<BulletBehaviour>().SetVelocityDirectionAndHitPoint(0.1f, BulletDir, hitPoint);
                Quaternion lookRotation = Quaternion.LookRotation(BulletDir);
                // Apply the rotation to the projectile
                projectile.transform.rotation = lookRotation;
                //play shoot sound
                _audioSource.PlayOneShot(_shootAudioClip);
            //}
        }
    }
    private IEnumerator SphereIndicator(Vector3 pos){
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = pos;
        sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        yield return new WaitForSeconds(10);
        
        Destroy(sphere);
    }
    private IEnumerator ResetShooting(){
        yield return new WaitForSeconds(_rechargeTime);
        _animator.SetBool("shooting", false);
    }
}
