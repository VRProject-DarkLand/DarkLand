using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class OpenDoubleDoor :  IInteractableObject
{
    private Tuple<Quaternion,Quaternion> open;
    private Tuple<Quaternion,Quaternion> close;
    private bool opened ;    
    private bool isMoving;
    private bool canInteract;
    private float speed;
    private float timeCount;
    private Transform playerTransform;
    [SerializeField] private float left_rotation;
    [SerializeField] private float right_rotation;
    [SerializeField] private GameObject left;
    [SerializeField] private GameObject right;
    
    [SerializeField] private bool requireKey = false;
    [SerializeField] private string key = "Key";
    // Start is called before the first frame updatevoid Start()
    void Start(){
        opened = false;
        canInteract = false;
        isMoving = false;
        speed = 1f;
        timeCount = 0f; 
        interactableTrigger = GetComponent<InteractableTrigger>();
         interactableTrigger = GetComponent<InteractableTrigger>();
        if(!requireKey)
            interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.OPEN_DOOR);
        else
            interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.UNLOCK);
        close  = Tuple.Create(left.transform.rotation, right.transform.rotation);
        open = Tuple.Create(close.Item1*Quaternion.Euler(0, left.transform.rotation.y+left_rotation, 0), close.Item2*Quaternion.Euler(0, right.transform.rotation.y+right_rotation, 0));
    }

       private IEnumerator AnimateDoor(){
        Tuple<Quaternion,Quaternion> begin = close;
        Tuple<Quaternion,Quaternion> end = open;
        if(!opened){
            begin = open;
            end = close;
        }
        while(isMoving){
            left.transform.rotation = Quaternion.Lerp(begin.Item1, end.Item1,  timeCount * speed);
            right.transform.rotation = Quaternion.Lerp(begin.Item2, end.Item2, timeCount * speed);
            timeCount += Time.deltaTime;
            if(left.transform.rotation == end.Item1 && right.transform.rotation == end.Item2 ){
                isMoving = false;
                timeCount = 0;
            }
            yield return null;
        }
    }

    public void ChangeState(){
        if(left != null && right != null){
              if(requireKey){
                if(Managers.Inventory.GetItemCount(key) == 0)
                    return;
                else{ 
                    requireKey = false;
                    Managers.Inventory.ConsumeItem(key);
                    interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.OPEN_DOOR);
                    return;
                }
              }
                // if(opened){
                //     left.transform.rotation = close.Item1 ;
                //     right.transform.rotation = close.Item2 ;
                // }else{
                //     left.transform.rotation = open.Item1 ;
                //     right.transform.rotation = open.Item2 ;
                // }
                if(opened)
                    interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.OPEN_DOOR);
                else
                    interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.CLOSE_DOOR);
                opened = !opened;   
                isMoving = true;  
        }
    }
    public override void Interact(){
        if(!isMoving){
            ChangeState();
            StartCoroutine(AnimateDoor());
        }
    }
}
