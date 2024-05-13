using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableTrigger : MonoBehaviour{
    [SerializeField] public GameEvent.InteractWithMessage InteractionMessage {get; private set;}
    [SerializeField] protected bool ignoreLookingScore = false;
    protected bool enteredInRange = false;
    protected Transform playerTransform;

    private bool looking = false;

    private bool isCollectable = false;

    // public void OnTriggerEnter(Collider other){
    //     enteredInRange = true;
    //     Messenger<string, string>.Broadcast(_toSendEnterMessage.ToString(), gameObject.name, _interactionMessage.ToString());
    //     //Debug.Log("Sending enter trigger into " + gameObject.name);

    // }
    // public void OnTriggerExit(Collider other){ 
    //     Messenger<string, string>.Broadcast(_toSendExitMessage.ToString(), gameObject.name, _interactionMessage.ToString());
    //     //Debug.Log("Sending exit trigger from " + gameObject.name);
    // }

    void Start() {
        //OpenDoor openDoor;
        isCollectable =false;// TryGetComponent<OpenDoor> (out openDoor);
    }
    void Update(){
        if(enteredInRange){
            float lookingScore = LookingCondition(playerTransform, gameObject.transform);
            if(lookingScore > 0){
                InteractableManager.SetInteractableGameObject(new Tuple<InteractableTrigger, float>(this, lookingScore), isCollectable);
                if(!looking){
                    Debug.Log("Looking");
                    looking = true;
                }
            }else{
                if(looking){
                    looking = false;
                    InteractableManager.DeselectInteractableGameObject(this, isCollectable);
                    Debug.Log("Not Looking anymore");
                }
            }
        }     
    }
   void OnTriggerEnter(Collider collider){
        if(collider.tag == Settings.PLAYER_TAG){
            enteredInRange = true;
            playerTransform = collider.gameObject.transform;
        }

    }
    void OnTriggerExit(Collider collider){
        if(collider.tag == Settings.PLAYER_TAG){
            enteredInRange = false;
            if(looking){
                looking = false;
                Debug.Log("Exited");
                //Messenger<string, string>.Broadcast(_toSendExitMessage.ToString(), gameObject.name, InteractionMessage.ToString());
                InteractableManager.DeselectInteractableGameObject(this, isCollectable);
                //gameObject.SendMessage("CannotInteract");
            }
        }
    }

    public float LookingCondition(Transform source, Transform target){
        if(ignoreLookingScore)
            return 1f;
        Vector3 sourceDirection =  target.transform.position - source.position;
        return Vector3.Dot(source.forward, sourceDirection) ;
    }
}