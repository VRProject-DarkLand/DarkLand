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
    private IInteractableObject interactableObject;

    [SerializeField] private bool reactToEnemy = false;

    public bool isCollectable  {get; internal set;} = false;


    void Start() {
        //OpenDoor openDoor;
        interactableObject = GetComponent<IInteractableObject>();

    }
    /// <summary>
    /// if the player entered in the trigger, it computes a looking score, if the player can interact with the interactable object, then it tries to set it as
    /// the selected interactable. If looking score is not sufficient, it tries to deselect the object from the selected interactable in the manager
    /// </summary>
    /// <param name="isInventoryOpen">actual state of the inventory type</param>
    void Update(){
        if(enteredInRange){
            float lookingScore = LookingCondition(playerTransform, gameObject.transform);
            if(lookingScore > 0){
                bool can_do = true;
                if(interactableObject != null){
                    can_do = interactableObject.CanInteract();
                }
                //Debug.Log(gameObject.name +" " +lookingScore);
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

    /// <summary>
    /// If the player entered the trigger, it updates its internal states and enable the possibility to be selected 
    /// if an a reactive enemy entered the trigger it calls the reactive interaction of the interactable object
    /// </summary>
    /// <param name="Collider">object that triggered the event</param>
   void OnTriggerEnter(Collider collider){
        if(collider.tag == Settings.PLAYER_TAG){
            enteredInRange = true;
            playerTransform = Camera.main.transform;
        }
        if(!reactToEnemy)
            return;
        if(collider.tag == Settings.INTERACTION_ENEMY_TAG){
            interactableObject.ReactiveInteraction();
        }

    }
    /// <summary>
    /// If the player exited the trigger, it tries to deselect itself 
    /// </summary>
    /// <param name="Collider">object that triggered the event</param>
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

    public void RemoveFromInteractables(){
        InteractableManager.DeselectInteractableGameObject(this);
    }

    /// <summary>
    /// Computes a values that indicates how likely is the source to select the item by looking to it
    /// if the object ignores this value, it return 0.9 as default.
    /// </summary>
    /// <param name="source">object transform  that can select the item</param>
    /// <param name="target">object transform of the candidate selectable item</param>
    /// <return name=float>a value between 0 and 1 whene 0 is the oppposite direction and 1 is the direct looking by the source</return>
    public float LookingCondition(Transform source, Transform target){
        if(ignoreLookingScore)
            return 0.9f;
        Vector3 sourceDirection = target.transform.position - source.position ;
        if(isCollectable){
            RaycastHit hit;
            if(Physics.Raycast(target.position, -sourceDirection, out hit, 10)){
                //Debug.Log("LOOK: "+ hit.collider.gameObject.name+ " "+);
                if(hit.collider.tag != Settings.PLAYER_TAG ){
                    Debug.Log("LOOK: "+ hit.collider.gameObject.name);
                    return 0;
                }
            }
        }
        Debug.Log(gameObject.name +" "+ Vector3.Dot(source.forward, sourceDirection.normalized));
        return Vector3.Dot(source.forward, sourceDirection.normalized) ;
    }
}
