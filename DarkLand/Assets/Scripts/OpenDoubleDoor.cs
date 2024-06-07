using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class OpenDoubleDoor :  IInteractableObject
{
    private Tuple<Quaternion,Quaternion> open;
    private Tuple<Quaternion,Quaternion> close;
    private bool opened ;    
    private bool isMoving;
    private float speed;
    private float timeCount;
    private Transform playerTransform;
    [SerializeField] private float left_rotation;
    [SerializeField] private float right_rotation;
    [SerializeField] private GameObject left;
    [SerializeField] private GameObject right;
    [SerializeField] private NavMeshLink link;
    
    private IEnumerator coroutine;
    
    [SerializeField] private bool requireKey = false;
    [SerializeField] private string key = "Key";
    // Start is called before the first frame updatevoid Start()
    void Start(){
        opened = false;
        isMoving = false;
        speed = 1f;
        timeCount = 0f; 
        if(link != null)
            link.enabled = !requireKey;
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

    public override bool CanInteract()
    {
         if(left != null && right != null){
            if(requireKey){
                if(Managers.Inventory.GetItemCount(key) == 0)
                    return false;
            }
        }
        return true;

    }

    public override void ReactiveInteraction(){
        if(!requireKey){
            isMoving = false;
            if(coroutine != null)
                StopCoroutine(coroutine);
            timeCount = 1;
            left.transform.rotation =open.Item1;
            right.transform.rotation = open.Item2;
            opened = true;
            interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.CLOSE_DOOR);
        }
    }


    public void ChangeState(){
        if(left != null && right != null){
              if(CanInteract()){
                 if(requireKey){
                    requireKey = false;
                    if(link != null)
                        link.enabled = true;
                    //Managers.Inventory.ConsumeItem(key);
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
            if(opened){
                interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.OPEN_DOOR);
                interactionSound = ResourceLoader.GetSound(Settings.AudioSettings.DOOR_CLOSE_SOUND);
                
            }   else {
                interactableTrigger.SetInteractionMessage(GameEvent.InteractWithMessage.CLOSE_DOOR);
                interactionSound =  ResourceLoader.GetSound(Settings.AudioSettings.DOOR_OPEN_SOUND);
            }
            Managers.AudioManager.PlaySound(interactionSound);
                opened = !opened;   
                isMoving = true;  
                timeCount = 0;
        }
    }
    public override void Interact(){
        if(!isMoving){
            ChangeState();
            StartCoroutine(AnimateDoor());
        }
    }
}
