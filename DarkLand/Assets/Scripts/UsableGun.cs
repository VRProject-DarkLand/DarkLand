using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UsableGun : IUsableObject{
    private ShootingBehaviour _shootingBehaviour = null;
    [SerializeField] private float _pickupTime = 1.2f;
    private bool _readyToFire = false;

    private Renderer _gunRenderer;
    private Renderer _armsRenderer;
    
    void Awake(){
        Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
        _gunRenderer = renderers[0];
        _armsRenderer = renderers[1];
        _gunRenderer.enabled = true;
        _armsRenderer.enabled = false;
        
    }
    public override bool IsDummy(){
        return false;
    }
    //dispose and deactivate the gun (without animation)
    public override void Deselect(){
        _readyToFire = false;
        CreateAnimatorIfNull();
        //Debug.Log("Deselected pistol");
        _animator.SetBool("selected", false);
        _animator.SetBool("shooting", false);
        GameEvent.isUsingGun = false;
        DisableGun();
    }
    //position and activate the gun
    public override void Select(){
        EnableGun();
        CreateAnimatorIfNull();
        GameEvent.isUsingGun = true;
        _animator.SetBool("selected", true);
        StartCoroutine(PickupTime());
        Debug.Log("Pistol selected");
    }

    //shoot with the gun
    public override void Use(){
        Debug.Log("Calling use");
        if(_shootingBehaviour == null){
            _shootingBehaviour = GetComponentInChildren<ShootingBehaviour>();
            Debug.Log("Shooting behaviour is null");
        }
        if(_readyToFire){
            _shootingBehaviour.Shoot();
            Debug.Log("Calling shoot");
        }
    }
    public override void SecondaryUse(){
        Camera.main.fieldOfView = Mathf.Lerp(40, 60, Time.deltaTime * 0.1f);
    }
    public override void UndoSecondaryUse(){
        Camera.main.fieldOfView = Mathf.Lerp(60, 40, Time.deltaTime * 0.1f);
    }
    void CreateAnimatorIfNull(){
        if(_animator == null && _controller != null){
            _animator = gameObject.transform.GetChild(0).gameObject.AddComponent<Animator>();
            _animator.runtimeAnimatorController = _controller;
            Debug.Log(_animator.parameters);
        }
    }
    public override void Position(){
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localEulerAngles = Vector3.zero;
        gameObject.transform.GetChild(0).localPosition = _handlingPosition;
        gameObject.transform.GetChild(0).localEulerAngles = _handlingRotation;
        CreateAnimatorIfNull();
        _animator.enabled = true;
    }
    private IEnumerator PickupTime(){
        yield return new WaitForSeconds(_pickupTime);
        _readyToFire = true;
    }

    private void EnableGun(){
        _gunRenderer.enabled = true;
        _armsRenderer.enabled = true;
    }
    private void DisableGun(){
        _gunRenderer.enabled = false;
        _armsRenderer.enabled = false;
    }
}
