using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AxeDamageCollider : MonoBehaviour{

    private UsableAxe _usableAxe;

    void Start(){
        _usableAxe = transform.parent.GetComponent<UsableAxe>();
    }

    void OnTriggerEnter(Collider other){
        //axe collider is only enabled during hit and it is
        //only enabled when the interactable trigger is disabled
        //Debug.Log("Axe collided");
        if(other.transform.gameObject.CompareTag(Settings.ENEMY_TAG))
            _usableAxe.Damage(other.gameObject);
    }
}
