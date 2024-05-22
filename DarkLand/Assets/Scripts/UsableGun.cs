using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(ShootingBehaviour))]
public class UsableGun : IUsableObject{
    private ShootingBehaviour _shootingBehaviour;

    void Start(){
        _shootingBehaviour = GetComponent<ShootingBehaviour>();
    }

    public override bool IsDummy(){
        return false;
    }

    public override void Deselect(){
        GameEvent.isUsingGun = false;
        base.Deselect();
    }
    public override void Select(){
        GameEvent.isUsingGun = true;
        Debug.Log("SELECTED PISTOL");
        gameObject.SetActive(true);
        gameObject.transform.GetChild(0).gameObject.SetActive(true);
        gameObject.transform.localPosition = new Vector3(0, -0.3f, 1f);
        gameObject.transform.localEulerAngles = new Vector3(-5f, 0f, 0f);
    }

    public override void Use(){
        _shootingBehaviour.Shoot();
    }

}
