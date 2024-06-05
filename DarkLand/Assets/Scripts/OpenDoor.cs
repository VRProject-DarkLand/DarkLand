using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class OpenDoor : IInteractableObject{
    private Quaternion open;
    private Quaternion close;
    private bool opened ;
    private bool isMoving;
    private float speed;
    private float timeCount;
    [SerializeField] private float rotation = -90f;
    [SerializeField] private GameObject door;
    [SerializeField] private bool requireKey = false;
    [SerializeField] private string key = "Key";
    // Start is called before the first frame update
    void Start(){
        if(door != null){
            isMoving = false;
            opened = false;
            close  = door.transform.rotation;
            open = close*Quaternion.Euler(0, door.transform.rotation.y+rotation, 0);
        }
        interactableTrigger = GetComponent<InteractableTrigger>();
        if(!requireKey)
            interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.OPEN_DOOR);
        else
            interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.UNLOCK);
        timeCount = 0;
        speed = 1f;
    }

    // Update is called once per frame
    // void Update(){
    //     if(isMoving){
    //         Quaternion begin = open;
    //         Quaternion end = close;
    //         if(opened){
    //             begin = close;
    //             end = open;
    //         }
    //         door.transform.rotation = Quaternion.Lerp(begin, end,  timeCount * speed);
    //         timeCount += Time.deltaTime;
    //         if(door.transform.rotation == end){
    //             isMoving = false;
    //             timeCount = 0;
    //         }
    //     }
    // }

    /* *
    * <summary><\summary> 
    * <param name=""><\param>
    * <return><\return>
    *
    */
    private IEnumerator AnimateDoor(){
        Quaternion begin = open;
        Quaternion end = close;
        if(opened){
            begin = close;
            end = open;
        }
        while(isMoving){
            door.transform.rotation = Quaternion.Lerp(begin, end,  timeCount * speed);
            timeCount += Time.deltaTime;
            if(door.transform.rotation == end){
                isMoving = false;
                timeCount = 0;
            }
            yield return null;
        }
    }

    public override bool CanInteract()
    {
        if(door != null){
            if(requireKey){
                if(Managers.Inventory.GetItemCount(key) == 0)
                    return false;
            }
        }
        return true;

    }

    public void ChangeState(){
            if(CanInteract()){
                if(requireKey){
                    requireKey = false;
                    //Managers.Inventory.ConsumeItem(key);
                    interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.OPEN_DOOR);
                    return;
                }
            }   
            //door.transform.rotation = opened ? close : open; 
            if(opened)
                interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.OPEN_DOOR);
            else 
                interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.CLOSE_DOOR);
            opened = !opened;   
            isMoving = true;     
    }

    public override void Interact(){
        if(!isMoving){
            ChangeState();
            StartCoroutine(AnimateDoor());
        }
    }
}
