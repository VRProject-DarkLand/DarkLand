using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableAxe : IUsableObject{
    [SerializeField] private float _pickupTime = 0.25f;
    private bool _readyToHit = false;

    private Renderer _axeRenderer;
    private Renderer _armsRenderer;
    [SerializeField] private int attackDamage = 10;
    void Awake(){
        Renderer[] renderers = transform.GetComponentsInChildren<Renderer>();
        _axeRenderer = renderers[0];
        _armsRenderer = renderers[1];
        _axeRenderer.enabled = true;
        _armsRenderer.enabled = false;
        
    }
    public override bool IsDummy(){
        return false;
    }

    public override void Select(){
        CreateAnimatorIfNull();
        _animator.SetBool("selected", true);
        StartCoroutine(PickupTime());
        //Debug.Log("Selected axe");
        EnableAxe();
    }

    public override void Use(){
        if(_readyToHit){
            _animator.SetBool("hitting", true);
            //Debug.Log("hitting");
            StartCoroutine(Hit());
        }
    }

    public override void Deselect(){
        Debug.Log("Deselected axe");
        DisableAxe();
        _animator.SetBool("selected", false);
        _animator.SetBool("hitting", false);
        _readyToHit = false;
    }
    public override void Position(){
        gameObject.transform.localPosition = Vector3.zero;
        gameObject.transform.localEulerAngles = Vector3.zero;
        gameObject.transform.GetChild(0).localPosition = _handlingPosition;
        gameObject.transform.GetChild(0).localEulerAngles = _handlingRotation;
        CreateAnimatorIfNull();
    }
    void CreateAnimatorIfNull(){
        if(_animator == null && _controller != null){
            _animator = gameObject.transform.GetChild(0).gameObject.AddComponent<Animator>();
            _animator.runtimeAnimatorController = _controller;
        }
    }
    private void EnableAxe(){
        _axeRenderer.enabled = true;
        _armsRenderer.enabled = true;
    }
    private void DisableAxe(){
        _axeRenderer.enabled = false;
        _armsRenderer.enabled = false;
        _animator.Play("initial");
    }
    private IEnumerator PickupTime(){
        yield return new WaitForSeconds(_pickupTime);
        _readyToHit = true;
    }
        private IEnumerator Hit(){
        _readyToHit = false;
        yield return new WaitForSeconds(0.8f);
        _animator.SetBool("hitting", false);
        _readyToHit = true;
    }
}

