using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(Animator))]
public class ShootingBehaviour : MonoBehaviour
{
    public enum ShootingMode{
        Auto = 0,
        Single = 1
    }
    [SerializeField] private Camera _camera;
    [SerializeField] private float _rechargeTime;
    [SerializeField] private float _gunRange;
    [SerializeField] private float _bulletsSpread;

    [SerializeField] private ShootingMode shootingMode = ShootingMode.Single;
    //private Animator _animator;
    private float _timeSinceLastShoot = 0f;
 
    void Start(){
        //_animator = GetComponent<Animator>();
    }

    void Update(){
        _timeSinceLastShoot += Time.deltaTime;
    }
    public void Shoot(){
    //    if(_timeSinceLastShoot > _rechargeTime && !_animator.GetBool("shooting")){
            
    //         _timeSinceLastShoot = 0.0f;
    //         if(shootingMode == ShootingMode.Single){
    //             _animator.SetBool("shooting", true);
    //             Debug.Log("Shooting in single mode");
    //         }
    //         else if(shootingMode == ShootingMode.Auto){
    //             Debug.Log("Shooting in auto mode");
    //         }
    //         Vector3 point = new Vector3(_camera.pixelWidth/2, _camera.pixelHeight/2, 0);
    //         Vector3 worldPoint = _camera.ScreenToWorldPoint(point);
    //         RaycastHit hit;
    //         if (Physics.Raycast(worldPoint, _camera.gameObject.transform.forward, out hit)){
    //             GameObject hitObject = hit.transform.gameObject;
    //             // Calculate spread along x and y axes based on the hit normal
    //             Vector3 spreadDirection = Vector3.Cross(hit.normal, _camera.gameObject.transform.forward).normalized;
    //             Vector3 spreadX = spreadDirection * Random.Range(-_bulletsSpread, _bulletsSpread);
    //             Vector3 spreadY = Vector3.Cross(hit.normal, spreadDirection).normalized * Random.Range(-_bulletsSpread, _bulletsSpread);
                
    //             // Add bias to the hit point based on the calculated spreads
    //             Vector3 hitPoint = hit.point + spreadX + spreadY;
    //             StartCoroutine(SphereIndicator(hitPoint));
    //         }
    //     }
    }
    private IEnumerator SphereIndicator(Vector3 pos){
        GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        sphere.transform.position = pos;
        sphere.transform.localScale = new Vector3(0.2f, 0.2f, 0.2f);
        yield return new WaitForSeconds(2);
        
        Destroy(sphere);
    }
    void StartedShooting(){
        //Debug.Log("started shooting");
    }
    void EndedShooting(){
        //Debug.Log("ended shooting");
        //_animator.SetBool("shooting", false);
    }
}
