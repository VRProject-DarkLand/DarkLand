using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class InteractableTrigger : MonoBehaviour{
    [SerializeField] public GameEvent.InteractWithMessage InteractionMessage {get; private set;} = GameEvent.InteractWithMessage.INTERACT;
    [SerializeField] protected bool ignoreLookingScore = false;
    protected bool enteredInRange = false;
    protected Transform playerTransform;

    private bool looking = false;

    public bool isCollectable  {get; internal set;} = false;

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
    }
    void Update(){
        IInteractableObject obj = GetComponent<IInteractableObject>();
        if(enteredInRange){
            float lookingScore = LookingCondition(playerTransform, gameObject.transform);
            if(lookingScore > 0){
                bool can_do = true;
                if(obj != null){
                    can_do = obj.CanInteract();
                }
                InteractableManager.SetInteractableGameObject(new Tuple<InteractableTrigger, float, bool>(this, lookingScore, can_do));
                if(!looking){
                    looking = true;
                }
            }else{
                if(looking){
                    looking = false;
                    InteractableManager.DeselectInteractableGameObject(this);
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
                //Messenger<string, string>.Broadcast(_toSendExitMessage.ToString(), gameObject.name, InteractionMessage.ToString());
                InteractableManager.DeselectInteractableGameObject(this);
                //gameObject.SendMessage("CannotInteract");
            }
        }
    }

    public void SetInteractionMessage(GameEvent.InteractWithMessage message){
        InteractionMessage = message;
        InteractableManager.DeselectInteractableGameObject(this);
    }

    void OnDestroy(){
         InteractableManager.DeselectInteractableGameObject(this);
    }

    public float LookingCondition(Transform source, Transform target){
        if(ignoreLookingScore)
            return 1f;
        Vector3 sourceDirection =  target.transform.position - source.position;
        return Vector3.Dot(source.forward, sourceDirection) ;
    }
}
